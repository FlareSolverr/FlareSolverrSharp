using Newtonsoft.Json;

namespace FlareSolverrSharp.Types
{
    public class FlareSolverrRequestProxy
    {
        [JsonProperty("url")]
        public string Url;

        [JsonProperty("username")]
        public string Username;

        [JsonProperty("password")]
        public string Password;
    }
}
