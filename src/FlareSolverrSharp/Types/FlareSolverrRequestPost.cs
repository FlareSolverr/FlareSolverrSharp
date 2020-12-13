using Newtonsoft.Json;

namespace FlareSolverrSharp.Types
{
    public class FlareSolverrRequestPost : FlareSolverrRequest
    {
        [JsonProperty("postData")]
        public string PostData;

        [JsonProperty("maxTimeout")]
        public int MaxTimeout;
    }
}