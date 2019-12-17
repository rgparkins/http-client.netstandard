using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using rgparkins.PrometheusMetrics.Netstandard;

namespace rgparkins.HttpClient.Netstandard.Core
{
    internal class MetricHttpClientHandler : HttpClientHandler
    {
        private readonly string _targetService;
        private readonly MetricFactory _factory;

        public MetricHttpClientHandler(string targetService, MetricFactory factory)
        {
            _targetService = targetService;
            _factory = factory;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var watch = new Stopwatch();
            watch.Start();
            
            var result = await base.SendAsync(request, cancellationToken);

            var latencyMetric = _factory.CreateSummary("client_http_response_time_milliseconds");
            var responseSizeMetric = _factory.CreateSummary("client_http_response_size_bytes");
            var requestSizeMetric = _factory.CreateSummary("client_http_request_size_bytes");

            latencyMetric.Log(new
                {
                    http_verb = request.Method.ToString(),
                    http_status_code = (int)result.StatusCode,
                    path_bucket = request.RequestUri.PathAndQuery,
                    target_service = _targetService
                },
                watch.ElapsedMilliseconds);


            responseSizeMetric.Log(new
                {
                    http_verb = request.Method.ToString(),
                    http_status_code = (int) result.StatusCode,
                    path_bucket = request.RequestUri.PathAndQuery,
                    target_service = _targetService
                },
                (await result.Content.ReadAsStringAsync()).Length);

            requestSizeMetric.Log(new
                {
                    http_verb = request.Method.ToString(),
                    http_status_code = (int) result.StatusCode,
                    path_bucket = request.RequestUri.PathAndQuery,
                    target_service = _targetService
                },
                request.Content== null? 0: (await request.Content.ReadAsStringAsync()).Length);

            return result;
        }
    }
}