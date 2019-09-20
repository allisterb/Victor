using System;
using Xunit;

namespace Victor.Tests
{
    public class JuliusTests : BaseTests
    {
        [Fact]
        public void CanCreateJuliusProcess()
        {
            var s = new JuliusSession();
            Assert.True(s.Initialized);
            s.Start();
        }
    }
}
