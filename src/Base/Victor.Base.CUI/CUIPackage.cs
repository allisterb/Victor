using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Humanizer;
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
            Intents.Add("help", Help);
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
                ItemsCurrentPage.Add(Prefixed(i), 1);
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

        protected void SayInfoLineIfDebug(string template, params object[] args) => Controller.SayInfoLineIfDebug(template, args);

        protected void SayErrorLine(string template, params object[] args) => Controller.SayErrorLine("Error: " + template, args);

        protected void SayWarningLine(string template, params object[] args) => Controller.SayWarningLine("Warning: " + template, args);

        protected void SetPrompt(string prompt) => Controller.SetPrompt(prompt);
        #endregion

        #region Context
        public Stack<CUIContext> Context => Controller.Context;

        public string CurrentContext => Context.Count > 0 ? Context.Peek().Label : "";

        public bool IsItemsContext => CurrentContext.StartsWith("ITEMS_");

        public string GetItemsContext() => CurrentContext.Replace("ITEMS_"+ this.Name.ToUpper() + "_", "");

        public void SetItemsContext(string name) => Controller.SetContext("ITEMS_" + Prefixed(name));

        public bool IsMenuContext => CurrentContext.StartsWith("MENU_");

        public string GetMenuContext() => CurrentContext.Replace("MENU_", "").Replace(this.Name.ToUpper() + "_", "");

        public void SetMenuContext(string name, Intent intent = null, Action<Intent> action = null) => Controller.SetContext("MENU_" + Prefixed(name), intent, action);
        #endregion

        #region Variable Input
        public void GetVariableInput(string variableName, Action<Intent> action = null, Intent intent = null)
        {
            Controller.SetContext("INPUT_" + Prefixed(variableName), intent, action);
            Controller.SetPrompt("|*>");
            if (Controller.DebugEnabled)
            {
                SayInfoLine("Get input for variable {0}.", Prefixed(variableName));
            }
        }

        public bool CanDispatchVariableInput(CUIContext context)
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

        #region Variables
        public string Prefixed(string name) => Name.ToUpper() + "_" + name;

        public string Suffixed(string name) => name + "_" + Name.ToUpper();

        public string GetVar(string name) => Variables[Prefixed(name).ToUpper()];

        public string SetVar(string name, string value) => Variables[Prefixed(name).ToUpper()] = value;
        #endregion

        #region Items
        public T GetItems<T>(string name) => Items[Prefixed(name).ToUpper()] != null ? (T)Items[Prefixed(name).ToUpper()] : default(T);

        public T GetOrFetchItems<T>(string name, Func<T> fetch) => Items[Prefixed(name).ToUpper()] != null ? (T)Items[Prefixed(name).ToUpper()] : fetch();

        public T SetItems<T>(string name, T value) => (T)(Items[Prefixed(name).ToUpper()] = value);

        public int GetItemsPageSize(string name) => ItemsPageSize[Prefixed(name).ToUpper()];

        public int SetItemsPageSize(string name, int value) => ItemsPageSize[Prefixed(name).ToUpper()] = value;

        public int GetItemsCurrentPage(string name) => ItemsCurrentPage[Prefixed(name).ToUpper()];

        public Action<int, CUIPackage> GetItemsDescriptionHandler(string name) => ItemsDescriptionHandlers[Prefixed(name).ToUpper()];

        public void DescribeItem(string name, int index) => ItemsDescriptionHandlers[Prefixed(name)].Invoke(index, this);

        public bool CanDispatchToItemsPage()
        {
            if (IsItemsContext)
            {
                string label = CurrentContext.Replace("ITEMS_", "");
                return Items.ContainsKey(label);
            }
            else
            {
                return false;
            }
        }

        public void DescribeItems(int page)
        {
            var items = GetItemsContext();
            var handler = GetItemsDescriptionHandler(items);
            handler.Invoke(page, this);
        }
        #endregion

        #region Menus
        public bool CanDispatchToMenuSelection()
        {
            if (IsMenuContext)
            {
                string label = CurrentContext.Replace("MENU_", "");
                return MenuHandlers.ContainsKey(label);
            }
            else
            {
                return false;
            }
        }

        public void DispatchToMenuItem(CUIContext context, DateTime time, int i)
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

        #endregion

        #region Intent parsing
        public Tuple<string, string, string> GetIntentFeaturePackageFunction(Intent intent)
        {
            if (ObjectEmpty(intent))
            {
                throw new InvalidOperationException("The intent has no object.");
            }
            var feature = intent.Entities.FirstOrDefault(e => e.SlotName.EndsWith("feature"))?.Value;
            var package = intent.Entities.FirstOrDefault(e => e.SlotName.EndsWith("package"))?.Value;
            var function = intent.Entities.FirstOrDefault(e => e.SlotName.EndsWith("function"))?.Value;
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
            var objects = 
                intent.Entities.Any(e => e.SlotName.EndsWith("_object")) ?
                intent.Entities
                .Where(e => e.SlotName.EndsWith("_object"))
                .Select(e => e.Value?.ToAlphaNumeric()).ToList() : new List<string>();
            if (Controller.DebugEnabled)
            {
                SayInfoLine("Task: {0}, Command: {1}, Objects: {2}.", task ?? "None", command ?? "None", objects);
            }
            return new Tuple<string, string, List<string>>(task, command, objects);
        }

        public void Page(Intent intent)
        {
            if (!IsItemsContext)
            {
                SayErrorLine("Sorry I don't understand what you mean. There is not anything I can page.");
                return;
            }
            var input = intent.Input.Trim().ToLower();
            var items = GetItemsContext();
            var current = GetItemsCurrentPage(items);
            int page;
            if (ObjectEmpty(intent))
            {
                if (input == "np" || input.Contains("next"))
                {
                    page = current + 1;
                }
                else if (input == "pp" || intent.Input.Contains("previous"))
                {
                    page = current - 1;
                }
                else
                {
                    SayErrorLine("Sorry I don't understand what you mean. Say something like {0} or {1}.", "page 5", "next page");
                    return;
                }
            }
            else
            {
                string _no = intent.Entities.FirstOrDefault(e => e.SlotName == "no")?.Value;
                page = _no.ToInteger();
            }
            DescribeItems(page);
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

        public bool ObjectEmpty(Intent intent) => Empty(intent) || intent.Entities.Count() == 0;
        #endregion

        #endregion

        #region Virtual and abstract members
        public virtual bool HandleInput(DateTime time, string input)
        {
            if (CanDispatchVariableInput(Context.Peek()))
            {
                DispatchVariableInput(Context.Peek(), input);
                return true;
            }
            else if (Int32.TryParse(input, out int result))
            {
                if (CanDispatchToMenuSelection())
                {
                    DispatchToMenuItem(Controller.Context.Peek(), DateTime.Now, result);
                    return true;
                }
                else if (CanDispatchToItemsPage())
                {
                    DescribeItems(GetItemsPageSize(GetItemsContext()));
                    return true;
                }
                else
                {
                    return ParseIntent(Controller.Context.Peek(), time, input);
                }
            }
            else
            {
                return ParseIntent(Controller.Context.Peek(), time, input);
            }
        }

        public virtual bool ParseIntent(CUIContext context, DateTime time, string input)
        {
            var intent = NLUEngine.GetIntent(input);
            if (Controller.DebugEnabled)
            {
                DebugIntent(intent);
            }
            if (!intent.IsNone && intent.Top.Label == "menu" && intent.Top.Score > 0.7)
            {
                Menu(intent);
            }
            if (intent.Top.Score < 0.8)
            {
                return false;
            }
            else
            {
                if (Intents.ContainsKey(intent.Top.Label))
                {
                    DispatchIntent(intent, Intents[intent.Top.Label]);
                }
                else
                {
                    SayErrorLine("This package recognizes intent {0} but does not have handler for it.", intent.Top.Label);
                    DebugIntent(intent);
                }                     
                return true;
            }
        }

        public virtual void DispatchVariableInput(CUIContext context, string input)
        {
            string variableName = context.Label.Replace("INPUT_", "");
            Variables[variableName] = input;
            if (Controller.DebugEnabled)
            {
                SayInfoLine("Variable {0} set to to {1}.", variableName, input);
                SayInfoLine("Dispatch variable input {0} to {1}.", input, context.IntentAction.Method.Name);
            }
            Controller.SetDefaultPrompt();
            context.IntentAction.Invoke(context.Intent);
        }

        #region Intents

        public virtual void Welcome(Intent intent = null)
        {
            Controller.SetContext("WELCOME_" + this.Name.ToUpper());
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
