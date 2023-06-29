using System.Text.Json.Serialization;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable ClassNeverInstantiated.Global
namespace FlareSolverrSharp.Types
{
    public class FlareSolverrResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
        
        [JsonPropertyName("startTimestamp")]
        public long StartTimestamp { get; set; }

        [JsonPropertyName("endTimestamp")]
        public long EndTimestamp { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("solution")]
        public Solution Solution { get; set; }
    }

    public class Solution
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("headers")]
        public Headers Headers { get; set; }

        [JsonPropertyName("response")]
        public string Response { get; set; }

        [JsonPropertyName("cookies")]
        public Cookie[] Cookies { get; set; }

        [JsonPropertyName("userAgent")]
        public string UserAgent { get; set; }
    }

    public class Cookie
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; }
        
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("expiry")]
        public double Expires { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }
        
        [JsonPropertyName("httpOnly")]
        public bool HttpOnly { get; set; }

        [JsonPropertyName("secure")]
        public bool Secure { get; set; }

        [JsonPropertyName("session")]
        public bool Session { get; set; }

        [JsonPropertyName("sameSite")]
        public string SameSite { get; set; }

        public string ToHeaderValue() => $"{Name}={Value}";
        public System.Net.Cookie ToCookieObj() => new System.Net.Cookie(Name, Value);

    }

    public class Headers
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
        
        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("content-type")]
        public string ContentType { get; set; }
    }
}