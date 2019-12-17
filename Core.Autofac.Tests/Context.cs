using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using rgparkins.PrometheusMetrics.Netstandard.Serialisation;
using HttpMock.Net;
using NUnit.Framework;
using rgparkins.PrometheusMetrics.Netstandard;
using rgparkins.PrometheusMetrics.Netstandard.Autofac;

namespace rgparkins.HttpClient.Netstandard.Core.Autofac.Tests
{
    public class Context
    {
        protected IContainer Container;
        protected ContainerBuilder bldr = new ContainerBuilder();
        protected HttpHandlerBuilder MyMockServer;
        
        const int PORT = 5001;
        
        protected InMemoryMetricStore MyMetricStore => Container.Resolve<IMetricStore>() as InMemoryMetricStore;

        public Context()
        {
            MyMockServer = Server.Start(PORT);
        }
        
        protected void Given_a_named_client_registered(string targetSource)
        {
            
            bldr.RegisterHttpClient(targetSource);
        }
        
        protected void When_container_is_built()
        {
            bldr.RegisterMetrics(new InMemoryMetricStore());
            
            Container = bldr.Build();
        }
        
        protected void Given_a_stub_service_with_get_endpoint(string path, string response)
        {
            MyMockServer.WhenGet(path).Respond(response);
        }

        protected void When_making_a_get_request_with_named_client(string namedClient, string path)
        {
            var client = Container.ResolveNamed<System.Net.Http.HttpClient>(namedClient);
            
            client.GetAsync("http://localhost:" + PORT + path).Wait();
        }
        
        public class InMemoryMetricStore : IMetricStore
        {
            public List<Metric> Metrics = new List<Metric>();

            public void Init()
            {
                
            }

            public Task Log(Metric metric)
            {
                Metrics.Add(metric);
                return Task.CompletedTask;
            }
        }
        
        [TearDown]
        public void Dispose()
        {
            Console.WriteLine("Disposing");
            
            MyMockServer?.Dispose();
        }
    }
}