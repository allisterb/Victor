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
        [Option('v', "version", Required = false, Default = 1, HelpText = "Set the version of the object to be retrieved.")]
        public int Version { get; set; }

        [Option('f', "filter", Required = false, Default = null, HelpText = "Filter the current query on this value.")]
        public string Filter { get; set; }

        [Option("get-bots", Required = false, HelpText = "List the current chatbots on the EDDI server.")]
        public bool ListBots { get; set; }

        [Option("export-bot", Required = false, HelpText = "Export a chatbot on the EDDI server.")]
        public string ExportBot { get; set; }

        [Option("get-packages", Required = false, HelpText = "List the current packages on the EDDI server.")]
        public bool ListPackages { get; set; }

        [Option("get-package", Required = false, HelpText = "Get the package with the specified id")]
        public string GetPackage { get; set; }

        [Option("get-dictionary", Required = false, HelpText = "Get the dictionary with the specified id")]
        public string GetDictionary { get; set; }

        [Option("get-behavior", Required = false, HelpText = "Get the behavior set with the specified id")]
        public string GetBehavior { get; set; }
    }
}
