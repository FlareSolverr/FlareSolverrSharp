using System.ComponentModel;
using Newtonsoft.Json;

namespace FlareSolverrSharp.Types
{
    public class FlareSolverrRequestProxy
    {
        [JsonProperty("url")]
        public string Url;

        [DefaultValue("")]
        [JsonProperty("username", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Username;

        [DefaultValue("")]
        [JsonProperty("password", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Password;
    }
}
