using System.Text.Json.Serialization;

namespace FlareSolverrSharp.Types
{
    public class FlareSolverrRequestPost : FlareSolverrRequest
    {
        [JsonPropertyName("postData")]
        public string PostData { get; set; }

        [JsonPropertyName("maxTimeout")]
        public int MaxTimeout { get; set; }
    }
}