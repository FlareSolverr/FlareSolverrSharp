// Author: Deci | Project: FlareSolverrSharp | Name: FlareSolverrContext.cs
// Date: 2024/10/23 @ 18:10:09

using System.Text.Json.Serialization;
using FlareSolverrSharp.Types;

namespace FlareSolverrSharp.Solvers;

[JsonSerializable(typeof(FlareSolverrRequest))]
[JsonSerializable(typeof(FlareSolverrIndexResponse))]
[JsonSerializable(typeof(FlareSolverrResponse))]
[JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class FlareSolverrContext : JsonSerializerContext
{
}