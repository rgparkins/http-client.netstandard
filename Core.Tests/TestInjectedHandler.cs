using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Tests
{
    public class TestInjectedHandler : DelegatingHandler
    {
        public List<string> Requests = new List<string>();
        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = base.SendAsync(request, cancellationToken);

            Requests.Add(request.RequestUri.PathAndQuery);

            return result;
        }
    }
}