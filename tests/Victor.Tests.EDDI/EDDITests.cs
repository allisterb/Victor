using System.Linq;
using System.Net.Http;
using System.Threading;
using Xunit;

namespace Victor.Tests
{
    public class EDDITests : BaseTests
    {
        static HttpClient HttpClient = new HttpClient();
        static CancellationTokenSource CTS = new CancellationTokenSource();
        protected EDDIClient EDDIClient = new EDDIClient(Api.Config("CUI:EDDIServerUrl"), HttpClient);
        public EDDITests() : base() { }

        [Fact]
        public void CanGetBotDescriptors()
        {
            var bots = EDDIClient.BotstoreBotsDescriptorsGetAsync(null, null, null).Result;
            Assert.NotEmpty(bots);
        }

        [Fact]
        public void CanStartBotConversation()
        {
            var bot = EDDIClient.BotstoreBotsDescriptorsGetAsync(null, null, null).Result.First();
            Assert.NotNull(bot);
            var cid = EDDIClient.BotsPostAsync(Environment8.Test, bot.ResourceId, null, null).Result;
            Assert.NotNull(cid);
            var c = EDDIClient.ConversationstoreConversationsGetAsync(cid).Result;
            Assert.NotNull(c);
            EDDIClient.ConversationstoreConversationsDeleteAsync(cid, true).Wait();
          
        }

        [Fact]
        public void CanGetConversations()
        {
            var c = EDDIClient.ConversationstoreConversationsGetAsync("5db2455dfc051079c87b93f8").Result;
            Assert.NotNull(c);
        }
    }
}
