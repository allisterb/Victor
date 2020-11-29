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

        [Fact]
        public void CanRecognize()
        {
            var sre = new SpeechRecognitionEngine();
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += Sre_SpeechRecognized; 
            Choices ch_StartStopCommands = new Choices();
            ch_StartStopCommands.Add("speech on");
            ch_StartStopCommands.Add("speech off");
            GrammarBuilder gb_StartStop = new GrammarBuilder();
            gb_StartStop.Append(ch_StartStopCommands);
            Grammar g_StartStop = new Grammar(gb_StartStop);
            sre.LoadGrammarAsync(g_StartStop);
            sre.RecognizeAsync(RecognizeMode.Multiple);
            var done = false;
            while (done == false) {; }
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //throw new NotImplementedException();
            string txt = e.Result.Text;
            float confidence = e.Result.Confidence;
        }
    }
}
