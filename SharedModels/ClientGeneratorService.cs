using NSwag;
using NSwag.CodeGeneration.CSharp;

public class ClientGeneratorService
{
    private readonly HttpClient _httpClient;

    public ClientGeneratorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GenerateClientAsync(string swaggerJsonUrl)
    {
        try
        {
            // Method 1: Download JSON first and fix references
            var json = await _httpClient.GetStringAsync(swaggerJsonUrl);
                
            // Fix the problematic references in the JSON
            json = json.Replace("CreateRoleCommand+SelectedPermission", "CreateRoleCommand.SelectedPermission");
            json = json.Replace("#/components/schemas/", "#/components/schemas/");
                
            // Load from fixed JSON
            var document = await OpenApiDocument.FromJsonAsync(json);
                
            // Configure generator settings
            var settings = new CSharpClientGeneratorSettings
            {
                ClassName = "ApiClient",
                CSharpGeneratorSettings = 
                {
                    Namespace = "NovaAccounts.SharedModels.ApiService",
                },
                
                // GenerateClientClasses = true,
                // GenerateClientInterfaces = false,
                 UseBaseUrl = false,
                 InjectHttpClient = true,
                // ExceptionClass = "ApiException",
                //
                // // Add these settings to handle problematic schemas
                // GenerateExceptionClasses = false,
                // GenerateOptionalParameters = true
            };
                
            // Generate the code
            var generator = new CSharpClientGenerator(document, settings);
            return generator.GenerateFile();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to generate client: {ex.Message}", ex);
        }
    }
}