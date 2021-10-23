using Newtonsoft.Json;

namespace FlareSolverrSharp.Types
{
    public class FlareSolverrRequest
    {
        [JsonProperty("cmd")]
        public string Cmd;

        [JsonProperty("url")]
        public string Url;

        [JsonProperty("session")]
        public string Session;

        [JsonProperty("proxy")]
        public FlareSolverrRequestProxy Proxy;
    }
}
