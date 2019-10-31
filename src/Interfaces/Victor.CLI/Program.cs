using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Colorful;
using CO = Colorful.Console;
using Figgle;
using CommandLine;
using CommandLine.Text;

namespace Victor
{
    #region Enums
    public enum ExitResult
    {
        SUCCESS = 0,
        UNHANDLED_EXCEPTION = 1,
        INVALID_OPTIONS = 2,
        UNKNOWN_ERROR = 3
    }
    #endregion

    class Program : Api
    {
        #region Entry-point
        static void Main(string[] args)
        {
            if (args.Contains("--debug"))
            {
                SetLogger(new SerilogLogger(console: true, debug: true));
            }
            else
            {
                SetLogger(new SerilogLogger(console: true, debug: false));
            }

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            System.Console.CancelKeyPress += Console_CancelKeyPress;
            if (args.Contains("--debug"))
            {
                CO.WriteLine(FiggleFonts.Chunky.Render("Victor"), Color.Blue);
                CO.WriteLine("v{0}", AssemblyVersion.ToString(3), Color.Blue);
                Info("Debug mode set.");
            }
            ParserResult<object> result = new Parser().ParseArguments<Options, SpeechRecognitionOptions, TTSOptions, CUIOptions, NLUOptions>(args);
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
                        help.AddVerbs(typeof(SpeechRecognitionOptions), typeof(TTSOptions), typeof(CUIOptions), typeof(NLUOptions));
                    }
                    Info(help);
                    Exit(ExitResult.SUCCESS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.HelpRequestedError))
                {
                    help.AddVerbs(typeof(SpeechRecognitionOptions), typeof(TTSOptions), typeof(CUIOptions), typeof(NLUOptions));
                    Info(help);
                    Exit(ExitResult.SUCCESS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.NoVerbSelectedError))
                {
                    help.AddVerbs(typeof(SpeechRecognitionOptions), typeof(TTSOptions), typeof(CUIOptions), typeof(NLUOptions));
                    Error("No input selected. Specify one of: mic.");
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
                    help.AddVerbs(typeof(SpeechRecognitionOptions), typeof(TTSOptions), typeof(CUIOptions), typeof(NLUOptions));
                    Error("Unknown option: {error}.", error.Token);
                    Info(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
                else
                {
                    Error("An error occurred parsing the program options: {errors}.", errors);
                    help.AddVerbs(typeof(SpeechRecognitionOptions), typeof(TTSOptions), typeof(CUIOptions), typeof(NLUOptions));
                    Info(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
            })
            .WithParsed<SpeechRecognitionOptions>(o =>
            {
                Recognize();
                Exit(ExitResult.SUCCESS);
            })
            .WithParsed<TTSOptions>(o =>
            {
                TTS(o.Text);
                Exit(ExitResult.SUCCESS);
            })
            .WithParsed<CUIOptions>(o =>
            {
                CUI(o).Wait();
            })
            .WithParsed<NLUOptions>(o =>
            {
                NLU(o);
            });

        }
        #endregion

        #region Methods
        static void Recognize()
        {
            JuliusSession s = new JuliusSession();
            if (!s.Initialized)
            {
                Error("Could not initialize Julius session.");
                Exit(ExitResult.UNKNOWN_ERROR);
            }
            SnipsNLUEngine engine = new SnipsNLUEngine("nlu_engine_beverage");
            if (!engine.Initialized)
            {
                Error("Could not initialize SnipsNLU engine.");
                Exit(ExitResult.UNKNOWN_ERROR);
            }
        
            s.Recognized += (text) =>
            {
                engine.GetIntents(text, out string[] intents, out string json, out string error);
                if (intents.Length > 0)
                {
                    Info("Intents: {0}", intents);
                    if (!intents.First().StartsWith("None"))
                    {
                        new MimicSession(intents.First().Split(':').First()).Run();
                    }
                    if (!string.IsNullOrEmpty(json))
                    {
                        Info("Slots: {0}", json);
                    }
                }
            };
            s.Start();
            s.WaitForExit();
        }

        static void TTS(string text)
        {
            new MimicSession(text).Run();
        }

        static async Task CUI(CUIOptions o)
        {
            EDDIClient c = new EDDIClient(Config("CUI:EDDIServerUrl"), HttpClient);
            if (o.ListBots)
            {
                Info("Querying for bots...");
                var descriptors = await c.BotstoreBotsDescriptorsGetAsync(null, null, null);
                foreach (var d in descriptors)
                {
                    System.Console.WriteLine("{0} {1} {2} Created: {3} Modified: {4}.", d.ResourceId, d.Name, d.Description, d.CreatedOn, d.LastModifiedOn);
                }
            }
            else if (!string.IsNullOrEmpty(o.ExportBot))
            {
                    var r = await c.BackupExportPostAsync(o.ExportBot, 1);
                    System.Console.WriteLine("Bot {0} exported to location: {1}.", o.ExportBot, r);
            }
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
        static void Exit(ExitResult result)
        {

            if (Cts != null && !Cts.Token.CanBeCanceled)
            {
                Cts.Cancel();
                Cts.Dispose();
            }

            Environment.Exit((int)result);
        }

        static int ExitWithCode(ExitResult result)
        {
            return (int)result;
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
        #endregion

        #region Event Handlers
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Error((Exception)e.ExceptionObject, "Error occurred during operation. Victor CLI will shutdown.");
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
