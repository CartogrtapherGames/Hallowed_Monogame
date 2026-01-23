using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Hallowed.Core.IO;

public static class JsonConfig
{
  public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
  {
    ContractResolver = new CamelCasePropertyNamesContractResolver(),
    Formatting = Formatting.Indented,
    NullValueHandling = NullValueHandling.Ignore,
    
    // ⭐ Add these two lines for polymorphic serialization
    TypeNameHandling = TypeNameHandling.Auto,
    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
  };
}
