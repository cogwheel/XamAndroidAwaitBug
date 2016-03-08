//#define FAIL_TEST

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace AwaitTest
{
	public class MyClient
	{
		public MyClient()
		{
		}

		public struct PageResult
		{
			public HttpStatusCode status;
			public Uri uri;
			public string content;
		}

		public async Task<HttpWebResponse> GetResponse(HttpWebRequest request)
		{
			//request.CookieContainer = cookies.GetContainer();
			var response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false);
			//foreach (Cookie cookie in response.Cookies)
			//{
			//	cookies.SetCookie(cookie);
			//}
			return response;
		}

		private HttpWebRequest CloneRequest(HttpWebRequest orig, Uri uri)
		{
			var clone = new HttpWebRequest(uri)
			{
				Referer = orig.RequestUri.ToString(),
				Accept = orig.Accept,
				AutomaticDecompression = orig.AutomaticDecompression,
				UserAgent = orig.UserAgent,
				CookieContainer = orig.CookieContainer,
				Proxy = orig.Proxy,
				AllowAutoRedirect = false,
				MaximumAutomaticRedirections = orig.MaximumAutomaticRedirections
			};

			foreach (string key in orig.Headers.Keys)
			{
				if (!WebHeaderCollection.IsRestricted(key))
				{
					clone.Headers.Add(key, orig.Headers[key]);
				}
			}

			return clone;
		}

		private static bool IsRedirect(HttpStatusCode statusCode)
		{
			return (int)statusCode / 100 == 3;
		}

		private async Task<PageResult> GetPage(HttpWebRequest request)
		{
			request.AllowAutoRedirect = false;

			HttpWebResponse response = null;
			try
			{
				response = await GetResponse(request);
				int redirectCount = 0;
				while (IsRedirect(response.StatusCode))
				{
					if (++redirectCount > request.MaximumAutomaticRedirections)
					{
						throw new WebException("Too many automatic redirections.", WebExceptionStatus.UnknownError);
					}
					var location = response.Headers["Location"];

					request = CloneRequest(request, new Uri(location));
					response.Dispose();
					response = await GetResponse(request);
				}

				using (var stream = response.GetResponseStream())
				using (var reader = new StreamReader(stream))
				{
#if FAIL_TEST
					return new PageResult
					{
						status = response.StatusCode,
						uri = response.ResponseUri,
						content = await reader.ReadToEndAsync().ConfigureAwait(false)
					};
#else
					var result = new PageResult
					{
						status = response.StatusCode,
						uri = response.ResponseUri,
						content = await reader.ReadToEndAsync().ConfigureAwait(false)
					};
					return result;
#endif
				}
			}
			finally
			{
				response?.Dispose();
			}
		}

		public Task<PageResult> GetPage(Uri uri)
		{
			var request = new HttpWebRequest(uri)
			{
				//UserAgent = webUserAgent,
				AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
			};

			return GetPage(request);
		}

	}
}

