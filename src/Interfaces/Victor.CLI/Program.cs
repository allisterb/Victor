using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using CO = Colorful.Console;
using Figgle;
using CommandLine;
using CommandLine.Text;

using Victor.CLI;

namespace Victor
{
    class Program : Api
    {
        #region Entry-point
        static void Main(string[] args)
        {
            Args = args;
            if (Args.Contains("fn") || Args.Contains("pm") || Args.Contains("cr"))
            {
                SetLogger(new SerilogLogger(console: false, debug: true));
            }
            else if (Args.Contains("--debug"))
            {
                SetLogger(new SerilogLogger(console: true, debug: true));
            }
            else
            {
                SetLogger(new SerilogLogger(console: true, debug: false));
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Console.CancelKeyPress += Console_CancelKeyPress;
            if (!args.Contains("fn") && !args.Contains("pm") && !args.Contains("cr"))
            {
                PrintLogo();
            }
            if (args.Contains("--debug"))
            {
                Info("Debug mode set.");
            }
            ParserResult<object> result = new Parser().ParseArguments<Options, SpeechRecognitionOptions, TTSOptions, FNOptions, PMOptions>(args);
            result.WithNotParsed((IEnumerable<Error> errors) =>
            {
                HelpText help = GetAutoBuiltHelpText(result);
                help.Copyright = string.Empty;
                help.AddPreOptionsLine(string.Empty);

                if (errors.Any(e => e.Tag == ErrorType.VersionRequestedError))
                {
                    Exit(ExitResult.SUCCESS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.HelpVerbRequestedError))
                {
                    HelpVerbRequestedError error = (HelpVerbRequestedError)errors.First(e => e.Tag == ErrorType.HelpVerbRequestedError);
                    if (error.Type != null)
                    {
                        help.AddVerbs(error.Type);
                    }
                    else
                    {
                        help.AddVerbs(typeof(SpeechRecognitionOptions), typeof(TTSOptions), typeof(FNOptions), typeof(PMOptions));
                    }
                    Info(help);
                    Exit(ExitResult.SUCCESS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.HelpRequestedError))
                {
                    HelpRequestedError error = (HelpRequestedError)errors.First(e => e.Tag == ErrorType.HelpRequestedError);
                    help.AddVerbs(typeof(SpeechRecognitionOptions), typeof(TTSOptions), typeof(FNOptions), typeof(PMOptions));
                    Info(help);
                    Exit(ExitResult.SUCCESS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.NoVerbSelectedError))
                {
                    help.AddVerbs(typeof(SpeechRecognitionOptions), typeof(TTSOptions), typeof(FNOptions), typeof(PMOptions));
                    Info(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.MissingRequiredOptionError))
                {
                    MissingRequiredOptionError error = (MissingRequiredOptionError)errors.First(e => e.Tag == ErrorType.MissingRequiredOptionError);
                    Error("A required option is missing: {0}.", error.NameInfo.NameText);
                    Info(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.UnknownOptionError))
                {
                    UnknownOptionError error = (UnknownOptionError)errors.First(e => e.Tag == ErrorType.UnknownOptionError);
                    help.AddVerbs(typeof(SpeechRecognitionOptions), typeof(TTSOptions));
                    Error("Unknown option: {error}.", error.Token);
                    Info(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
                else
                {
                    Error("An error occurred parsing the program options: {errors}.", errors);
                    help.AddVerbs(typeof(SpeechRecognitionOptions), typeof(TTSOptions));
                    Info(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
            })
            .WithParsed<SpeechRecognitionOptions>(o =>
            {
                ASR();
                Exit(ExitResult.SUCCESS);
            })
            .WithParsed<TTSOptions>(o =>
            {
                TTS(o.Text);
                Exit(ExitResult.SUCCESS);
            })
            .WithParsed<FNOptions>(o =>
            {
                FNController.Options = o;
                new FNController(o).Start();
                Exit(ExitResult.SUCCESS);
            })
            .WithParsed<PMOptions>(o =>
            {
                PMController.Options = o;
                new PMController(o).Start();
                Exit(ExitResult.SUCCESS);
            });

        }
        #endregion

        #region Methods
        static void ASR()
        {
            #if UNIX
            JuliusSession s = new JuliusSession();
            if (!s.Initialized)
            {
                Error("Could not initialize Julius session.");
                Exit(ExitResult.UNKNOWN_ERROR);
            }
            SnipsNLUEngine engine = new SnipsNLUEngine(Path.Combine("Engines", "beverage"));
            if (!engine.Initialized)
            {
                Error("Could not initialize SnipsNLU engine.");
                Exit(ExitResult.UNKNOWN_ERROR);
            }
        
            s.Recognized += (text) =>
            {
                Info("Text: {0}", text);
                engine.GetSnipsIntents(text, out string[] intents, out string json, out string error);
                if (intents.Length > 0)
                {
                    Info("Intents: {0}", intents);
                    if (!intents.First().StartsWith("None"))
                    {
                        Info(intents.First().Split(':').First());
                    }
                    if (!string.IsNullOrEmpty(json))
                    {
                        Info("Slots: {0}", json);
                    }
                }
                Info("Listening. Press any key to exit...");
            };
            s.Start();
            Info("Waiting for the ASR process to become ready...");
            s.Listening += () =>
            {
                Info("Listening. Press any key to exit...");
            };
            System.Console.ReadKey(false);
            Info("Exiting...");
            s.Stop();
            #endif
        }

        static void TTS(string text)
        {
            #if UNIX
            new MimicSession(text).Run();
            #endif
        }

        static void NLU(NLUOptions o)
        {
            SnipsNLUEngine engine = new SnipsNLUEngine("nlu_engine_beverage");
            if (!engine.Initialized)
            {
                Error("Could not initialize SnipsNLU engine.");
                Exit(ExitResult.UNKNOWN_ERROR);
            }

        }
        
        static void PrintLogo()
        {
            CO.WriteLine(FiggleFonts.Chunky.Render("Victor"), Color.Blue);
            CO.WriteLine("v{0}", AssemblyVersion.ToString(3), Color.Blue);
        }

        public static void Exit(ExitResult result)
        {

            if (Cts != null && !Cts.Token.CanBeCanceled)
            {
                Cts.Cancel();
                Cts.Dispose();
            }

            Environment.Exit((int)result);
        }

        static HelpText GetAutoBuiltHelpText(ParserResult<object> result)
        {
            return HelpText.AutoBuild(result, h =>
            {
                h.AddOptions(result);
                return h;
            },
            e =>
            {
                return e;
            });
        }

        static void WriteInfo(string template, params object[] args) => CO.WriteLineFormatted(template, Color.AliceBlue, Color.PaleGoldenrod, args);
        #endregion

        #region Properties
        static string [] Args { get; set; }
        #endregion

        #region Event Handlers
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        { 
            Error((Exception)e.ExceptionObject, "Unhandled error occurred during operation. Victor CLI will now shutdown.");
            Exit(ExitResult.UNHANDLED_EXCEPTION);
        }
        
        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Info("Ctrl-C pressed. Exiting.");
            Cts.Cancel();
            Exit(ExitResult.SUCCESS);
        }
        #endregion
    }
}
