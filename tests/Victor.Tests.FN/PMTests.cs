using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;

using Victor.CUI.PM;

namespace Victor.Tests
{
    public class PMTests
    {
        [Fact]
        public void CanGetBoards()
        {
            var boards = MdcApi.GetBoards().Result;
            Assert.NotNull(boards.Data);
        }
    }
}
