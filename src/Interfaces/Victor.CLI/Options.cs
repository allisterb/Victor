using System;
using System.Collections.Generic;
using System.Text;

using CommandLine;
using CommandLine.Text;

namespace Victor
{
    public class Options
    {
        [Option('d', "debug", Required = false, HelpText = "Enable debug mode.")]
        public bool Debug { get; set; }
    }

    [Verb("asr", HelpText = "Test the speech recognition feature of Victor with the default mic as the input source.")]
    class SpeechRecognitionOptions : Options
    {
        [Option('t', "text", Required = true, HelpText = "The text to understand.")]
        public string Text { get; set; }
    }

    [Verb("nlu", HelpText = "Test the NLU feature of Victor.")]
    class NLUOptions : Options
    {
        [Option('m', "model", Required = true, HelpText = "The NLU model to use.")]
        public string Model { get; set; }

        [Option('t', "text", Required = true, HelpText = "The text to understand.")]
        public string Text { get; set; }
    }

    [Verb("tts", HelpText = "Test the TTS feature of Victor.")]
    class TTSOptions : Options
    {
        [Option('t', "text", Required = true, HelpText = "The text to synthesize speech for.")]
        public string Text { get; set; }
    }

    [Verb("fn", HelpText = "Launch the Victor FN auditory CUI.")]
    public class FNOptions : Options
    {
        [Option('b', "no-beeper", Required = false, Default = false, HelpText = "Disable the beeper sound.")]
        public bool NoBeeper { get; set; }
    }

    [Verb("pm", HelpText = "Launch the Victor PM auditory CUI.")]
    public class PMOptions : Options
    {
        [Option('b', "no-beeper", Required = false, Default = false, HelpText = "Disable the beeper sound.")]
        public bool NoBeeper { get; set; }
    }

    [Verb("du", HelpText = "Launch the Victor DU auditory CUI.")]
    public class DUOptions : Options
    {
        [Option('b', "no-beeper", Required = false, Default = false, HelpText = "Disable the beeper sound.")]
        public bool NoBeeper { get; set; }
    }
}
