using System;
using Xunit;

namespace Victor.Tests
{
    public class BaseTests
    {
        public BaseTests()
        {
            Api.SetDefaultLoggerIfNone();
        }

        [Fact]
        public void CanConvertToInteger()
        {
            Assert.Equal(33, "thirty-three".ToInteger());

        }
    }
}
