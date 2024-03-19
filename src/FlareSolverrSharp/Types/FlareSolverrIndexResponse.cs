using Newtonsoft.Json;

namespace FlareSolverrSharp.Types
{
    public class FlareSolverrIndexResponse
    {
        [JsonProperty("msg")]
        public string Message;
        public string Version;
        public string UserAgent;
    }
}
