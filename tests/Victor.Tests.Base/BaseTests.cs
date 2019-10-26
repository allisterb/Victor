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
    }
}
