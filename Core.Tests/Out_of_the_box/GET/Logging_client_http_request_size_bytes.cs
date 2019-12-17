using System.Linq;
using NUnit.Framework;
using rgparkins.PrometheusMetrics.Netstandard;

namespace Core.Tests.Out_of_the_box.GET
{
    public class Logging_client_http_request_size_bytes : Context
    {
        public Logging_client_http_request_size_bytes()
        {
            Given_a_client_factory();
            
            Given_a_stub_service_with_get_endpoint("/ping", "pong");

            When_creating_a_client("targetA");
            
            When_making_a_get_request("/ping");
        }

        [Test]
        public void Metric_is_logged()
        {
            var timeTakenMetric = MyMetricStore.Metrics.Single(s => s.Data.Name == "client_http_request_size_bytes").Data as Summary;

            Assert.That(timeTakenMetric.Value, Is.EqualTo(0));
            
            var labels = timeTakenMetric.Labels;
            
            Assert.That(labels["http_verb"], Is.EqualTo("GET"));
            Assert.That(labels["http_status_code"], Is.EqualTo(200));
            Assert.That(labels["path_bucket"], Is.EqualTo("/ping"));
            Assert.That(labels["target_service"], Is.EqualTo("targetA"));
        }
    }
}