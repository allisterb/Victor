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
            Args = args;
            if (Args.Contains("--debug"))
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
                PrintLogo();
                Info("Debug mode set.");
            }
            ParserResult<object> result = new Parser().ParseArguments<Options, SpeechRecognitionOptions, TTSOptions, CUIOptions, NLUOptions>(args);
            result.WithNotParsed((IEnumerable<Error> errors) =>
            {
                PrintLogo();
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
            PrintLogo();
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
            else if (o.ListPackages)
            {
                Info("Querying for packages...");
                var descriptors = await c.PackagestorePackagesDescriptorsGetAsync(null, null, null);
                foreach (var d in descriptors)
                {
                    System.Console.WriteLine("{0} {1} {2} Created: {3} Modified: {4}.", d.ResourceId, d.Name, d.Description, d.CreatedOn, d.LastModifiedOn);
                }
            }
            else if (!string.IsNullOrEmpty(o.GetPackage))
            {
                Info("Querying for package {0}...", o.GetPackage);
                try
                {
                    var package = await c.PackagestorePackagesGetAsync(o.GetPackage, o.Version);
                    foreach(var pe in package.PackageExtensions)
                    {
                        System.Console.WriteLine("Extension: {0}", pe.Type.ToString());
                        if (pe.Config.Count > 0)
                        {
                            System.Console.Write("  Config: ");
                            foreach (var config in pe.Config)
                            {
                                System.Console.Write("{0}: {1}", config.Key, config.Value);
                            }
                            System.Console.WriteLine("\n");
                        }
                        if (pe.Extensions.Count > 0)
                        {
                            System.Console.Write("  Extensions: ");
                            foreach (var ex in pe.Extensions)
                            {
                                System.Console.Write("{0}: {1}", ex.Key, ex.Value);
                            }
                            System.Console.WriteLine("\n");
                        }
                    }

                }
                catch (EDDIApiException eae)
                {
                    Error(eae, "Could not get package {0}.", o.GetPackage);
                    return;
                }
            }
            else if (!string.IsNullOrEmpty(o.GetDictionary))
            {
                Info("Querying for dictionary {0}...", o.GetDictionary);
                try
                {
                    var dictionary = await c.RegulardictionarystoreRegulardictionariesGetAsync(o.GetDictionary, o.Version, o.Filter, null, null, null);
                    if(dictionary.Words.Count > 0)
                    {
                        System.Console.WriteLine("Words:");
                        foreach(var w in dictionary.Words)
                        {
                            System.Console.WriteLine("  Word: {0}", w.Word);
                            System.Console.WriteLine("  Frequency: {0}", w.Frequency);
                            System.Console.WriteLine("  Expressions: {0}", w.Expressions);
                        }
                    }
                    if (dictionary.Phrases.Count > 0)
                    {
                        System.Console.WriteLine("Phrases:");
                        foreach (var p in dictionary.Phrases)
                        {
                            System.Console.WriteLine("  Phrase: {0}", p.Phrase);
                            System.Console.WriteLine("  Expressions: {0}", p.Expressions);
                        }
                        System.Console.WriteLine("");
                    }
                    if (dictionary.RegExs.Count > 0) 
                    {
                        System.Console.WriteLine("RegExs:");
                        foreach (var r in dictionary.RegExs)
                        {
                            System.Console.WriteLine("  RegEx: {0}", r.RegEx);
                            System.Console.WriteLine("  Expressions: {0}", r.Expressions);

                        }
                    }

                }
                catch (EDDIApiException eae)
                {
                    Error(eae, "Could not get dictionary {0}.", o.GetDictionary);
                    return;
                }
            }
            else if (!string.IsNullOrEmpty(o.GetBehavior))
            {
                Info("Querying for behavior set {0}...", o.GetBehavior);
                try
                {
                    var behavior = await c.BehaviorstoreBehaviorsetsGetAsync(o.GetBehavior, o.Version);
                    if (behavior.BehaviorGroups.Count > 0)
                    {
                        System.Console.WriteLine("Groups:");
                        foreach (var bg in behavior.BehaviorGroups)
                        {
                            System.Console.WriteLine("  Name: {0}", bg.Name);
                            System.Console.WriteLine("  Execution Strategy: {0}", bg.ExecutionStrategy);
                            if (bg.BehaviorRules.Count > 0)
                            {
                                System.Console.WriteLine("  Rules:");
                                foreach (var r in bg.BehaviorRules)
                                {
                                    System.Console.WriteLine("      Phrase: {0}", r.Name);
                                    if (r.Actions.Count > 0)
                                    {
                                        System.Console.WriteLine("      Actions: {0}", r.Actions.Aggregate((s1, s2) => s1 +"," + s2));
                                    }
                                    if (r.Conditions.Count > 0)
                                    {
                                        foreach(var condition in r.Conditions)
                                        {
                                            System.Console.WriteLine("      Conditions:");
                                            PrintBehaviorRuleCondition(condition, "         ");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (EDDIApiException eae)
                {
                    Error(eae, "Could not get behavior {0}.", o.GetBehavior);
                    return;
                }
            }
            else
            {
                HelpText help = new HelpText();
                help.Copyright = string.Empty;
                help.AddPreOptionsLine(string.Empty);
                help.AddVerbs(typeof(CUIOptions));
                Info(help);

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

        static void PrintBehaviorRuleCondition(BehaviorRuleConditionConfiguration condition, string indent)
        {
            System.Console.WriteLine(indent + "Type: {0}", condition.Type);
            if (condition.Configs.Count > 0)
            {
                System.Console.WriteLine(indent + "Config: " + condition.Configs.Select(cfg => cfg.Key + ":" + cfg.Value).Aggregate((cfg1, cfg2) => cfg1 + " " + cfg2));
            }
            if (condition.Conditions != null && condition.Conditions.Count > 0)
            {
                foreach(var c in condition.Conditions)
                {
                    PrintBehaviorRuleCondition(c, indent + "   ");
                }
            }
        }

        static void PrintLogo()
        {
            CO.WriteLine(FiggleFonts.Chunky.Render("Victor"), Color.Blue);
            CO.WriteLine("v{0}", AssemblyVersion.ToString(3), Color.Blue);
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

        #region Properties
        static string [] Args { get; set; }
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
