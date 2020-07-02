using System.Linq;
using NUnit.Framework;

namespace rgparkins.HttpClient.Netstandard.Core.Autofac.Tests
{
    public class Resolving_a_named_client : Context
    {
        public Resolving_a_named_client()
        {
            Given_a_named_client_registered("clientA");
            
            Given_a_named_client_registered("serviceA");
            
            Given_a_stub_service_with_get_endpoint("/ping", "pong");
            
            When_container_is_built();
            
            When_making_a_get_request_with_named_client("clientA", "/ping");
            
            When_making_a_get_request_with_named_client("serviceA", "/ping");
        }

        [Test]
        public void Metrics_logged_with_correct_target()
        {
            Assert.That(MyMetricStore.Metrics .Count(s => s.Data.Labels.ContainsValue("clientA")), Is.EqualTo(3));
            
            var timeTakenMetric = MyMetricStore.Metrics
                .Where(s => s.Data.Name == "client_http_response_time_milliseconds" &&
                             s.Data.Labels.ContainsValue("clientA"));

            Assert.That(timeTakenMetric.Count(), Is.EqualTo(1));
        }
        
        [Test]
        public void Metrics_logged_with_correct_target_for_second_registered_service()
        {
            Assert.That(MyMetricStore.Metrics .Count(s => s.Data.Labels.ContainsValue("serviceA")), Is.EqualTo(3));
            
            var timeTakenMetric = MyMetricStore.Metrics
                .Where(s => s.Data.Name == "client_http_response_time_milliseconds" &&
                            s.Data.Labels.ContainsValue("serviceA"));

            Assert.That(timeTakenMetric.Count(), Is.EqualTo(1));
        }
    }
}