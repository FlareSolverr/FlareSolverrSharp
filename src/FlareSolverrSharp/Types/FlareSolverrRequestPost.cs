using System.Text.Json.Serialization;

namespace FlareSolverrSharp.Types
{
    public class FlareSolverrRequestPost : FlareSolverrRequest
    {
        [JsonPropertyName("postData")]
        public string PostData;

        [JsonPropertyName("maxTimeout")]
        public int MaxTimeout;
    }
}