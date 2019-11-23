using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Victor
{
    public class Bots : CUIPackage
    {
        #region Constructors
        public Bots(CUIController controller, CancellationToken ct) : base("Bots", new SnipsNLUEngine(Path.Combine("Engines", "vish")), controller, ct)
        {
            Client = new EDDIClient(Api.Config("CUI_EDDI_SERVER_URL"), HttpClient);
            BotDescriptors = Client.BotstoreBotsDescriptorsGetAsync(null, null, null).Result;
            MenuHandlers["BOTS_AVAILABLE"] = GetBotMenuItem;
            MenuIndexes["BOTS_AVAILABLE"] = BotDescriptors.Count;
            Initialized = NLUEngine.Initialized && BotDescriptors != null && BotDescriptors.Count > 0;
        }

        public Bots(CUIController controller) : this(controller, Api.Ct) { }
        #endregion

        #region Overriden members
        public override string[] VariableNames { get; } = {};

        public override string[] MenuNames { get; } = { "AVAILABLE" };

        public override string[] ItemNames { get; } = {};

        public override void Menu(Intent intent)
        {
            SetMenuContext("AVAILABLE", intent, Menu);
            SayInfoLine("Select the bot you want to talk to.");
            int i = 0;
            foreach(var b in BotDescriptors)
            {
                SayInfoLine("{0} {1}", ++i, b.Name);
            }
        }

        public override void Info(Intent intent = null)
        {
            throw new NotImplementedException();
        }

        public override void Help(Intent intent = null)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Properties
        public EDDIClient Client { get; }
        
        public ICollection<DocumentDescriptor> BotDescriptors { get; }
        #endregion

        #region Methods
        protected void GetBotMenuItem(int i)
        {
            switch (i - 1)
            {
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        
        #endregion

    }
}
