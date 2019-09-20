using System;
using Xunit;

namespace Victor.Tests
{
    public class MimicTests : BaseTests
    {
        [Fact]
        public void CanRunMimcSession()
        {
            var m = new MimicSession("Hello world");
            m.Run();
        }
    }
}
