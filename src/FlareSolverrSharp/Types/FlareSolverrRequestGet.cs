
using Newtonsoft.Json;

namespace FlareSolverrSharp.Types
{
    public class FlareSolverrRequestGet : FlareSolverrRequest
    {
        [JsonProperty("headers")]
        public string Headers;

        [JsonProperty("maxTimeout")]
        public int MaxTimeout;
    }
}