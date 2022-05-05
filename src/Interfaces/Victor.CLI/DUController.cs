﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Text;

using Sc = System.Console;

using Colorful;
using Co = Colorful.Console;

using Victor.CUI;
using Victor.CUI.FN;
using Victor.CUI.DU;

namespace Victor.CLI
{
    public class DUController : Controller
    {
        #region Constructors 
        public DUController(DUOptions o) : base("Victor CLI", Ct)
        {
            if (_beeperThread == null)
            {
                EnableBeeper();
            }
            SayInfoLine("Victor DU loading...");
            StartBeeper();
            Options = o;
            if(Options.Debug)
            {
                DebugEnabled = true;
                SayInfoLine("Debug enabled.");
            }
            Packages.Add(new DUHome(this));
            HomePackage = Packages[0];
            ActivePackage = Packages[0];
            PreviousPackage = Packages[0];
            #if UNIX
            JuliusSession = new JuliusSession();
            #endif
            Initialized = Packages[0].Initialized;
            StopBeeper();
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
            #endif
        }

        public override void StopASR()
        {
            #if UNIX
            JuliusSession.Stop();
            #endif
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

        public override void Exit(ExitResult code)
        {
            Program.Exit(code);
        }
        #endregion

        #region Properties
        public static DUOptions Options { get; set; }
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

        #endregion

        #region Fields
        static Thread _beeperThread;

        static ManualResetEvent _signalBeep;

        public static bool beeperOn;
        #endregion
    }
}
