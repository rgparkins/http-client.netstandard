using System.Net.Http;
using rgparkins.PrometheusMetrics.Netstandard;

namespace rgparkins.HttpClient.Netstandard.Core
{
    public class HttpClientFactory
    {
        private readonly MetricFactory _factory;

        public HttpClientFactory(MetricFactory factory)
        {
            _factory = factory;
        }

        public System.Net.Http.HttpClient CreateClient(string targetService)
        {
            return new System.Net.Http.HttpClient(new MetricHttpClientHandler(targetService, _factory));
        }

        public System.Net.Http.HttpClient CreateClient(string targetService, DelegatingHandler innerHandler)
        {
            innerHandler.InnerHandler = new MetricHttpClientHandler(targetService, _factory);
            
            return new System.Net.Http.HttpClient(innerHandler);
        }
    }
}