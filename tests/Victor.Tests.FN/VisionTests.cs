using System;
using System.Globalization;

using Xunit;

using Victor.Vision;

namespace Victor.Tests
{
    public class VisionTests
    {
        [Fact]
        public void CanCapture()
        {
            CV.CaptureCamera();
            Assert.True(true);
        }   
    }
}
