using Newtonsoft.Json;

namespace FlareSolverrSharp.Types
{
    public class FlareSolverrRequestPostUrlEncoded : FlareSolverrRequestPost
    {
        [JsonProperty("headers")]
        public HeadersPost Headers;
    }

    public class HeadersPost
    {
        [JsonProperty(PropertyName = "Content-Type")]
        public string ContentType;

        [JsonProperty(PropertyName = "Content-Length")]
        public string ContentLength;
    }
}
