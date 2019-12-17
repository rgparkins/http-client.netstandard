using System.Linq;
using NUnit.Framework;

namespace rgparkins.HttpClient.Netstandard.Core.Autofac.Tests
{
    public class Resolving_a_named_client : Context
    {
        public Resolving_a_named_client()
        {
            Given_a_named_client_registered("quoting.enquiry-orchestrator");
            
            Given_a_named_client_registered("panel.teletraan");
            
            Given_a_stub_service_with_get_endpoint("/ping", "pong");
            
            When_container_is_built();
            
            When_making_a_get_request_with_named_client("quoting.enquiry-orchestrator", "/ping");
            
            When_making_a_get_request_with_named_client("panel.teletraan", "/ping");
        }

        [Test]
        public void Metrics_logged_with_correct_target()
        {
            Assert.That(MyMetricStore.Metrics .Count(s => s.Data.Labels.ContainsValue("quoting.enquiry-orchestrator")), Is.EqualTo(3));
            
            var timeTakenMetric = MyMetricStore.Metrics
                .Where(s => s.Data.Name == "client_http_response_time_milliseconds" &&
                             s.Data.Labels.ContainsValue("quoting.enquiry-orchestrator"));

            Assert.That(timeTakenMetric.Count(), Is.EqualTo(1));
        }
        
        [Test]
        public void Metrics_logged_with_correct_target_for_second_registered_service()
        {
            Assert.That(MyMetricStore.Metrics .Count(s => s.Data.Labels.ContainsValue("panel.teletraan")), Is.EqualTo(3));
            
            var timeTakenMetric = MyMetricStore.Metrics
                .Where(s => s.Data.Name == "client_http_response_time_milliseconds" &&
                            s.Data.Labels.ContainsValue("panel.teletraan"));

            Assert.That(timeTakenMetric.Count(), Is.EqualTo(1));
        }
    }
}