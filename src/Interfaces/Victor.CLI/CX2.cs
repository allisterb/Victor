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
    public class CX2 : CUIController
    {
        #region Constructors 
        public CX2(CXOptions o) : base("Victor CLI", Ct)
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
            Initialized = Packages[0].Initialized;
            StopBeeper();
        }
        #endregion

        #region Overriden methods
        public override void Start()
        {
            ThrowIfNotInitialized();
            if (!Options.Debug)
            {
                Sc.Clear();
            }

            SayInfoLine("Welcome to Victor CX");
            SetContext("WELCOME");
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
                HomePackage.HandleInput(time, input);
            }
            Prompt();
        }



        public override void SayInfo(string template, params object[] args) => Co.WriteFormatted(template, Color.Pink, Color.PaleGoldenrod, args);

        public override void SayInfoLine(string template, params object[] args) => Co.WriteLineFormatted(template, Color.Pink, Color.PaleGoldenrod, args);

        public override void SayErrorLine(string template, params object[] args) => Co.WriteLineFormatted(template, Color.Pink, Color.Red, args);

        public override void StartBeeper() => _StartBeeper();

        public override void StopBeeper() => _StopBeeper();
        #endregion

        #region Properties
        public CXOptions Options { get; }
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
        #endregion

        #region Fields
        static Thread _beeperThread;

        static ManualResetEvent _signalBeep;

        public static bool beeperOn;
        #endregion
    }
}
