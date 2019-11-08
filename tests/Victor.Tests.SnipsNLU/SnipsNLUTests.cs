using System;
using System.IO;
using Xunit;

namespace Victor.Tests
{
    public class SnipsNLUTests : BaseTests
    {
        [Fact]
        public void CanInitEngine()
        {
            var engine = new SnipsNLUEngine(Path.Combine("Engines", "beverage"));
            Assert.True(engine.Initialized);
        }

        [Fact]
        public void CanRecognizeTestIntent()
        {
            var engine = new SnipsNLUEngine(Path.Combine("Engines", "beverage"));
            engine.GetSnipsIntents("Make me two cups of coffee.", out string[] intents, out string json, out string error);
            Assert.NotEmpty(intents);

        }
    }
}
