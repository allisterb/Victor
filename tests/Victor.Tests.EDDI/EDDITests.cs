using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

using System.Threading;
using System.Threading.Tasks;

namespace Victor.Tests
{
    public class EDDITests : BaseTests
    {
        static HttpClient HttpClient = new HttpClient();
        static CancellationTokenSource CTS = new CancellationTokenSource();
        protected EDDIClient EDDIClient = new EDDIClient("http://eddi-evals25-shared-7daa.apps.hackathon.rhmi.io", HttpClient);
        public EDDITests() :base()
        {
            
        }

        [Fact]
        public void CanGetBotDescriptors()
        {
            var bots = EDDIClient.BotstoreBotsDescriptorsGetAsync(null, null, null).Result;
            Assert.NotEmpty(bots);
        }
    }
}
