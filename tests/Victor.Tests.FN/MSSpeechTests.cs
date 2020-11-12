using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;

namespace Victor.Tests
{
    public class MSSpeechTests
    {
        [Fact]
        public void CanSpeak()
        {
            SpeechSynthesizer ss = new SpeechSynthesizer();
            ss.SetOutputToDefaultAudioDevice();
            ss.Speak("I am awake");
            Assert.NotNull(ss);
            ss.Dispose();
        }
    }
}
