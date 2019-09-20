using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;
using CommandLine.Text;

namespace Victor
{
    class Options
    {
        [Option('d', "debug", Required = false, HelpText = "Enable debug mode.")]
        public bool Debug { get; set; }
    }

    [Verb("nlu", HelpText = "Use the default NLU feature of Victor with the default mic as the input source for speech recognition.")]
    class NLUOptions : Options
    {
        

    }

    [Verb("tts", HelpText = "Use the TTS feature of Victor.")]
    class TTSOptions : Options
    {
        public string Text { get; set; }

    }
}
