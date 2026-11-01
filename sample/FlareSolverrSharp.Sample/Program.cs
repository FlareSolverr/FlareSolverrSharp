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
		await ClearanceHandlerSample.SampleGet();
	}

}