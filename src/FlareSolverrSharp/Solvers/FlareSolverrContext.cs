using System.Text.Json.Serialization;
using FlareSolverrSharp.Types;

namespace FlareSolverrSharp.Solvers
{
    [JsonSerializable(typeof(FlareSolverrRequest))]
    [JsonSerializable(typeof(FlareSolverrResponse))]
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    public partial class FlareSolverrContext : JsonSerializerContext
    {
    }
}