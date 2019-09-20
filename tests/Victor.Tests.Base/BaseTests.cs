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
        public void CanRunProcess()
        {
            var p = new ConsoleProcess("cmd", new[] { "/c", "dir" });
            p.Start();

        }
    }
}
