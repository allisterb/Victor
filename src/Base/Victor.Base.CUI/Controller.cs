using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Victor.CUI
{
    public abstract class Controller : Api
    {
        #region Constructors
        public Controller(string name, CancellationToken ct) : base(ct)
        {
            Name = name;
        }

        public Controller(string name) : this(name, Ct) {}

        public Controller(string name, CancellationToken ct, params Package[] packages) : this(name, ct)
        {
            Packages.AddRange(packages.ToList());
        }

        #endregion

        #region Properties
        public string Name { get; }

        public List<Package> Packages { get; } = new List<Package>();
        
        public Stack<Context> Context { get; } = new Stack<Context>();
        
        public string PromptString { get; protected set; }

        public Package HomePackage { get; set; }
        
        public Package ActivePackage { get;  set; }

        public Package PreviousPackage { get; set; }

        public bool DebugEnabled { get; set; }

        public bool InputEnabled { get; set; }

        public Intent LastIntent { get; set; }

        public Action<Intent> LastAction { get; set; }

        #endregion

        #region Abstract methods
        public abstract void Start();

        public abstract void Prompt();

        public abstract void HandleInput(DateTime time, string input);

        public abstract void SayInfoLine(string template, params object[] args);

        public abstract void SayErrorLine(string template, params object[] args);

        public abstract void SayWarningLine(string template, params object[] args);
        
        public abstract void StartBeeper();

        public abstract void StopBeeper();

        public abstract void Buzz();

        public abstract void EnableASR();

        public abstract void StopASR();

        public abstract void Exit(ExitResult code);

        public abstract List<byte[]> Scan();

        public abstract bool ASREnabled { get; }
        #endregion

        #region Methods
        public void SetContext(string c, Intent intent = null, Action<Intent> action = null) => Context.Push(new Context(DateTime.Now, c, intent, action));

        public void SetActivePackage(Package package)
        {
            this.PreviousPackage = this.ActivePackage;
            this.ActivePackage = package;
        }

        public void SetPrompt(string prompt) => PromptString = prompt;

        public void SetDefaultPrompt() => PromptString = "|>";

        public void SayInfoLineIfDebug(string template, params object[] args)
        {
            if (DebugEnabled)
            {
                SayInfoLine(template, args);
            }
        }
        #endregion
    }
}
