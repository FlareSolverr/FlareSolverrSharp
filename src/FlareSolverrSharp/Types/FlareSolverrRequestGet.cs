using Newtonsoft.Json;

namespace FlareSolverrSharp.Types
{
    public class FlareSolverrRequestGet : FlareSolverrRequest
    {
        [JsonProperty("maxTimeout")]
        public int MaxTimeout;
    }
}