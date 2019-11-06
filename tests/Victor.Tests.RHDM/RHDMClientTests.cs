using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
namespace Victor.Tests
{
    [TestFixture]
    public class RHDMClientTests
    {
        public RHDMClient Client { get; protected set; }

        [SetUp]
        public void Init()
        {
            Client = new RHDMClient("https://victor-kieserver-evals25-shared-7daa.apps.hackathon.rhmi.io/");
        }

        [Test]
        public void InstanceTest()
        {
            //Client
        }
    }
}
