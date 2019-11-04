using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Diagnostics;


using Sc = System.Console;

using Colorful;
using Co = Colorful.Console;

namespace Victor.CLI
{
    public class CX : Api
    {
        #region Constructors
        public CX(CXOptions options) : base() 
        {
            Options = options;
            Client = new EDDIClient("http://eddi-evals25-shared-7daa.apps.hackathon.rhmi.io", HttpClient);
            
        }

        private void JuliusSession_Recognized(string sentence)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Properties
        public CXOptions Options { get; }
        protected EDDIClient Client { get;  }
        protected JuliusSession JuliusSession { get; } = new JuliusSession();

        //protected SnipsNLUEngine GeneralNLU = new SnipsNLUEngine("victorcx_general");

        protected Tuple<DateTime, string> Context { get; set; }

        public void SetContext(string c) => Context = new Tuple<DateTime, string>(DateTime.Now, c);
        #endregion

        #region Methods
        public void Start()
        {
            Sc.Clear();
            Sc.Beep();
            Sc.Beep();
            
            WriteLine("Welcome to Victor CX");
            SetContext("Welcome");
            Prompt();
        }

        public void WriteLine(string s) => Co.WriteLine(s, Color.PaleGoldenrod);
        
        public string Prompt() => ReadLine.Read("|> ");

        #endregion
    }
}
