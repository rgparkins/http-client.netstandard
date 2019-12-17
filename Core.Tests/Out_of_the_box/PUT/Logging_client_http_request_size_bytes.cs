using System.Linq;
using NUnit.Framework;
using rgparkins.PrometheusMetrics.Netstandard;

namespace Core.Tests.Out_of_the_box.PUT
{
    public class Logging_client_http_request_size_bytes : Context
    {
        public Logging_client_http_request_size_bytes()
        {
            Given_a_client_factory();
            
            Given_a_stub_service_with_put_endpoint("/ping", "ok");

            When_creating_a_client("targetA");
            
            When_making_a_put_request(new
            {
                testintegerExample = 1,
                stringExample = "string"
            }, "/ping");
        }

        [Test]
        public void Metric_is_logged()
        {
            var requestSizemetric = MyMetricStore.Metrics.Single(s => s.Data.Name == "client_http_request_size_bytes").Data as Summary;

            Assert.That(requestSizemetric.Value, Is.EqualTo(49));
            
            var labels = requestSizemetric.Labels;
            
            Assert.That(labels["http_verb"], Is.EqualTo("PUT"));
            Assert.That(labels["http_status_code"], Is.EqualTo(200));
            Assert.That(labels["path_bucket"], Is.EqualTo("/ping"));
            Assert.That(labels["target_service"], Is.EqualTo("targetA"));
        }
    }
}