using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FaceMaskVideoDetector.Helpers
{
    public class HttpClientHelper
    {
        public static HttpClient CreateHttpClient(string url, string headerKey, string key)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrWhiteSpace(headerKey))
                client.DefaultRequestHeaders.Add(headerKey, key);

            return client;
        }
    }
}
