using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FlareSolverrSharp.Sample
{
    public static class ClearanceHandlerSample
    {

        public static async Task SampleGet()
        {
            var handler = new ClearanceHandler("http://localhost:8191/")
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36",
                MaxTimeout = 60000
            };

            var client = new HttpClient(handler);
            var content = await client.GetStringAsync("https://uam.hitmehard.fun/HIT");
            Console.WriteLine(content);
        }

        public static async Task SamplePostUrlEncoded()
        {
            var handler = new ClearanceHandler("http://localhost:8191/")
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36",
                MaxTimeout = 60000
            };

            var request = new HttpRequestMessage();
            request.Headers.ExpectContinue = false;
            request.RequestUri = new Uri("https://uam.hitmehard.fun/HIT");
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36");
            var postData = new Dictionary<string, string> { { "story", "test" }};
            request.Content = FormUrlEncodedContentWithEncoding(postData, Encoding.UTF8);
            //request.Content.Headers.Remove("Content-Type");
            //request.Content.Headers.Add("Content-Type", "multipart/form-data; boundary=6");
            request.Method = HttpMethod.Post;

            var client = new HttpClient(handler);
            var content = await client.SendAsync(request);
            Console.WriteLine(content);
        }

        static ByteArrayContent FormUrlEncodedContentWithEncoding(
            IEnumerable<KeyValuePair<string, string>> nameValueCollection, Encoding encoding)
        {
            // utf-8 / default
            if (Encoding.UTF8.Equals(encoding) || encoding == null)
                return new FormUrlEncodedContent(nameValueCollection);

            // other encodings
            var builder = new StringBuilder();
            foreach (var pair in nameValueCollection)
            {
                if (builder.Length > 0)
                    builder.Append('&');
                builder.Append(HttpUtility.UrlEncode(pair.Key, encoding));
                builder.Append('=');
                builder.Append(HttpUtility.UrlEncode(pair.Value, encoding));
            }
            // HttpRuleParser.DefaultHttpEncoding == "latin1"
            var data = Encoding.GetEncoding("latin1").GetBytes(builder.ToString());
            var content = new ByteArrayContent(data);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            return content;
        }

    }
}