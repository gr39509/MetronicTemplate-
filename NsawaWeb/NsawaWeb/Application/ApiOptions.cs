namespace NsawaWeb.Application;

public sealed class ApiOptions
{
    public const string SectionName = "Api";

    public string BaseUrl { get; set; } = string.Empty;
}

public static class ImageProxyClient
{
    public const string Name = "files";
}
