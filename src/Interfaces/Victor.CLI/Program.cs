using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Colorful;
using CO = Colorful.Console;
using Figgle;
using CommandLine;
using CommandLine.Text;

using Victor.CLI;

namespace Victor
{
    #region Enums
    public enum ExitResult
    {
        SUCCESS = 0,
        UNHANDLED_EXCEPTION = 1,
        INVALID_OPTIONS = 2,
        UNKNOWN_ERROR = 3,
        NOT_FOUND_OR_SERVER_ERROR = 4
    }
    #endregion

    class Program : Api
    {
        #region Entry-point
        static void Main(string[] args)
        {
            Args = args;
            if (Args.Contains("cx"))
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
            System.Console.CancelKeyPress += Console_CancelKeyPress;
            EnableBeeper();
            if (args.Contains("--debug"))
            {
                PrintLogo();
                Info("Debug mode set.");
            }
            ParserResult<object> result = new Parser().ParseArguments<Options, SpeechRecognitionOptions, TTSOptions, CUIOptions, NLUOptions, CXOptions>(args);
            result.WithNotParsed((IEnumerable<Error> errors) =>
            {
                if (!args.Contains("--debug"))
                {
                    PrintLogo();
                }
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
                        help.AddVerbs(typeof(SpeechRecognitionOptions), typeof(TTSOptions), typeof(CUIOptions), typeof(NLUOptions), typeof(CXOptions));
                    }
                    Info(help);
                    Exit(ExitResult.SUCCESS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.HelpRequestedError))
                {
                    HelpRequestedError error = (HelpRequestedError)errors.First(e => e.Tag == ErrorType.HelpRequestedError);
                    help.AddVerbs(typeof(SpeechRecognitionOptions), typeof(TTSOptions), typeof(CUIOptions), typeof(NLUOptions), typeof(CXOptions));
                    Info(help);
                    Exit(ExitResult.SUCCESS);
                }
                else if (errors.Any(e => e.Tag == ErrorType.NoVerbSelectedError))
                {
                    help.AddVerbs(typeof(SpeechRecognitionOptions), typeof(TTSOptions), typeof(CUIOptions), typeof(NLUOptions), typeof(CXOptions));
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
                    help.AddVerbs(typeof(SpeechRecognitionOptions), typeof(TTSOptions), typeof(CUIOptions), typeof(NLUOptions), typeof(CXOptions));
                    Error("Unknown option: {error}.", error.Token);
                    Info(help);
                    Exit(ExitResult.INVALID_OPTIONS);
                }
                else
                {
                    Error("An error occurred parsing the program options: {errors}.", errors);
                    help.AddVerbs(typeof(SpeechRecognitionOptions), typeof(TTSOptions), typeof(CUIOptions), typeof(NLUOptions), typeof(CXOptions));
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
                Exit(ExitResult.SUCCESS);
            })
            .WithParsed<NLUOptions>(o =>
            {
                NLU(o);
            })
            .WithParsed<CXOptions>(o =>
            {
                StartBeeper();
                new CX(o).Start();
                Exit(ExitResult.SUCCESS);
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
            EDDIClient c = new EDDIClient(Config("CUI_EDDI_SERVER_URL"), HttpClient);
            if (o.GetBots)
            {
                Info("Querying for bots...");
                try
                {
                    var descriptors = await c.BotstoreBotsDescriptorsGetAsync(null, null, null);
                    if (o.Json)
                    {
                        System.Console.WriteLine(EDDIClient.Serialize(descriptors));
                        WriteToFileIfRequired(o, EDDIClient.Serialize(descriptors));
                    }
                    else
                    {
                        foreach (var d in descriptors)
                        {
                            System.Console.WriteLine("{0} {1} {2} Created: {3} Modified: {4}.", d.ResourceId, d.Name, d.Description, d.CreatedOn, d.LastModifiedOn);
                        }
                    }
                }
                catch (EDDIApiException eae)
                {
                    Error("Could not get bot descriptors: {0}.", eae.Message);
                    Exit(ExitResult.NOT_FOUND_OR_SERVER_ERROR);
                }
                catch (Exception e)
                {
                    Error(e, "Unknown error retrieving bot descriptors.");
                    Exit(ExitResult.UNKNOWN_ERROR);
                }
            }
            else if(!string.IsNullOrEmpty(o.GetBot))
            {
                Info("Querying for bot {0}...", o.GetBot);
                try
                {
                    var bot = await c.BotstoreBotsGetAsync(o.GetBot, o.Version);
                    if (o.Json)
                    {
                        System.Console.WriteLine(EDDIClient.Serialize(bot));
                        WriteToFileIfRequired(o, EDDIClient.Serialize(bot));
                    }
                    else 
                    {
                        foreach(var p in bot.Packages)
                        {
                            System.Console.WriteLine("Package: {0}", p.Segments.Last());
                        }
                        
                        if (bot.Channels != null)
                        {
                            foreach (var channel in bot.Channels)
                            {
                                System.Console.WriteLine("Channel: {0}", channel.Type);
                            }
                        }
                        
                        if (bot.GitBackupSettings != null && bot.GitBackupSettings.RepositoryUrl != null)
                        {
                            System.Console.WriteLine("Git repo: {0}", bot.GitBackupSettings.RepositoryUrl.ToString());
                        }
                        
                    }
                }
                catch (EDDIApiException eae)
                {
                    Error("Could not get bot {0}: {1}.", o.GetBot, eae.Message);
                    Exit(ExitResult.NOT_FOUND_OR_SERVER_ERROR);
                }
                catch (Exception e)
                {
                    Error(e, "Unknown error retrieving bot {0}.", o.GetBot);
                    Exit(ExitResult.UNKNOWN_ERROR);
                }
            }
        
            else if (!string.IsNullOrEmpty(o.ExportBot))
            {
                try
                {
                    var r = await c.BackupExportPostAsync(o.ExportBot, 1);
                    System.Console.WriteLine("Bot {0} exported to location: {1}.", o.ExportBot, r);
                    Exit(ExitResult.SUCCESS);
                }
                catch (EDDIApiException eae)
                {
                    Error("Could not export bot: {0}: {1}", o.ExportBot, eae.Message);
                    Exit(ExitResult.NOT_FOUND_OR_SERVER_ERROR);
                }
                catch (Exception e)
                {
                    Error(e, "Unknown error exporting bot {0}.", o.ExportBot);
                    Exit(ExitResult.UNHANDLED_EXCEPTION);
                }
            }

            else if (o.GetPackages)
            {
                Info("Querying for packages...");
                try
                {
                    var descriptors = await c.PackagestorePackagesDescriptorsGetAsync(null, null, null);
                    if (o.Json)
                    {
                        System.Console.WriteLine(EDDIClient.Serialize(descriptors));
                        WriteToFileIfRequired(o, EDDIClient.Serialize(descriptors));
                    }
                    else
                    {
                        foreach (var d in descriptors)
                        {
                            System.Console.WriteLine("{0} {1} {2} Created: {3} Modified: {4}.", d.ResourceId, d.Name, d.Description, d.CreatedOn, d.LastModifiedOn);
                        }
                    }
                }
                catch (EDDIApiException eae)
                {
                    Error("Could not list packages: {0}", eae.Message);
                    Exit(ExitResult.UNHANDLED_EXCEPTION);
                }
                catch (Exception e)
                {
                    Error(e, "Unknown error retrieving packages.");
                    Exit(ExitResult.UNHANDLED_EXCEPTION);
                }
            }

            else if (!string.IsNullOrEmpty(o.GetPackage))
            {
                Info("Querying for package {0}...", o.GetPackage);
                try
                {
                    var package = await c.PackagestorePackagesGetAsync(o.GetPackage, o.Version);
                    if (o.Json)
                    {
                        System.Console.WriteLine(EDDIClient.Serialize(package));
                        WriteToFileIfRequired(o, EDDIClient.Serialize(package));
                    }
                    else
                    {
                        foreach (var pe in package.PackageExtensions)
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
                }
                catch (EDDIApiException eae)
                {
                    Error("Could not get package {0}: {1}", o.GetPackage, eae.Message);
                    Exit(ExitResult.UNHANDLED_EXCEPTION);
                }
                catch (Exception e)
                {
                    Error(e, "Unknown error retrieving package: {0}.", o.GetPackage);
                    Exit(ExitResult.UNHANDLED_EXCEPTION);
                }
            }
            else if (!string.IsNullOrEmpty(o.GetDictionary))
            {
                Info("Querying for dictionary {0}...", o.GetDictionary);
                try
                {
                    var dictionary = await c.RegulardictionarystoreRegulardictionariesGetAsync(o.GetDictionary, o.Version, o.Filter, null, null, null);
                    if (o.Json)
                    {
                        System.Console.WriteLine(EDDIClient.Serialize(dictionary));
                        WriteToFileIfRequired(o, EDDIClient.Serialize(dictionary));
                    }
                    else
                    {
                        if (dictionary.Words.Count > 0)
                        {
                            System.Console.WriteLine("Words:");
                            foreach (var w in dictionary.Words)
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
                }
                catch (EDDIApiException eae)
                {
                    Error(eae, "Could not get dictionary {0}.", o.GetDictionary);
                    Exit(ExitResult.NOT_FOUND_OR_SERVER_ERROR);
                }
                catch (Exception e)
                {
                    Error(e, "Unknown error retrieving dictionary {0}", o.GetDictionary);
                    Exit(ExitResult.UNKNOWN_ERROR);
                }
            }

            else if (!string.IsNullOrEmpty(o.GetBehavior))
            {
                Info("Querying for behavior set {0}...", o.GetBehavior);
                try
                {
                    var behavior = await c.BehaviorstoreBehaviorsetsGetAsync(o.GetBehavior, o.Version);
                    if (o.Json)
                    {
                        System.Console.WriteLine(EDDIClient.Serialize(behavior));
                        WriteToFileIfRequired(o, EDDIClient.Serialize(behavior));
                    }
                    else
                    {
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
                                            System.Console.WriteLine("      Actions: {0}", r.Actions.Aggregate((s1, s2) => s1 + " " + s2));
                                        }
                                        if (r.Conditions.Count > 0)
                                        {
                                            foreach (var condition in r.Conditions)
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
                }
                catch (EDDIApiException eae)
                {
                    Error("Could not get behavior {0} : {1}.", o.GetBehavior, eae.Message);
                    Exit(ExitResult.NOT_FOUND_OR_SERVER_ERROR);
                }
            }

            else if (!string.IsNullOrEmpty(o.GetOutput))
            {
                Info("Querying for output set {0}...", o.GetOutput);
                try
                {
                    var outputSet = await c.OutputstoreOutputsetsGetAsync(o.GetOutput, o.Version, o.Filter, null, null, null);
                    if (o.Json)
                    {
                        System.Console.WriteLine(EDDIClient.Serialize(outputSet));
                        WriteToFileIfRequired(o, EDDIClient.Serialize(outputSet));
                    }
                    else
                    {
                        if (outputSet.OutputSet != null && outputSet.OutputSet.Count > 0)
                        {
                            foreach (var oc in outputSet.OutputSet)
                            {
                                System.Console.WriteLine("Action: {0}", oc.Action);
                                System.Console.WriteLine("Times Occurred: {0}", oc.TimesOccurred);
                                if (oc.Outputs.Count > 0)
                                {
                                    System.Console.WriteLine("Outputs: ");
                                    foreach (var output in oc.Outputs)
                                    {
                                        System.Console.WriteLine("  Type: {0}. Alternatives: {1}", output.Type, output.ValueAlternatives.Select(va => va.ToString()).Aggregate((s1, s2) => s1 + "=" + s2));
                                    }
                                }
                            }
                        }
                    }
                }
                catch (EDDIApiException eae)
                {
                    Error("Could not get output {0}: {1}", o.GetOutput, eae.Message);
                    Exit(ExitResult.UNHANDLED_EXCEPTION);
                }
                catch (Exception e)
                {
                    Error(e, "Unknown error retrieving output {0}.", o.GetOutput);
                    Exit(ExitResult.UNHANDLED_EXCEPTION);
                }
            }
            else if (!string.IsNullOrEmpty(o.GetProperty))
            {
                Info("Querying for property {0}...", o.GetProperty);
                try
                {
                    var prop = await c.PropertysetterstorePropertysettersGetAsync(o.GetProperty, o.Version);
                    if (o.Json)
                    {
                        System.Console.WriteLine(EDDIClient.Serialize(prop));
                        WriteToFileIfRequired(o, EDDIClient.Serialize(prop));
                    }
                    else
                    {
                        if (prop.SetOnActions != null && prop.SetOnActions.Count > 0)
                        {
                            foreach (var soa in prop.SetOnActions)
                            {
                                System.Console.WriteLine("Actions: {0}", soa.Actions.Aggregate((a1, a2) => a1 + " " + a2));
                                if (soa.SetProperties.Count > 0)
                                {
                                    System.Console.WriteLine("Set Properties: ");
                                    foreach (var ap in soa.SetProperties)
                                    {
                                        System.Console.WriteLine("  " + EDDIClient.Serialize(ap));
                                    }
                                }
                            }
                        }
                    }
                }
                catch (EDDIApiException eae)
                {
                    Error("Could not get property {0}: {1}", o.GetProperty, eae.Message);
                    Exit(ExitResult.UNHANDLED_EXCEPTION);
                }
                catch (Exception e)
                {
                    Error(e, "Unknown error retrieving property {0}.", o.GetProperty);
                    Exit(ExitResult.UNHANDLED_EXCEPTION);
                }

            }
            else if (o.CreateDictionary)
            {
                Info("Creating dictionary...");
                await c.RegulardictionarystoreRegulardictionariesPostAsync(ReadFromFileIfRequired<RegularDictionaryConfiguration>(o));
                int s = EDDIClient.LastStatusCode;
                if (s == 201)
                {
                    string l = EDDIClient.GetLastResponseHeader("Location").First();
                    Info("Created dictionary at {0}.", l);
                }
                else
                {
                    Error("Did not create dictionary. HTTP status code {0}.", s);
                }
            }
            else if (o.CreateBehavior)
            {
                Info("Creating behavior...");
                await c.BehaviorstoreBehaviorsetsPostAsync(ReadFromFileIfRequired<BehaviorConfiguration>(o));
                int s = EDDIClient.LastStatusCode;
                if (s == 201)
                {
                    string l = EDDIClient.GetLastResponseHeader("Location").First();
                    Info("Created behavior set at {0}.", l);
                }
                else
                {
                    Error("Did not create behavior set. HTTP status code {0}.", s);
                }
            }
            else if (o.CreateOutput)
            {
                Info("Creating output...");
                await c.OutputstoreOutputsetsPostAsync(ReadFromFileIfRequired<OutputConfigurationSet>(o));
                int s = EDDIClient.LastStatusCode;
                if (s == 201)
                {
                    string l = EDDIClient.GetLastResponseHeader("Location").First();
                    Info("Created output configuration set at {0}.", l);
                }
                else
                {
                    Error("Did not create output configuration set. HTTP status code {0}.", s);
                }
            }

            else if (o.CreatePackage)
            {
                Info("Creating package...");
                await c.PackagestorePackagesPostAsync(ReadFromFileIfRequired<PackageConfiguration>(o));
                int s = EDDIClient.LastStatusCode;
                if (s == 201)
                {
                    string l = EDDIClient.GetLastResponseHeader("Location").First();
                    Info("Created package at {0}.", l);
                }
                else
                {
                    Error("Did not create package. HTTP status code {0}.", s);
                }
            }
           
            else if(!string.IsNullOrEmpty(o.StartConversation))
            {
                Info("Starting conversation with bot {0}...", o.StartConversation);
                await c.BotsPostAsync(Environment8.Test, o.StartConversation, "", null);
                int s = EDDIClient.LastStatusCode;
                if (s == 201)
                {
                    string l = EDDIClient.GetLastResponseHeader("Location").First();
                    Info("Started conversation at {0}.", l);
                }
                else
                {
                    Error("Did not start conversation. HTTP status code {0}.", s);
                }
            }

            else if (!string.IsNullOrEmpty(o.GetConversation))
            {
                try
                {
                    Info("Getting conversation {0}...", o.GetConversation);
                    var convo = await c.ConversationstoreConversationsSimpleAsync(o.GetConversation, true, true, null);
                    if (o.Json)
                    {
                        System.Console.WriteLine(EDDIClient.Serialize(convo));
                        WriteToFileIfRequired(o, EDDIClient.Serialize(convo));

                    }
                    else
                    {
                        System.Console.WriteLine("BotId:{0}, BotVersion:{1}, BotEnvironment:{2}.", convo.BotId, convo.BotVersion, convo.Environment.ToString());
                    }
                    
                }
                catch (EDDIApiException eae)
                {
                    Error("Could not get conversation: {0}: {1}", o.GetConversation, eae.Message);
                    Exit(ExitResult.NOT_FOUND_OR_SERVER_ERROR);
                }
                catch (Exception e)
                {
                    Error(e, "Unknown error getting conversation {0}.", o.GetConversation);
                    Exit(ExitResult.UNHANDLED_EXCEPTION);
                }
            }

            else
            {
                Error("Select the CUI operation and options you want to use.");
                HelpText help = new HelpText();
                help.Copyright = string.Empty;
                help.AddPreOptionsLine(string.Empty);
                help.AddVerbs(typeof(CUIOptions));
                help.AddOptions(new Parser().ParseArguments<CUIOptions>(Args));
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

        static void WriteToFileIfRequired(CUIOptions o, string s)
        {
            if (!string.IsNullOrEmpty(o.File))
            {
                if (File.Exists(o.File) && !o.Overwrite)
                {
                    Error("The file {0} already exists.", o.File);
                }
                else
                {
                    File.WriteAllText(o.File, s);
                    Info("Wrote to {0}.", o.File);
                }
            }
        }

        static T ReadFromFileIfRequired<T>(CUIOptions o)
        {
            if (!string.IsNullOrEmpty(o.File))
            {
                Info("Using {0} as input.", o.File);
                return EDDIClient.Deserialize<T>(File.ReadAllText(o.File)); 
            }
            else
            {
                return EDDIClient.Deserialize<T>(File.ReadAllText(o.Input));
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

        static int ExitWithCode(ExitResult result)
        {
            return (int)result;
        }

        static void EnableBeeper()
        {
            _signalBeep = new ManualResetEvent(false);
            _beeperThread = new Thread(() =>
            {
                while (true)
                {
                    _signalBeep.WaitOne();
                    System.Console.Beep();
                    Thread.Sleep(800);
                }
            }, 1);
            _beeperThread.Name = "Beeper";
            _beeperThread.IsBackground = true;
            _beeperThread.Start();
        }

        public static void StartBeeper()
        {
            _signalBeep.Set();
            beeperOn = true;
        }

        public static void StopBeeper()
        {
            _signalBeep.Reset();
            beeperOn = false;
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

        #region Fields
        static Thread _beeperThread;

        static ManualResetEvent _signalBeep;

        public static bool beeperOn;
        #endregion
    }
}
