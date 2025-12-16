using System;
using System.Collections.Generic;
using FlareSolverrSharp.Exceptions;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlareSolverrSharp.Sample;

public static class Program
{

	public static async Task Main()
	{
		/*ClearanceHandlerSample.SampleGet().Wait();
		ClearanceHandlerSample.SamplePostUrlEncoded().Wait();*/

		var handler = new ClearanceHandler(Settings.FlareSolverrApiUrl)
		{
			EnsureResponseIntegrity = false,
			Solverr =
			{
				MaxTimeout = 60000
			}
		};

		var client = new HttpClient(handler);

		HttpRequestMessage[] rg =
		[
			new(HttpMethod.Get, "https://ascii2d.net/search/url/https://pomf2.lain.la/f/fy32pj5e.png"),
			new(HttpMethod.Get, "https://ascii2d.net/search/url/https://i.redd.it/xixxli0axz7b1.jpg"),
		];

		await Parallel.ForEachAsync(rg, async (x, y) =>
		{
			var res = await client.SendAsync(x, y);
			Console.WriteLine($"{x.RequestUri} -> {res.StatusCode}");
			return;
		});
	}

}