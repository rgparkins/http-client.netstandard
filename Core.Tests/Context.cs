using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttpMock.Net;
using Newtonsoft.Json;
using NUnit.Framework;
using rgparkins.PrometheusMetrics.Netstandard;
using rgparkins.PrometheusMetrics.Netstandard.Serialisation; 
using rgparkins.HttpClient.Netstandard.Core;

namespace Core.Tests
{
    public class Context
    {
        protected HttpClientFactory MyFactory;
        protected HttpClient MyClient;
        protected InMemoryMetricStore MyMetricStore;
        protected TestInjectedHandler MyInjectedHandler;
        protected HttpHandlerBuilder MyMockServer;
        
        const int PORT = 5001;

        public Context()
        {
            MyMockServer = Server.Start(PORT);
        }

        protected void Given_a_client_factory()
        {
            MyMetricStore = new InMemoryMetricStore();
            MyFactory = new HttpClientFactory(new MetricFactory(MyMetricStore));
        }
        

        protected void When_creating_a_client(string targetServiceName)
        {
            MyClient = MyFactory.CreateClient(targetServiceName);
        }
        
        protected void When_creating_a_client_with_handler(string targetServiceName)
        {
            MyInjectedHandler = new TestInjectedHandler();
            
            MyClient = MyFactory.CreateClient(targetServiceName, MyInjectedHandler);
        }
        
        protected void When_making_a_get_request(string path)
        {
            MyClient.GetAsync("http://localhost:" + PORT + path).Wait();
        }

        protected void When_making_a_post_request<T>(T body, string path)
        {
            var json = JsonConvert.SerializeObject(body);

            MyClient.PostAsync("http://localhost:" + PORT + path,
                new StringContent(json, Encoding.UTF8, "application/json")).Wait();
        }
        
        protected void When_making_a_put_request<T>(T body, string path)
        {
            var json = JsonConvert.SerializeObject(body);

            MyClient.PutAsync("http://localhost:" + PORT + path,
                new StringContent(json, Encoding.UTF8, "application/json")).Wait();
        }

        protected void When_making_a_delete_request<T>(T body, string path)
        {
            var json = JsonConvert.SerializeObject(body);
            
            MyClient.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "http://localhost:" + PORT + path)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                }).Wait();
        }
       
        protected void Given_a_stub_service_with_get_endpoint(string path, string response)
        {
            MyMockServer.WhenGet(path).Respond(response);
        }
        
        protected void Given_a_stub_service_with_post_endpoint(string path, string response)
        {
            MyMockServer.WhenPost(path, s => true).Respond(response);
        }
        
        protected void Given_a_stub_service_with_put_endpoint(string path, string response)
        {
            MyMockServer.When(context => context.Request.Method.Equals("PUT") && context.MatchPath(path)).Respond(response);
        }
        
        protected void Given_a_stub_service_with_delete_endpoint(string path, string response)
        {
            MyMockServer.When(context => context.Request.Method.Equals("DELETE") && context.MatchPath(path)).Respond(response);
        }

        [TearDown]
        public void Dispose()
        {
            MyClient?.Dispose();

            Console.WriteLine("Disposing");
            
            MyMockServer?.Dispose();
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


    }
}