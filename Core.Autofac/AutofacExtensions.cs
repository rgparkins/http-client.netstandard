using Autofac;
using rgparkins.PrometheusMetrics.Netstandard;

namespace rgparkins.HttpClient.Netstandard.Core.Autofac
{
    public static class AutofacExtensions 
    {
        public static void RegisterHttpClient(this ContainerBuilder bldr, string targetService)
        {
            bldr.Register(c => new HttpClientFactory(c.Resolve<MetricFactory>())
                .CreateClient(targetService))
                .Named<System.Net.Http.HttpClient>(targetService)
                .As<System.Net.Http.HttpClient>()
                .SingleInstance();
        }
    }
}