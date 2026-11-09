using Microsoft.JSInterop;

namespace NsawaWeb.Services.Services;

/// <summary>Where AuthService keeps the sign-in token and user name between visits.</summary>
public interface ITokenStorage
{
    /// <returns>The stored value, or null when missing or storage is unavailable.</returns>
    Task<string?> GetAsync(string key);
    Task SetAsync(string key, string value);
    Task RemoveAsync(string key);
}

/// <summary>Browser localStorage, used by the website.</summary>
public sealed class BrowserTokenStorage(IJSRuntime jsRuntime) : ITokenStorage
{
    public async Task<string?> GetAsync(string key)
    {
        try
        {
            var value = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", key);
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
        catch (Exception ex) when (ex is JSException or InvalidOperationException or JSDisconnectedException or TaskCanceledException)
        {
            // JS is unavailable (no interactive circuit yet, or the circuit closed).
            return null;
        }
    }

    public async Task SetAsync(string key, string value) =>
        await jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);

    public async Task RemoveAsync(string key) =>
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
}
