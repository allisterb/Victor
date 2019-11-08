using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;

namespace Victor
{
    public abstract class CUIModule : Api
    {
        #region Constructors
        public CUIModule(string name, CUIPackage package, CUIController controller, CancellationToken ct) : base(ct)
        {
            Name = name;
            Package = package;
            Controller = controller;
        }
        public CUIModule(string name, CUIPackage package, CUIController controller) : this(name, package, controller, Ct) {}
        #endregion

        #region Properties
        public string Name { get; }

        public CUIPackage Package { get; internal set; }

        public CUIController Controller { get; }

        protected Dictionary<string, Action<object>> CommandHandlers { get; } = new Dictionary<string, Action<object>>();

        protected Dictionary<string, Action<int>> MenuHandlers { get; } = new Dictionary<string, Action<int>>();

        protected Dictionary<string, int> MenuIndexes { get; } = new Dictionary<string, int>();

        protected Dictionary<string, List<object>> MenuItems { get; } = new Dictionary<string, List<object>>();

        
        protected Dictionary<string, int> SelectedMenuItem = new Dictionary<string, int>();
        #endregion

        #region Methods
        public Stack<CUIContext> Context => Controller.Context;

        public void SayInfo(string template, params object[] args) => Controller.SayInfo(template, args);

        public void SayInfoLine(string template, params object[] args) => Controller.SayInfoLine(template, args);

        public void SayErrorLine(string template, params object[] args) => Controller.SayErrorLine(template, args);

        public void DispatchToMenuItem(string c, int i)
        {
            if (i < 0 || i > MenuIndexes[c])
            {
                SayInfoLine("Enter a number between {0} and {1}.", 1, MenuIndexes[c]);
                return;
            }
            else
            {
                Context.Pop();
                MenuHandlers[c].Invoke(i);
            }
        }
        #endregion
    }
}
