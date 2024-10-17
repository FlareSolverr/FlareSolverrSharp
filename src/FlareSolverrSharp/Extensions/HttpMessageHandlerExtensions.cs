using System.Net.Http;
// ReSharper disable TailRecursiveCall
// ReSharper disable InconsistentNaming

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