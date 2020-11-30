using System.Net.Http;
using System.Threading.Tasks;
namespace Victor.CUI.PM
{
    class MessageHandler1 : DelegatingHandler
    {
        private int _count = 0;
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            System.Threading.Interlocked.Increment(ref _count);
            request.Headers.Add("X-Custom-Header", _count.ToString());
            var r = await base.SendAsync(request, cancellationToken);
            var s = r.Content.ReadAsStringAsync();
            return r;
        }
    }
}