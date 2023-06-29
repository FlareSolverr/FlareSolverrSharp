using System.Text.Json.Serialization;

namespace FlareSolverrSharp.Types
{
    [JsonDerivedType(typeof(FlareSolverrRequestGet))]
    [JsonDerivedType(typeof(FlareSolverrRequestPost))]
    public class FlareSolverrRequest
    {
        [JsonPropertyName("cmd")]
        public string Cmd;

        [JsonPropertyName("url")]
        public string Url;

        [JsonPropertyName("session")]
        public string Session;

        [JsonPropertyName("proxy")]
        public FlareSolverrRequestProxy Proxy;
    }
}
