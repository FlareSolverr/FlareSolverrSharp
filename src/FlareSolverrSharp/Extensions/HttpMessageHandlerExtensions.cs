using System.Net.Http;
// ReSharper disable TailRecursiveCall

namespace FlareSolverrSharp.Extensions;

internal static class HttpMessageHandlerExtensions
{
	public static HttpMessageHandler GetInnermostHandler(this HttpMessageHandler self)
	{
		return self is DelegatingHandler handler
			       ? handler.InnerHandler.GetInnermostHandler()
			       : self;
	}
}