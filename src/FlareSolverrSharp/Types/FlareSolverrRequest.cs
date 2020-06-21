
using Newtonsoft.Json;

namespace FlareSolverrSharp.Types
{
    public class FlareSolverrRequest
    {
        [JsonProperty("method")]
        public string Method;

        [JsonProperty("url")]
        public string Url;

        [JsonProperty("userAgent")]
        public string UserAgent;

        [JsonProperty("maxTimeout")]
        public int MaxTimeout;
    }
}