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
            if (subPackages != null && subPackages.Length > 0)
            {
                SubPackages = subPackages.ToList();
            }
        }
        public CUIPackage(string name, NLUEngine engine, CUIController controller, params CUIPackage[] subPackages) : this(name, engine, controller, Ct, subPackages) {}
        #endregion

        #region Properties
        public string Name { get; }

        public NLUEngine NLUEngine { get; }

        public CUIController Controller { get; }

        public List<CUIPackage> SubPackages { get; } = new List<CUIPackage>();

        protected Dictionary<string, Action<object>> CommandHandlers { get; } = new Dictionary<string, Action<object>>();

        protected Dictionary<string, Action<int>> MenuHandlers { get; } = new Dictionary<string, Action<int>>();

        protected Dictionary<string, int> MenuIndexes { get; } = new Dictionary<string, int>();

        protected Dictionary<string, List<object>> MenuItems { get; } = new Dictionary<string, List<object>>();


        protected Dictionary<string, int> SelectedMenuItem = new Dictionary<string, int>();
        #endregion

        #region Methods
        public Stack<CUIContext> Context => Controller.Context;

        public virtual bool HandleInput(DateTime time, string input)
        {
            ThrowIfNotInitialized();
            if (Int32.TryParse(input, out int result) && Controller.Context.Peek().Label.StartsWith("MENU") && CanDispatchMenuSelection(Controller.Context.Peek()))
            {
                return DispatchToMenuItem(Controller.Context.Peek(), DateTime.Now, result);

            }
            else
            {
                return ParseIntent(Controller.Context.Peek(), time, input);
            }
        }

        public void DebugIntent(Intent intent)
        {
            SayInfoLine("Context: {0}, Package: {1}, Intent: {2} Score: {3}.", Context.PeekIfNotEmpty().Label, this.Name, intent.Top.Label, intent.Top.Score);
            foreach (var e in intent.Entities)
            {
                SayInfoLine("Entity:{0} Value:{1}.", e.Entity, e.Value);
            }
        }

        public bool CanDispatchMenuSelection(CUIContext context)
        {
            string label = context.Label.Replace("MENU_", "");
            return MenuHandlers.ContainsKey(label);

        }
        public virtual bool DispatchToMenuItem(CUIContext context, DateTime time, int i)
        {
            if (i < 0 || i > MenuIndexes[context.Label])
            {
                SayInfoLine("Enter a number between {0} and {1}.", 1, MenuIndexes[Context.Peek().Label]);
                return true;
            }
            else
            {
                MenuHandlers[context.Label].Invoke(i);
                return true;
            }
        }

        public void DispatchIntent(Intent intent, Action<Intent> action)
        {
            action(intent);
            Context.Peek().SetIntentAction(intent, action);
        }

        public void DispatchToMenuItem(string c, int i)
        {
            if (i < 0 || i > MenuIndexes[c])
            {
                SayInfoLine("Enter a number between {0} and {1}.", 1, MenuIndexes[c]);
                return;
            }
            else
            { 
                MenuHandlers[c].Invoke(i);
            }
        }

        protected void SayInfo(string template, params object[] args) => Controller.SayInfo(template, args);

        protected void SayInfoLine(string template, params object[] args) => Controller.SayInfoLine(template, args);

        protected void SayErrorLine(string template, params object[] args) => Controller.SayErrorLine("Error: " + template, args);


        #endregion

        #region Abstract methods
        public abstract bool ParseIntent(CUIContext context, DateTime time, string input);

        public abstract void Menu(Intent intent);
        #endregion
    }
}
