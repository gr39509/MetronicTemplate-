using NSwag;
using NSwag.CodeGeneration.CSharp;

namespace NovaAccounts.SharedModels;

using NSwag;
using NSwag.CodeGeneration.CSharp;

class Program
{
    static async Task Main(string[] args)
    {
        // 1. Load the OpenAPI/Swagger specification
        var openApiDocument = await OpenApiDocument.FromUrlAsync("http://185.132.37.188:8070/swagger/./v7/swagger.json");

        // 2. Configure the generator settings
        var settings = new CSharpClientGeneratorSettings
        {
            ClassName = "MyApiClient",
            CSharpGeneratorSettings = 
            {
                Namespace = "MyProject.Clients",
            },
            // Use the following settings to generate only the models (DTOs)
            // GenerateClientClasses = false,
            // GenerateClientInterfaces = false
        };

        // 3. Generate the C# code
        var generator = new CSharpClientGenerator(openApiDocument, settings);
        var csharpCode = generator.GenerateFile();

        // 4. Save the code to a file
        await File.WriteAllTextAsync("MyApiClient.cs", csharpCode);
        
        Console.WriteLine("C# client generated successfully!");
    }
}