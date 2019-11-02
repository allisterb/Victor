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

    [Verb("sr", HelpText = "Use the default speech recognition feature of Victor with the default mic as the input source.")]
    class SpeechRecognitionOptions : Options
    {

    }

    [Verb("nlu", HelpText = "Use the NLU feature of Victor.")]
    class NLUOptions : Options
    {
        [Option('m', "model", Required = true, HelpText = "The NLU model to use.")]
        public string Model { get; set; }

        [Option('t', "text", Required = true, HelpText = "The text to understand.")]
        public string Text { get; set; }
    }

    [Verb("tts", HelpText = "Use the TTS feature of Victor.")]
    class TTSOptions : Options
    {
        [Option('t', "text", Required = true, HelpText = "The text to synthesize speech for.")]
        public string Text { get; set; }
    }

    [Verb("cui", HelpText = "Use the CUI features of Victor.")]
    class CUIOptions : Options
    {
        [Option('v', "version", Required = false, Default = 1, HelpText = "Set the version of the object.")]
        public int Version { get; set; }

        [Option("list-bots", Required = false, HelpText = "List the current chatbots on the EDDI server.")]
        public bool ListBots { get; set; }

        [Option('e', "export-bot", Required = false, HelpText = "Export a chatbot on the EDDI server.")]
        public string ExportBot { get; set; }

        [Option("list-packages", Required = false, HelpText = "List the current packages on the EDDI server.")]
        public bool ListPackages { get; set; }

        [Option("get-package", Required = false, HelpText = "Get the package with the specified id")]
        public string GetPackage { get; set; }
    }
}
