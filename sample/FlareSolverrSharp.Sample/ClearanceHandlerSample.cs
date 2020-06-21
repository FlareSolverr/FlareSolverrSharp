using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlareSolverrSharp.Sample
{
    public static class ClearanceHandlerSample
    {

        public static async Task Sample()
        {
            var handler = new ClearanceHandler("http://localhost:8191/")
            {
                UserAgent = "Mozilla/5.0 (X11; Linux x86_64; rv:76.0) Gecko/20100101 Firefox/76.0",
                MaxTimeout = 60000
            };

            var client = new HttpClient(handler);
            var content = await client.GetStringAsync("https://uam.hitmehard.fun/HIT");
            Console.WriteLine(content);
        }

    }
}