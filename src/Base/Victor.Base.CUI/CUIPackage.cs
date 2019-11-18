using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Victor
{
    public abstract class CUIPackage : Api
    {
        #region Constructors
        public CUIPackage(string name, NLUEngine engine, CUIController controller, CancellationToken ct, params CUIPackage[] subPackages) : base(ct)
        {
            Name = name;
            NLUEngine = engine;
            Controller = controller;
            Intents.Add("menu", Menu);
            if (subPackages != null && subPackages.Length > 0)
            {
                SubPackages = subPackages.ToList();
            }
            foreach(var vn in VariableNames)
            {
                Variables.Add(Prefixed(vn), null);
            }
            foreach(var i in ItemNames)
            {
                Items.Add(Prefixed(i), null);
                ItemsPageSize.Add(Prefixed(i), 10);
                ItemsCurrentPage.Add(Prefixed(i), -1);
                ItemsSelection.Add(Prefixed(i), -1);
                ItemsDescriptionHandlers.Add(Prefixed(i), null);
            }
            foreach(var m in MenuNames)
            {
                MenuHandlers.Add(Prefixed(m), null);
                MenuIndexes.Add(Prefixed(m), 0);
            }
        }
        public CUIPackage(string name, NLUEngine engine, CUIController controller, params CUIPackage[] subPackages) : this(name, engine, controller, Ct, subPackages) {}
        #endregion

        #region Properties
        public string Name { get; }

        public NLUEngine NLUEngine { get; }

        public CUIController Controller { get; }

        public List<CUIPackage> SubPackages { get; } = new List<CUIPackage>();

        public Dictionary<string, Action<Intent>> Intents { get; } = new Dictionary<string, Action<Intent>>();

        public Dictionary<string, string> Variables { get; } = new Dictionary<string, string>();
        
        public Dictionary<string, object> Items { get; } = new Dictionary<string, object>();

        protected Dictionary<string, int> ItemsPageSize { get; } = new Dictionary<string, int>();

        public Dictionary<string, int> ItemsCurrentPage { get; } = new Dictionary<string, int>();

        
        public Dictionary<string, int> ItemsSelection = new Dictionary<string, int>();

        public Dictionary<string, Action<int, CUIPackage>> ItemsDescriptionHandlers { get; } = new Dictionary<string, Action<int, CUIPackage>>();
            
        public Dictionary<string, Action<int>> MenuHandlers { get; } = new Dictionary<string, Action<int>>();

        public Dictionary<string, int> MenuIndexes { get; } = new Dictionary<string, int>();
        
        
        public Dictionary<string, int> MenuSelection = new Dictionary<string, int>();

        #endregion

        #region Methods

        #region Controller
        public Stack<CUIContext> Context => Controller.Context;

        public void SetContext(string name) => Controller.SetContext(Prefixed(name));
        #endregion

        #region UI
        public void DebugIntent(Intent intent)
        {
            SayInfoLine("Context: {0}, Package: {1}, Intent: {2} Score: {3}.", Context.PeekIfNotEmpty().Label, Controller.ActivePackage.Name, intent.Top.Label, intent.Top.Score);
            foreach (var e in intent.Entities)
            {
                SayInfoLine("Entity:{0} Value:{1}.", e.Entity, e.Value);

            }
        }
        protected void SayInfoLine(string template, params object[] args) => Controller.SayInfoLine(template, args);

        protected void SayErrorLine(string template, params object[] args) => Controller.SayErrorLine("Error: " + template, args);

        protected void SayWarningLine(string template, params object[] args) => Controller.SayWarningLine("Warning: " + template, args);
        #endregion

        #region Input
        public void GetInput(string variableName, Action<Intent> action = null, Intent intent = null)
        {
            Controller.SetContext("INPUT_" + Prefixed(variableName), intent, action);
            if (Controller.DebugEnabled)
            {
                SayInfoLine("Get input for variable {0}.", Prefixed(variableName));
            }
        }

        public bool CanDispatchInput(CUIContext context)
        {
            if (context.Label.StartsWith("INPUT_"))
            {
                string label = context.Label.Replace("INPUT_", "");
                return Variables.ContainsKey(label);
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Intent parsing
        public Tuple<string, string, string> GetIntentFeaturePackageFunction(Intent intent)
        {
            if (ObjectEmpty(intent))
            {
                throw new InvalidOperationException("The intent has no object.");
            }
            var feature = intent.Entities.FirstOrDefault(e => e.SlotName == "feature")?.Value.ToAlphaNumeric();
            var package = intent.Entities.FirstOrDefault(e => e.SlotName == "package")?.Value.ToAlphaNumeric();
            var function = intent.Entities.FirstOrDefault(e => e.SlotName == "function")?.Value.ToAlphaNumeric();
            return new Tuple<string, string, string>(feature, package, function);

        }

        public Tuple<string, string, List<string>> GetIntentTaskCommandObjects(Intent intent)
        {
            if (ObjectEmpty(intent))
            {
                throw new InvalidOperationException("The intent has no object.");
            }
            var task = intent.Entities.FirstOrDefault(e => e.SlotName == "task")?.Value;
            var command = intent.Entities.FirstOrDefault(e => e.SlotName == "command")?.Value;
            var objects = intent.Entities.Any(e => e.SlotName == "object") ?
                intent.Entities.Where(e => e.SlotName == "object").Select(e => e.Value?.ToAlphaNumeric()).ToList() : new List<string>();
            return new Tuple<string, string, List<string>>(task, command, objects);

        }
        public bool CanDispatchToMenuItemSelection(CUIContext context)
        {
            if (Controller.Context.Peek().Label.StartsWith("MENU"))
            {
                string label = context.Label.Replace("MENU_", "");
                return MenuHandlers.ContainsKey(label);
            }
            else
            {
                return false;
            }

        }

        public void DispatchIntent(Intent intent, Action<Intent> action)
        {
            if (Controller.DebugEnabled)
            {
                SayInfoLine("Dispatching to command {0}.", action.Method.Name);
            }
            action(intent);
            if (Controller.DebugEnabled)
            {
                SayInfoLine("New context: {0}", Context.Peek().Label);
            }
        }

        public bool Empty(Intent intent) => intent == null || intent.IsNone;

        public bool ObjectEmpty(Intent intent) => !Empty(intent) && intent.Entities.Count() == 0;
        #endregion

        #region Variables and Items
        public string Prefixed(string name) => Name.ToUpper() + "_" + name;

        public string Suffixed(string name) => name + "_" + Name.ToUpper();

        public string GetVar(string name) => Variables[Prefixed(name).ToUpper()];

        public T GetItem<T>(string name) => (T)Items[Prefixed(name).ToUpper()];

        public T SetItem<T>(string name, T value) => (T)(Items[Prefixed(name).ToUpper()] = value);

        public int GetItemsPageSize(string name) => ItemsPageSize[Prefixed(name).ToUpper()];

        #endregion

        #endregion

        #region Virtual and abstract members
        public virtual bool HandleInput(DateTime time, string input)
        {
            ThrowIfNotInitialized();
            if (CanDispatchInput(Context.Peek()))
            {
                DispatchInput(Context.Peek(), input);
                return true;
            }
            else if (Int32.TryParse(input, out int result) && CanDispatchToMenuItemSelection(Controller.Context.Peek()))
            {
                DispatchToMenuItem(Controller.Context.Peek(), DateTime.Now, result);
                return true;
            }
            else if (Int32.TryParse(input, out int _) && !CanDispatchToMenuItemSelection(Controller.Context.Peek()))
            {
                SayInfoLine("A menu is not currently active. Try entering {0} to bring up the available menu.", "menu");
                return true;
            }
            else
            {
                return ParseIntent(Controller.Context.Peek(), time, input);
            }
        }

        public virtual bool ParseIntent(CUIContext context, DateTime time, string input)
        {
            ThrowIfNotInitialized();
            var intent = NLUEngine.GetIntent(input);
            if (intent.Top.Score < 0.8)
            {
                return false;
            }
            else
            {
                switch (intent.Top.Label)
                {
                    case "help":
                        Help(intent);
                        break;
                    case "info":
                        Info(intent);
                        break;
                    default:
                        if (Intents.ContainsKey(intent.Top.Label))
                        {
                            DispatchIntent(intent, Intents[intent.Top.Label]);
                        }
                        else
                        {
                            SayErrorLine("Unknown intent: {0}.", intent.Top.Label);
                            DebugIntent(intent);
                        }
                        break;
                }
                return true;
            }

        }

        public virtual void DispatchToMenuItem(CUIContext context, DateTime time, int i)
        {
            string label = context.Label.Replace("MENU_", "");
            if (i < 1 || i > MenuIndexes[label])
            {
                SayInfoLine("Enter a number between {0} and {1}.", 1, MenuIndexes[label]);
            }
            else
            {
                MenuHandlers[label].Invoke(i);
            }
        }

        public virtual void DispatchInput(CUIContext context, string input)
        {
            string variableName = context.Label.Replace("INPUT_", "");
            Variables[variableName] = input;
            if (Controller.DebugEnabled)
            {
                SayInfoLine("Variable {0} set to to {1}.", variableName, input);
                SayInfoLine("Dispatch input {0} to {1}.", input, context.IntentAction.Method.Name);
            }
            context.IntentAction.Invoke(context.Intent);
            
        }

        #region Intents

        public virtual void Welcome(Intent intent = null)
        {
            Controller.SetContext("WELCOME_" + this.Name.ToUpper());
            Help(null);
        }

        public abstract void Help(Intent intent = null);

        public abstract void Info(Intent intent = null);

        public abstract void Menu(Intent intent = null);
        #endregion

        public abstract string[] VariableNames { get; }

        public abstract string[] MenuNames { get; }

        public abstract string[] ItemNames { get; }

        #endregion
    }
}
