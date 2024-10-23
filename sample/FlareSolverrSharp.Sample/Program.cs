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

		var x = await client.GetAsync(Settings.ProtectedUri);

		Console.WriteLine(x);
		Console.WriteLine( ChallengeDetector.IsClearanceRequiredAsync(x));
		/*foreach (KeyValuePair<string, IEnumerable<string>> pair in x.Headers) {
			Console.WriteLine(pair);
		}*/
		foreach (var y in x.Headers.Server) {
			
			Console.WriteLine(y.Product.Name);
		}
		// Assert.IsTrue(c.Message.Contains("Error connecting to FlareSolverr server"));
	}

}