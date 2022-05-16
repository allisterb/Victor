using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using Humanizer;
namespace Victor.CUI
{
    public abstract class Package : Api
    {
        #region Constructors
        public Package(string name, NLUEngine engine, Controller controller, CancellationToken ct, params Package[] subPackages) : base(ct)
        {
            Name = name;
            NLUEngine = engine;
            Controller = controller;
            Intents.Add("info", Info);
            Intents.Add("help", Help);
            Intents.Add("menu", Menu);
            Intents.Add("enable", Enable);
            Intents.Add("disable", Disable);
            Intents.Add("back", Back);
            Intents.Add("page", Page);
            Intents.Add("list", List);
            if (subPackages != null && subPackages.Length > 0)
            {
                SubPackages = subPackages.ToList();
            }
            foreach (var vn in VariableNames)
            {
                Variables.Add(Prefixed(vn), new Variable(Prefixed(vn)));
            }
            foreach (var i in ItemNames)
            {
                Items.Add(Prefixed(i), null);
            }
            foreach (var m in MenuNames)
            {
                Menus.Add(Prefixed(m), null);
            }
        }
        public Package(string name, NLUEngine engine, Controller controller, params Package[] subPackages) : this(name, engine, controller, Ct, subPackages) { }
        #endregion

        #region Properties
        public string Name { get; }

        public NLUEngine NLUEngine { get; }

        public Controller Controller { get; }

        public List<Package> SubPackages { get; } = new List<Package>();

        public Dictionary<string, Action<Intent>> Intents { get; } = new Dictionary<string, Action<Intent>>();

        public abstract string[] VariableNames { get; }

        public Dictionary<string, Variable> Variables { get; } = new Dictionary<string, Variable>();

        public abstract string[] ItemNames { get; }

        public Dictionary<string, Items> Items { get; } = new Dictionary<string, Items>();

        public abstract string[] MenuNames { get; }

        public Dictionary<string, Menu> Menus { get; } = new Dictionary<string, Menu>();
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
        public Stack<Context> Context => Controller.Context;

        public string CurrentContext => Context.Count > 0 ? Context.Peek().Label : "";

        public bool IsItemsContext => CurrentContext.StartsWith("ITEMS_");

        public string GetItemsContext() => CurrentContext.Replace("ITEMS_", "");

        public void SetItemsContext(string name) => Controller.SetContext("ITEMS_" + Prefixed(name));

        public bool IsInputContext => CurrentContext.StartsWith("INPUT_");

        public void SetInputContext(string name) => Controller.SetContext("INPUT_" + Prefixed(name));
        
        public string GetInputContext() => CurrentContext.Replace("INPUT_", "");

        public bool IsMenuContext => CurrentContext.StartsWith("MENU_");

        public string GetMenuContext() => CurrentContext.Replace("MENU_", "");

        public void SetMenuContext(string name, Intent intent = null, Action<Intent> action = null) => Controller.SetContext("MENU_" + Prefixed(name), intent, action);

        public void SetContext(string name, Intent intent = null, Action<Intent> action = null) => Controller.SetContext(Prefixed(name), intent, action);

        #endregion

        #region Variables
        public void GetVariableInput(string variableName, Action<Intent> action = null, Intent intent = null)
        {
            Controller.SetContext("INPUT_" + Prefixed(variableName), intent, action);
            Controller.SetPrompt("|*>");
            if (Controller.DebugEnabled)
            {
                SayInfoLine("Get input for variable {0}.", Prefixed(variableName));
            }
        }

        public bool CanDispatchVariableInput(Context context)
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

        public virtual void DispatchVariableInput(Context context, string input)
        {
            string variableName = context.Label.Replace("INPUT_", "");
            Variables[variableName].Value = input;
            if (Controller.DebugEnabled)
            {
                SayInfoLine("Variable {0} set to to {1}.", variableName, input);
                SayInfoLine("Dispatch variable input {0} to {1}.", input, context.IntentAction.Method.Name);
            }
            Controller.SetDefaultPrompt();
            context.IntentAction.Invoke(context.Intent);
        }

        public string Prefixed(string name) => Name.ToUpper() + "_" + name;

        public string Suffixed(string name) => name + "_" + Name.ToUpper();

        public string GetVar(string name) => Variables[Prefixed(name).ToUpper()].Value;

        public string SetVar(string name, string value) => Variables[Prefixed(name).ToUpper()].Value = value;
        #endregion

        #region Items
        public void SetItems<T>(string name, IEnumerable<T> values) => Items[Prefixed(name).ToUpper()].AddRange(values.Cast<object>());

        public List<T> GetItems<T>(string name) => Items[Prefixed(name).ToUpper()].Cast<T>().ToList();

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
            var items = this.Items[GetItemsContext()];
            var pageSize = items.PageSize;
            var count = items.Count();
            var pages = count / pageSize + 1;
            if (page > pages)
            {
                SayErrorLine("There are only {0} pages available.", pages);
                return;
            }
            else
            {
                int start = (page - 1) * pageSize; 
                int end = start + pageSize;
                if (end > count) end = count;
                if (Controller.DebugEnabled)
                {
                    SayInfoLine("Count: {0}. Page: {1}. Start: {2}. End: {3}", items.Count(), page, start, end);
                }
                SayInfoLine("Items page {0} of {1}.", page, pages);
                for (int i = start; i < end; i++)
                {
                    items.DescriptionHandler.Invoke(i);
                }
                items.Page = page;
            }
        }
        #endregion

        #region Menus
        public bool CanDispatchToMenuSelection()
        {
            if (IsMenuContext)
            {
                return Menus.ContainsKey(GetMenuContext());
            }
            else
            {
                return false;
            }
        }

        public void DispatchToMenuItem(Context context, DateTime time, int i)
        {
            var menu = Menus[GetMenuContext()];
            if (i < 1 || i > menu.Items.Count)
            {
                SayInfoLine("Enter a number between {0} and {1}.", 1, menu.Items.Count);
            }
            else
            {
                Context.Pop();
                menu.Handler.Invoke(i);
            }
        }

        #endregion

        #region Intent parsing

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
                    DescribeItems(result);
                    return true;
                }
                else
                {
                    return ParseIntent(Controller.Context.Peek(), time, input);
                }
            }
			else if (input.ToLower() == "menu")
			{
                    DispatchIntent(null, Menu);
					return true;
			}
			else if (input.ToLower() == "help")
			{
                    DispatchIntent(null, Help);
					return true;
			}
			else if (input.ToLower() == "info")
			{
                    DispatchIntent(null, Info);
					return true;
			}
			else if (input.ToLower() == "exit")
			{
                    DispatchIntent(null, Exit);
					return true;
			}
			else if (input.ToLower() == "enable debug")
			{
                    Controller.DebugEnabled = true;
					SayInfoLine("Debug enabled.");
					return true;
			}
			else if (input.ToLower() == "disable debug")
			{
                    Controller.DebugEnabled = false;
					SayInfoLine("Debug disabled.");
					return true;
			}
            else
            {
                return ParseIntent(Controller.Context.Peek(), time, input);
            }
        }

        public virtual bool ParseIntent(Context context, DateTime time, string input)
        {
            var intent = NLUEngine.GetIntent(input);
            if (Controller.DebugEnabled)
            {
                DebugIntent(intent);
            }
            if (intent.Top.Score < 0.7)
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

        protected Tuple<string, string, string> GetIntentFeaturePackageFunction(Intent intent)
        {
            if (EmptyEntities(intent))
            {
                throw new InvalidOperationException("The intent has no entities.");
            }
            var feature = intent.Entities.FirstOrDefault(e => e.SlotName.EndsWith("feature"))?.Value;
            var package = intent.Entities.FirstOrDefault(e => e.SlotName.EndsWith("package"))?.Value;
            var function = intent.Entities.FirstOrDefault(e => e.SlotName.EndsWith("function"))?.Value;
            return new Tuple<string, string, string>(feature, package, function);

        }

        protected string GetIntentHelpTopic(Intent intent)
        {
            if (EmptyEntities(intent))
            {
                throw new InvalidOperationException("The intent has no entities.");
            }
            return intent.Entities.FirstOrDefault(e => e.SlotName.EndsWith("help_topic"))?.Value;
        }

        protected Tuple<string, string, List<string>> GetIntentCommandItemsParams(Intent intent)
        {
            if (EmptyEntities(intent))
            {
                throw new InvalidOperationException("The intent has no entities.");
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

        protected Tuple<string, string, List<string>> GetIntentTaskCommandObjects(Intent intent)
        {
            if (EmptyEntities(intent))
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

        protected string GetIntentItems(Intent intent)
        {
            ThrowIfNotItems(intent);
            return intent.Entities.Where(i => i.SlotName == "items").First().Value;
        }

        [DebuggerStepThrough]
        protected bool Empty(Intent intent) => intent == null || intent.IsNone;

        [DebuggerStepThrough]
        protected bool EmptyEntities(Intent intent) => Empty(intent) || intent.Entities.Count() == 0;
        
        [DebuggerStepThrough]
        protected void ThrowIfNotItems(Intent intent)
        {
            if (Empty(intent) || intent.Entities.All(e => e.SlotName != "items"))
            {
                throw new InvalidOperationException("Intent has no items.");
            }
        }
        #endregion

        #region Intents
        public virtual void Welcome(Intent intent = null)
        {
            Controller.SetContext("WELCOME_" + this.Name.ToUpper());
        }

        protected abstract void Help(Intent intent = null);

        protected abstract void Info(Intent intent = null);

        protected abstract void Menu(Intent intent = null);

        protected void Exit(Intent intent)
        {
            SayInfoLine("Shutting down...");
            if (Controller.ASREnabled)
            {
                Controller.StopASR();
            }
            Controller.Exit(ExitResult.SUCCESS);
        }

        protected void Enable(Intent intent)
        {
            if (intent.Entities.Length == 0)
            {
                SayErrorLine("Sorry I don't know what you want to enable.");
            }
            else
            {
                switch (intent.Entities.First().Value)
                {
                    case "debug":
                        if (!Controller.DebugEnabled)
                        {
                            Controller.DebugEnabled = true;
                            SayInfoLine("Debug enabled.");
                            break;
                        }
                        else
                        {
                            SayErrorLine("Debug is already enabled.");
                        }
                        break;
                    case "asr":
                        if (!Controller.ASREnabled)
                        {
                            Controller.EnableASR();
                            break;
                        }
                        else
                        {
                            SayErrorLine("ASR is already enabled.");
                        }
                        break;
                    default:
                        SayErrorLine("Sorry I don't know how to enable that.");
                        break;
                }
            }
        }

        protected void Disable(Intent intent)
        {
            if (intent.Entities.Length == 0)
            {
                SayErrorLine("Sorry I don't know what you want to disable.");
            }
            else
            {
                switch (intent.Entities.First().Value)
                {
                    case "debug":
                        if (Controller.DebugEnabled)
                        {
                            Controller.DebugEnabled = false;
                            SayInfoLine("Debug disabled. Commands will be executed.");
                        }
                        else
                        {
                            SayErrorLine("Debug is not enabled.");
                        }
                        break;
                    case "asr":
                        if (Controller.ASREnabled)
                        {
                            Controller.StopASR();
                            SayInfoLine("ASR disabled.");
                        }
                        else
                        {
                            SayErrorLine("ASR is not enabled.");
                        }
                        break;
                    default:
                        SayErrorLine("Sorry I don't know how to enable that.");
                        break;
                }
            }
        }

        protected void Back(Intent intent)
        {
            if (Controller.ActivePackage != Controller.PreviousPackage)
            {
                Controller.ActivePackage = Controller.PreviousPackage;
                Controller.ActivePackage.DispatchIntent(null, Controller.ActivePackage.Menu);
            }
            else
            {
                Controller.Buzz();
            }
        }


        protected void Page(Intent intent)
        {
            if (!IsItemsContext)
            {
                SayErrorLine("Sorry I don't understand what you mean. There is not anything I can page.");
                return;
            }
            var items = Items[GetItemsContext()];
            var input = intent.Input.Trim().ToLower();
            int page;
            if (EmptyEntities(intent))
            {
                if (input == "np" || input.Contains("next"))
                {
                    page = items.Page + 1;
                }
                else if (input == "pp" || intent.Input.Contains("previous"))
                {
                    page = items.Page - 1;
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

        protected virtual void List(Intent intent = null)
        {
            if (EmptyEntities(intent))
            {
                var ctx = GetItemsContext();
                if (Items.ContainsKey(ctx))
                {
                    DispatchIntent(intent, Items[ctx].ListHandler);
                }
                else
                {
                    SayErrorLine("Sorry I don't know how to list that.");
                    DebugIntent(intent);
                }
            }
            else
            {
                var items = GetIntentItems(intent).ToUpper();
                
                if (!Items.ContainsKey(items))
                {
                    SayInfoLine("Sorry I don't know how to list what you are saying: {0}. Say something like {1} or say {2} to see the menu.", intent.Top.Label, "list boards", "menu");
                }
                else
                {
                    Items[items].ListHandler.Invoke(intent);
                }
                /*
                else
                {
                    var c = objects.First();
                    if (ItemsListHandlers.ContainsKey(c))
                    {
                        DispatchIntent(intent, ItemsListHandlers[c]);
                    }
                    else
                    {
                        SayErrorLine("Sorry I don't know how to list that.");
                        DebugIntent(intent);
                    }
                }
                */
            }
        }
        #endregion

        #endregion
    }
}
