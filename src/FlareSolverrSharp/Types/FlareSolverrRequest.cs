using System.Text.Json.Serialization;

namespace FlareSolverrSharp.Types
{
    public class FlareSolverrRequest
    {
        [JsonPropertyName("cmd")]
        public string Cmd { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("session")]
        public string Session { get; set; }

        [JsonPropertyName("proxy")]
        public FlareSolverrRequestProxy Proxy { get; set; }
    }
}
