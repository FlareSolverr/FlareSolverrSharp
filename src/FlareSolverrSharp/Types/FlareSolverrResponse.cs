using Newtonsoft.Json;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace FlareSolverrSharp.Types
{
    public class FlareSolverrResponse
    {
        public string Status;
        public string Message;
        public long StartTimestamp;
        public long EndTimestamp;
        public string Version;
        public Solution Solution;
        public string Session;
        public string[] Sessions;
    }

    public class Solution
    {
        public string Url;
        public string Status;
        public Headers Headers;
        public string Response;
        public Cookie[] Cookies;
        public string UserAgent;
    }

    public class Cookie
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("value")]
        public string Value;

        [JsonProperty("domain")]
        public string Domain;

        [JsonProperty("path")]
        public string Path;

        [JsonProperty("expiry", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public int Expiry;

        [JsonProperty("httpOnly", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool HttpOnly;

        [JsonProperty("secure", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool Secure;

        [JsonProperty("sameSite")]
        public string SameSite;

        public string ToHeaderValue() => $"{Name}={Value}";

        public System.Net.Cookie ToCookieObj() => new System.Net.Cookie(Name, Value);
    }

    public class Headers
    {
        public string Status;
        public string Date;
        [JsonProperty(PropertyName = "content-type")]
        public string ContentType;
    }
}
