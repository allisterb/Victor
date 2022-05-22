using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Text;

#if WINDOWS && NET461
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
#endif

using Sc = System.Console;
using Colorful;
using Co = Colorful.Console;

using Victor.CUI;
using Victor.CUI.PM;

namespace Victor.CLI
{
    public class PMController : Controller
    {
        #region Constructors 
        public PMController(PMOptions o) : base("Victor CLI", Ct)
        {
            if (_beeperThread == null)
            {
                EnableBeeper();
            }
            SayInfoLine("Victor PM loading...");
            StartBeeper();
            Options = o;
            if(Options.Debug)
            {
                DebugEnabled = true;
                SayInfoLine("Debug enabled.");
            }
            Packages.Add(new PMHome(this));
            HomePackage = Packages[0];
            ActivePackage = Packages[0];
            PreviousPackage = Packages[0];
#if UNIX
            JuliusSession = new JuliusSession();
#endif

            Initialized = Packages[0].Initialized;
            StopBeeper();
            var boards = Victor.CUI.PM.MdcApi.GetBoards().Result;
        }
        #endregion

        #region Overriden members
        public override void Start()
        {
            ThrowIfNotInitialized();
            HomePackage.DispatchIntent(null, HomePackage.Welcome);
            ReadLine.HistoryEnabled = true;
            if (beeperOn) StopBeeper();
            SetDefaultPrompt();
            Prompt();
        }

        public override void Prompt()
        {
            InputEnabled = true;
            //string prompt = Context.Peek().Label.StartsWith("INPUT_") ? "|*>" : "|>";
            string i =  ReadLine.Read(PromptString);
            HandleInput(DateTime.Now, i);
        }

        public override void HandleInput(DateTime time, string input)
        {
            ThrowIfNotInitialized();
            InputEnabled = false;
            if (!string.IsNullOrEmpty(input.Trim()))
            {
                if (!ActivePackage.HandleInput(time, input))
                {
                    SayInfoLineIfDebug("Input handled by HOME package.");
                    if (!HomePackage.HandleInput(time, input))
                    {
                        SayCouldNotUnderstand(input);
                    }
                }
            }
            Prompt();
        }

        public override void SayInfoLine(string template, params object[] args) => Co.WriteLineFormatted(template, Color.Pink, Color.PaleGoldenrod, args);

        public override void SayErrorLine(string template, params object[] args) => Co.WriteLineFormatted(template, Color.Pink, Color.Red, args);

        public override void SayWarningLine(string template, params object[] args)
        {
            Sc.Beep(5000, 500);
            Co.WriteLineFormatted(template, Color.Pink, Color.Blue, args);
        }
        public override void StartBeeper() => _StartBeeper();

        public override void StopBeeper() => _StopBeeper();

        public override void Buzz() => Sc.Beep(400, 500);

        public override void EnableASR()
        {
#if UNIX
            if (JuliusSession == null || !JuliusSession.Initialized)
            {
                SayInfoLine("Sorry ASR is not available. Check that your Victor binary release has the required files or check the Victor log file for errors.");
                return;
            }
            if (JuliusSession != null && JuliusSession.Initialized && JuliusSession.IsStarted)
            {
                SayInfoLine("ASR is already enabled.");
                return;
            }
            else if (JuliusSession != null && JuliusSession.Initialized && !JuliusSession.IsStarted)
            {
                SayInfoLine("Enabling ASR...");
                JuliusSession.Recognized += JuliusSession_Recognized;
                JuliusSession.Listening += JuliusSession_Listening;
                StartBeeper();
                JuliusSession.Start();
                SayInfoLine("Waiting for the ASR process to become ready...");
            }
#elif WINDOWS && NET461

            /*
            if (sre.Grammars.Count == 0)
            {
                sre.SetInputToDefaultAudioDevice();
                sre.LoadGrammarAsync(rootGrammar);
                sre.LoadGrammarCompleted += Sre_LoadGrammarCompleted;
                sre.SpeechRecognized += Sre_SpeechRecognized;
                sre.SpeechDetected += Sre_SpeechDetected;
                sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected;
                sre.RecognizeAsync(RecognizeMode.Multiple);
            }
            else
            {
                sre.RecognizeAsync(RecognizeMode.Multiple);
                if (beeperOn)
                {
                    StopBeeper();
                }
                SayInfoLine("ASR enabled.");
            }
            */
            
#endif
        }

        public override void StopASR()
        {
#if UNIX
            JuliusSession.Stop();
#elif WINDOWS && NET461
            //sre.RecognizeAsyncStop();
            ///SayInfoLine("ASR disabled.");
#endif
        }

        public override void Exit(ExitResult code) => Victor.CLI.Program.Exit(code);

        public override List<byte[]> Scan()
        {
            throw new NotImplementedException();
        }

        public override bool ASREnabled
        {
            get
            {
                #if UNIX
                return JuliusSession.Initialized && JuliusSession.IsListening;
                #else
                return false;
                #endif
            }
        }
        #endregion

        #region Properties
        public static PMOptions Options { get; set; }
        #if UNIX
        public JuliusSession JuliusSession { get; protected set; }
        #endif
        #endregion
        
        #region Methods
        internal static void EnableBeeper()
        {
            if (Options.NoBeeper) return;
            _signalBeep = new ManualResetEvent(false);
            _beeperThread = new Thread(() =>
            {
                while (true)
                {
                    _signalBeep.WaitOne();
                    Sc.Beep();
                    Thread.Sleep(800);
                }
            }, 1);
            _beeperThread.Name = "Beeper";
            _beeperThread.IsBackground = true;
            _beeperThread.Start();
        }
        internal static void _StartBeeper()
        {
            if (Options.NoBeeper) return;
            _signalBeep.Set();
            beeperOn = true;
        }

        internal static void _StopBeeper()
        {
            if (Options.NoBeeper) return;
            _signalBeep.Reset();
            beeperOn = false;
        }


        protected void SayCouldNotUnderstand(string input)
        {
            if (DebugEnabled)
            {
                SayErrorLine("Did not understand {0}.", input);
            }
            SayErrorLine("Sorry, I don't understand what you mean. Enter {0} to see the things you can do right now or {1} to get more help.", "info", "help");
        }
        #endregion

        #region Event Handlers
#if UNIX
        private void JuliusSession_Recognized(string sentence)
        {
            if (InputEnabled)
            {
                ReadLine.Send(sentence);
                ReadLine.Send(ConsoleKey.Enter);
            };
        }

        private void JuliusSession_Listening()
        {
            if (beeperOn)
            {
                StopBeeper();
            }
            SayInfoLine("ASR enabled.");
        }
#endif

#if WINDOWS && NET461
        private void Sre_LoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs e)
        {
            if (beeperOn)
            {
                StopBeeper();
            }
            SayInfoLine("ASR enabled.");
        }

        private void Sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            
        }

        private void Sre_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (InputEnabled)
            {
                ReadLine.Send(e.Result.Text);
                ReadLine.Send(ConsoleKey.Enter);
            };
        }
#endif
        
        #endregion

        #region Fields
        static Thread _beeperThread;

        static ManualResetEvent _signalBeep;

        public static bool beeperOn;

#if WINDOWS && NET461
        //SpeechRecognitionEngine sre = new SpeechRecognitionEngine();
        //Grammar rootGrammar = new Grammar(new GrammarBuilder(new Choices(
        //    "menu", "info", "help", "exit", "list boards", "list users"
        //)));
#endif
#endregion
    }
}