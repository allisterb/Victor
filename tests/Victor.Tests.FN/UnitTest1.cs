using System;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Globalization;

using Xunit;

using Victor.Vision;
namespace Victor.Tests
{
    public class MSSpeechTests
    {
      
        [Fact]
        public void CanCapture()
        {
            CV.CaptureCamera();
            Assert.True(true);
        }
        
        [Fact]
        public void CanSpeak()
        {
            SpeechSynthesizer ss = new SpeechSynthesizer();
            ss.SetOutputToDefaultAudioDevice();
            ss.Speak("I am awake");
            Assert.NotNull(ss);
        }
    }
}
