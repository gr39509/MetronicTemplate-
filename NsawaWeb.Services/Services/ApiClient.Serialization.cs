// Hand-written companion to the NSwag-generated ApiClient.cs (survives regeneration).
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NsawaWeb.Services.Services;

public partial class ApiClient
{
    // The generated models mark many properties Required.DisallowNull / Always, but the API
    // does send nulls (e.g. "group": null on donations without a group). Treat every property
    // as optional so one null field doesn't fail the whole response.
    static partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
    {
        settings.ContractResolver = new LenientContractResolver();
    }

    private sealed class LenientContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            property.Required = Required.Default;
            return property;
        }
    }
}
