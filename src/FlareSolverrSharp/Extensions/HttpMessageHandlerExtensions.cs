using System.Net.Http;

// ReSharper disable TailRecursiveCall
// ReSharper disable InconsistentNaming

namespace FlareSolverrSharp.Extensions;

public static class HttpMessageHandlerExtensions
{
	extension(HttpMessageHandler self)
	{

		public HttpMessageHandler GetInnermostHandler()
		{
			return self is DelegatingHandler handler
				       ? handler.InnerHandler.GetInnermostHandler()
				       : self;
		}

	}
}