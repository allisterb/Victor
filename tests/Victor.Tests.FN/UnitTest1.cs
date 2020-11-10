using System;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Globalization;

using Xunit;

namespace Victor.Tests
{
    public class MSSpeechTests
    {
      

        [Fact]
        public void Test1()
        {
            SpeechSynthesizer ss = new SpeechSynthesizer();
            ss.SetOutputToDefaultAudioDevice();
            ss.Speak("I am awake");
            Assert.NotNull(ss);
            //Assert.Equal(3, 3);
        }
    }
}
