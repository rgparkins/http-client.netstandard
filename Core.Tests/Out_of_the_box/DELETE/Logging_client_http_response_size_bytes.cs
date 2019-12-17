using System.Linq;
using NUnit.Framework;
using rgparkins.PrometheusMetrics.Netstandard;

namespace Core.Tests.Out_of_the_box.DELETE
{
    public class Logging_client_http_response_size_bytes : Context
    {
        public Logging_client_http_response_size_bytes()
        {
            Given_a_client_factory();
            
            Given_a_stub_service_with_delete_endpoint("/ping", "ok");

            When_creating_a_client("targetA");
            
            When_making_a_delete_request(new
            {
                testintegerExample = 1,
                stringExample = "string"
            }, "/ping");
        }

        [Test]
        public void Metric_is_logged()
        {
            var responseSizeMetric = MyMetricStore.Metrics.Single(s => s.Data.Name == "client_http_response_size_bytes").Data as Summary;

            Assert.That(responseSizeMetric.Value, Is.EqualTo(2));
            
            var labels = responseSizeMetric.Labels;
            
            Assert.That(labels["http_verb"], Is.EqualTo("DELETE"));
            Assert.That(labels["http_status_code"], Is.EqualTo(200));
            Assert.That(labels["path_bucket"], Is.EqualTo("/ping"));
            Assert.That(labels["target_service"], Is.EqualTo("targetA"));
        }
    }
}