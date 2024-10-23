using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using FlareSolverrSharp.Constants;
using FlareSolverrSharp.Exceptions;
using FlareSolverrSharp.Types;
using FlareSolverrSharp.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace FlareSolverrSharp.Solvers;

public class FlareSolverr : INotifyPropertyChanged
{

	internal static readonly JsonSerializerOptions JsonSerializerOptions1 = new()
	{
		PropertyNameCaseInsensitive = true,
		PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
		IncludeFields               = true,

		// NumberHandling              = JsonNumberHandling.Strict | JsonNumberHandling.AllowReadingFromString,
		// DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
	};

	internal static readonly JsonSerializerOptions JsonSerializerOptions2 = new(JsonSerializerOptions1)
	{
		DefaultIgnoreCondition =
			JsonIgnoreCondition.WhenWritingDefault,
	};

	private static readonly SemaphoreLocker s_locker = new SemaphoreLocker();

	private /*readonly*/ HttpClient m_httpClient;

	public Uri FlareSolverrApi { get; }

	private readonly Uri _flareSolverrIndexUri;
	private          int m_maxTimeout;

	public int MaxTimeout
	{
		get => m_maxTimeout;
		set
		{
			SetField(ref m_maxTimeout, value);
			m_httpClient.Timeout = AdjustHttpClientTimeout();
		}
	}

	public FlareSolverrRequestProxy Proxy { get; }

	public bool AllowAnyStatusCode { get; set; }

	public FlareSolverr(string flareSolverrApiUrl)
	{
		if (String.IsNullOrWhiteSpace(flareSolverrApiUrl)
		    || !Uri.IsWellFormedUriString(flareSolverrApiUrl, UriKind.Absolute)) {
			throw new FlareSolverrException($"FlareSolverr URL is malformed: {flareSolverrApiUrl}");
		}

		var apiUrl = flareSolverrApiUrl;

		if (!apiUrl.EndsWith("/")) {
			apiUrl += "/";
		}

		FlareSolverrApi = new Uri($"{apiUrl}v1");

		m_httpClient = new HttpClient()
		{
			// Timeout = AdjustHttpClientTimeout()
		};

		MaxTimeout = FlareSolverrValues.MAX_TIMEOUT_DEFAULT;
		Proxy      = new FlareSolverrRequestProxy();

		/*PropertyChanged += (sender, args) =>
		{
			m_httpClient.Timeout = AdjustHttpClientTimeout();
		};*/
	}

	public Task<FlareSolverrResponse> SolveAsync(HttpRequestMessage request, string sessionId = null,
	                                             FlareSolverrCookie[] cookies = null)
	{
		var content = GenerateFlareSolverrRequest(request, sessionId, cookies);
		return SendFlareSolverrRequestAsync(content);
	}

	public Task<FlareSolverrResponse> CreateSessionAsync()
	{
		var req = new FlareSolverrRequestGet
		{
			Command    = FlareSolverrValues.CMD_SESSIONS_CREATE,
			MaxTimeout = MaxTimeout,
			Proxy      = Proxy
		};
		return SendFlareSolverrRequestAsync(GetSolverRequestContent(req));
	}

	public Task<FlareSolverrResponse> ListSessionsAsync()
	{
		var req = new FlareSolverrRequestGet
		{
			Command    = FlareSolverrValues.CMD_SESSIONS_LIST,
			MaxTimeout = MaxTimeout,
			Proxy      = Proxy
		};
		return SendFlareSolverrRequestAsync(GetSolverRequestContent(req));
	}

	public Task<FlareSolverrResponse> DestroySessionAsync(string sessionId)
	{
		var req = new FlareSolverrRequestGet
		{
			Command    = FlareSolverrValues.CMD_SESSIONS_DESTROY,
			MaxTimeout = MaxTimeout,
			Proxy      = Proxy,
			Session    = sessionId
		};
		return SendFlareSolverrRequestAsync(GetSolverRequestContent(req));
	}

	/*public Task<FlareSolverrResponse> SendFlareSolverrRequestAsyncFunctor(
		Func<FlareSolverrRequest, HttpContent> f, FlareSolverrRequest r)
	{
		return SendFlareSolverrRequestAsync(f(r));
	}*/

	// https://github.com/FlareSolverr/FlareSolverrSharp/pull/26

	public Task<FlareSolverrIndexResponse> GetIndexAsync()
	{
		return SendFlareSolverrRequestInternalAsync<FlareSolverrIndexResponse>(
			null, FlareSolverrContext.Default.FlareSolverrIndexResponse);
	}

	private async Task<FlareSolverrResponse> SendFlareSolverrRequestAsync(HttpContent flareSolverrRequest)
	{
		FlareSolverrResponse result =
			await SendFlareSolverrRequestInternalAsync<FlareSolverrResponse>(
				flareSolverrRequest, FlareSolverrContext.Default.FlareSolverrResponse);

		try {
			Enum.TryParse(result.Status, true, out FlareSolverrStatusCode returnStatusCode);

			if (returnStatusCode == FlareSolverrStatusCode.ok) {
				return result;

			}
			else {
				string errMsg = returnStatusCode switch
				{
					FlareSolverrStatusCode.warning =>
						$"FlareSolverr was able to process the request, but a captcha was detected. Message: {result.Message}",
					FlareSolverrStatusCode.error =>
						$"FlareSolverr was unable to process the request, please check FlareSolverr logs. Message: {result.Message}",
					_ =>
						$"Unable to map FlareSolverr returned status code, received code: {result.Status}. Message: {result.Message}"
				};
				throw new FlareSolverrException(errMsg);

			}

		}
		catch (ArgumentException) {
			throw new FlareSolverrException(
				$"Error parsing status code, check FlareSolverr log. Status: {result.Status}. Message: {result.Message}");
		}
	}

	private async Task<T> SendFlareSolverrRequestInternalAsync<T>(HttpContent flareSolverrRequest,
	                                                              JsonTypeInfo<T> typeInfo)
	{
		T result = default;

		//https://github.com/FlareSolverr/FlareSolverrSharp/pull/27/files

		//todo: what is this "semaphore locker" for
		await s_locker.LockAsync(async () =>
		{
			HttpResponseMessage response;

			try {
				// m_httpClient = new HttpClient();

				// wait 5 more seconds to make sure we return the FlareSolverr timeout message
				// m_httpClient.Timeout = TimeSpan.FromMilliseconds(MaxTimeout + 5000);

				if (flareSolverrRequest == null) {
					response = await m_httpClient.GetAsync(_flareSolverrIndexUri);
				}
				else {
					response = await m_httpClient.PostAsync(FlareSolverrApi, flareSolverrRequest);

				}
			}
			catch (HttpRequestException e) {
				throw new FlareSolverrException($"Error connecting to FlareSolverr server: {e}");
			}
			catch (Exception e) {
				throw new FlareSolverrException($"Exception: {e}");
			}
			finally {
				// m_httpClient.Dispose();
			}

			// Don't try parsing if FlareSolverr hasn't returned 200 or 500
			if (!AllowAnyStatusCode
			    && (response.StatusCode is not (HttpStatusCode.OK or HttpStatusCode.InternalServerError))) {
				throw new FlareSolverrException($"Status code: {response.StatusCode}");
			}

			var resContent = await response.Content.ReadAsStringAsync();

			try {
				// var options = JsonSerializerOptions1;

				// result  = await JsonSerializer.DeserializeAsync<FlareSolverrResponse>(resContent, options);

				result = JsonSerializer.Deserialize<T>(resContent, typeInfo);
			}
			catch (Exception) {
				throw new FlareSolverrException($"Error parsing response, check FlareSolverr. Response: {resContent}");
			}

			/*if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.InternalServerError) {
			throw new FlareSolverrException($"HTTP StatusCode not 200 or 500. Status is :{response.StatusCode}");
			}



			/*try {
				Enum.TryParse(result.Status, true, out FlareSolverrStatusCode returnStatusCode);

				if (returnStatusCode == FlareSolverrStatusCode.ok) {
					return result;

				}
				else {
					string errMsg = returnStatusCode switch
					{
						FlareSolverrStatusCode.warning =>
							$"FlareSolverr was able to process the request, but a captcha was detected. Message: {result.Message}",
						FlareSolverrStatusCode.error =>
							$"FlareSolverr was unable to process the request, please check FlareSolverr logs. Message: {result.Message}",
						_ =>
							$"Unable to map FlareSolverr returned status code, received code: {result.Status}. Message: {result.Message}"
					};
					throw new FlareSolverrException(errMsg);

				}

			}
			catch (ArgumentException) {
				throw new FlareSolverrException(
					$"Error parsing status code, check FlareSolverr log. Status: {result.Status}. Message: {result.Message}");
			}

			// return SendRequestAsync(flareSolverrRequest);*/


		});
		return result;


	}


	/*private FlareSolverrRequestProxy GetProxy()
	{
		FlareSolverrRequestProxy proxy = null;

		if (!string.IsNullOrWhiteSpace(ProxyUrl)) {
			proxy = new FlareSolverrRequestProxy
			{
				Url = ProxyUrl,
			};

			if (!string.IsNullOrWhiteSpace(ProxyUsername)) {
				proxy.Username = ProxyUsername;
			}

			if (!string.IsNullOrWhiteSpace(ProxyPassword)) {
				proxy.Password = ProxyPassword;
			}

		}

		return proxy;
	}*/

	private HttpContent GetSolverRequestContent(FlareSolverrRequest request)
	{
		var payload = JsonSerializer.Serialize(request, FlareSolverrContext.Default.FlareSolverrRequest);

		HttpContent content = new StringContent(payload, Encoding.UTF8, MediaTypeNames.Application.Json);
		return content;

		// return payload;
	}

	private HttpContent GenerateFlareSolverrRequest(HttpRequestMessage request, string sessionId = null,
	                                                FlareSolverrCookie[] cookies = null)
	{
		FlareSolverrRequest req;


		var url = request.RequestUri.ToString();


		if (request.Method == HttpMethod.Get) {
			req = new FlareSolverrRequestGet
			{
				Command    = FlareSolverrValues.CMD_REQUEST_GET,
				Url        = url,
				MaxTimeout = MaxTimeout,
				Proxy      = Proxy,
				Session    = sessionId,
				Cookies    = cookies,
			};
		}
		/*else if (request.Method == HttpMethod.Post) {
			// request.Content.GetType() doesn't work well when encoding != utf-8
			var contentMediaType = request.Content.Headers.ContentType?.MediaType.ToLower() ?? "<null>";

			if (contentMediaType.Contains("application/x-www-form-urlencoded")) {
				req = new FlareSolverrRequestPost
				{
					Command        = FlareSolverrValues.CMD_REQUEST_POST,
					Url        = url,
					PostData   = request.Content.ReadAsStringAsync().Result,
					MaxTimeout = MaxTimeout,
					Proxy      = Proxy,
					Session    = sessionId
				};
			}
			else if (contentMediaType.Contains("multipart/form-data")
			         || contentMediaType.Contains("text/html")) {
				//TODO Implement - check if we just need to pass the content-type with the relevant headers
				throw new FlareSolverrException("Unimplemented POST Content-Type: " + contentMediaType);
			}
			else {
				throw new FlareSolverrException("Unsupported POST Content-Type: " + contentMediaType);
			}
		}
		else {
			throw new FlareSolverrException("Unsupported HttpMethod: " + request.Method);
		}*/

		else if (request.Method == HttpMethod.Post) {
			// request.Content.GetType() doesn't work well when encoding != utf-8
			var contentType = request.Content.Headers.ContentType;

			// var contentMediaType = contentType?.MediaType.ToLower() ?? "<null>";

			switch (contentType.MediaType) {
				case Application.FormUrlEncoded:
					req = new FlareSolverrRequestPost
					{
						Command    = FlareSolverrValues.CMD_REQUEST_POST,
						Url        = url,
						PostData   = request.Content.ReadAsStringAsync().Result,
						MaxTimeout = MaxTimeout,
						Proxy      = Proxy,
						Session    = sessionId,
						Cookies    = cookies
					};
					break;

				case Multipart.FormData or Text.Html:
					//TODO Implement - check if we just need to pass the content-type with the relevant headers
					// throw new FlareSolverrException($"Unimplemented POST Content-Type: {contentMediaType}");
					throw new NotImplementedException($"{contentType.MediaType} POST Content-Type");

					break;

				default:
					throw new NotSupportedException($"{contentType.MediaType} POST Content-Type");

			}

		}
		else {
			throw new NotSupportedException($"Unsupported method: {request.Method}");
		}

		return GetSolverRequestContent(req);
	}


	public event PropertyChangedEventHandler PropertyChanged;

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value)) return false;

		field = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	private TimeSpan AdjustHttpClientTimeout(int delta = 5000)
	{
		return TimeSpan.FromMilliseconds(MaxTimeout + delta);
	}

}