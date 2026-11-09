using System.Text.Json;
using NsawaWeb.Services.Services;

namespace NsawaWeb.Application;

/// <summary>
/// Runs NSwag client calls and turns every outcome into a <see cref="Result"/>.
/// Technical details are logged; people only ever see plain messages.
/// </summary>
public sealed class ApiRunner(ILogger<ApiRunner> logger, AuthService authService)
{
    public const string Offline = "We can't reach the Nsawa server. Check your connection and try again.";
    public const string ServerProblem = "Something went wrong on our side. Please try again in a moment.";
    public const string SessionEnded = "Your session has ended. Please sign in again.";

    public async Task<Result<T>> RunAsync<T>(Func<Task<IApiResult<T>>> call, string action, Func<int, string?>? statusMessage = null)
    {
        try
        {
            var response = await call();
            if (response is null)
            {
                return Result<T>.Fail(ServerProblem);
            }

            return response.Success
                ? Result<T>.Ok(response.Data, Clean(response.Message))
                : Result<T>.Fail(Clean(response.Message) ?? $"We couldn't {action}. Please try again.");
        }
        catch (ApiException ex)
        {
            var (succeeded, serverMessage) = ReadBody(ex.Response);

            // The generated client throws on a 200 it can't deserialize (e.g. an empty data field).
            if (ex.StatusCode is >= 200 and < 300 && succeeded == true)
            {
                return Result<T>.Ok(default!, serverMessage);
            }

            var custom = statusMessage?.Invoke(ex.StatusCode);
            if (custom is not null)
            {
                logger.LogInformation("API returned {Status} while trying to {Action}", ex.StatusCode, action);
                return Result<T>.Fail(custom);
            }

            if (ex.StatusCode == 401)
            {
                logger.LogInformation("API returned 401 while trying to {Action}; signing out", action);
                await authService.LogoutAsync();
                return Result<T>.Fail(SessionEnded);
            }

            if (ex.StatusCode >= 500)
            {
                logger.LogError(ex, "API error {Status} while trying to {Action}", ex.StatusCode, action);
            }
            else
            {
                logger.LogWarning("API returned {Status} while trying to {Action}: {Body}", ex.StatusCode, action, Truncate(ex.Response));
            }

            return Result<T>.Fail(DefaultMessage(ex.StatusCode, action, serverMessage));
        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Network error while trying to {Action}", action);
            return Result<T>.Fail(Offline);
        }
        catch (TaskCanceledException ex)
        {
            logger.LogWarning(ex, "Request timed out while trying to {Action}", action);
            return Result<T>.Fail(Offline);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while trying to {Action}", action);
            return Result<T>.Fail(ServerProblem);
        }
    }

    public async Task<Result> RunAsync(Func<Task<IApiResult<string>>> call, string action, Func<int, string?>? statusMessage = null)
    {
        var result = await RunAsync<string>(call, action, statusMessage);
        return result.Succeeded ? Result.Ok(result.Message) : Result.Fail(result.Message!);
    }

    private static string DefaultMessage(int status, string action, string? serverMessage) => status switch
    {
        400 or 409 or 422 when serverMessage is not null => serverMessage,
        400 or 422 => $"We couldn't {action}. Check the details and try again.",
        403 => "You don't have permission to do that.",
        404 => "We couldn't find what you were looking for. It may have been removed.",
        409 => "That already exists.",
        429 => "Too many attempts. Wait a minute and try again.",
        _ => ServerProblem
    };

    private static (bool? Success, string? Message) ReadBody(string? body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return (null, null);
        }

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                return (null, null);
            }

            bool? success = TryGet(root, "success", out var s) && s.ValueKind is JsonValueKind.True or JsonValueKind.False
                ? s.GetBoolean()
                : null;

            string? message = null;
            if (TryGet(root, "message", out var m) && m.ValueKind == JsonValueKind.String)
            {
                message = m.GetString();
            }
            else if (TryGet(root, "errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
            {
                message = errors.EnumerateObject()
                    .SelectMany(p => p.Value.ValueKind == JsonValueKind.Array ? p.Value.EnumerateArray().Select(v => v.GetString()) : [])
                    .FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
            }
            else if (TryGet(root, "title", out var t) && t.ValueKind == JsonValueKind.String)
            {
                message = t.GetString();
            }

            return (success, Clean(message));
        }
        catch (JsonException)
        {
            return (null, null);
        }
    }

    private static bool TryGet(JsonElement obj, string name, out JsonElement value)
    {
        foreach (var property in obj.EnumerateObject())
        {
            if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    /// <summary>Only pass on server text that reads like a sentence for people.</summary>
    private static string? Clean(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return null;
        }

        message = message.Trim();
        var looksTechnical = message.Length > 240
                             || message.Contains("Exception", StringComparison.Ordinal)
                             || message.Contains(" at ", StringComparison.Ordinal) && message.Contains(".cs", StringComparison.Ordinal)
                             || message.StartsWith('{')
                             || message.StartsWith('<');
        if (looksTechnical || string.Equals(message, "successful", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return message;
    }

    private static string Truncate(string? value) =>
        value is null ? "(empty)" : value.Length <= 500 ? value : value[..500];
}
