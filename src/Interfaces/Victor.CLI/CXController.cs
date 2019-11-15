using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Text;

using Sc = System.Console;

using Colorful;
using Co = Colorful.Console;


namespace Victor.CLI
{
    public class CXController : CUIController
    {
        #region Constructors 
        public CXController(CXOptions o) : base("Victor CLI", Ct)
        {
            if (_beeperThread == null)
            {
                EnableBeeper();
            }
            StartBeeper();
            Options = o;
            Packages.Add(new CXHome(this));
            HomePackage = Packages[0];
            ActivePackage = Packages[0];
            JuliusSession = new JuliusSession();
            Initialized = Packages[0].Initialized;
            StopBeeper();
        }
        #endregion

        #region Overriden members
        public override void Start()
        {
            ThrowIfNotInitialized();
            if (!Options.Debug)
            {
                Sc.Clear();
            }

            HomePackage.DispatchIntent(null, HomePackage.Welcome);
            ReadLine.HistoryEnabled = true;
            if (beeperOn) Program.StopBeeper();
            Prompt();
        }

        public override void Prompt()
        {
            InputEnabled = true;
            string i = ReadLine.Read("|> ");
            HandleInput(DateTime.Now, i);
        }

        public override void HandleInput(DateTime time, string input)
        {
            ThrowIfNotInitialized();
            InputEnabled = false;
            if (!ActivePackage.HandleInput(time, input))
            {
                SayInfoLineIfDebugEnabled("Input handled by HOME package.");
                if (!HomePackage.HandleInput(time, input))
                {
                    SayCouldNotUnderstand(input);
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

        public override void EnableASR()
        {
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
        }

        public override void StopASR() => JuliusSession.Stop();

        public override bool ASREnabled => JuliusSession.Initialized && JuliusSession.IsListening;
        #endregion

        #region Properties
        public CXOptions Options { get; }

        public JuliusSession JuliusSession { get; protected set; }
        #endregion
        
        #region Methods
        internal static void EnableBeeper()
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
        internal static void _StartBeeper()
        {
            _signalBeep.Set();
            beeperOn = true;
        }

        internal static void _StopBeeper()
        {
            _signalBeep.Reset();
            beeperOn = false;
        }

        private void JuliusSession_Listening()
        {
            if (beeperOn)
            {
                StopBeeper();
            }
            SayInfoLine("ASR enabled.");
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
        private void JuliusSession_Recognized(string sentence)
        {
            if (InputEnabled)
            {
                ReadLine.Send(sentence);
                ReadLine.Send(ConsoleKey.Enter);
            };
        }
        #endregion
        #region Fields
        static Thread _beeperThread;

        static ManualResetEvent _signalBeep;

        public static bool beeperOn;
        #endregion
    }
}
