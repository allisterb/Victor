using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;
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
        public override string[] VariableNames { get; } = {"BOT_NAME", "BOT_ID", "CONVERSATION_ID"};

        public override string[] MenuNames { get; } = { "AVAILABLE" };

        public override string[] ItemNames { get; } = {};

        public override bool ParseIntent(CUIContext context, DateTime time, string input)
        {
            if (IsBotsContext)
            {
                if (BotCommands.Contains(input.ToLower()))
                {
                    var cmd = input.ToLower();
                    switch (cmd)
                    {
                        case "leave":
                            Controller.SetDefaultPrompt();
                            DispatchIntent(null, Menu);
                            break;
                        default:
                            SayErrorLine($"No handler for command {cmd}.");
                            break;
                    }
                    return true;
                }
                else
                {
                    DispatchBotInput(input);
                }
                return true;
            }
            else
            {
                return base.ParseIntent(context, time, input);
            }
        }

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

        public string[] BotCommands = { "leave" };
        public ICollection<DocumentDescriptor> BotDescriptors { get; }
        #endregion

        #region Methods
        protected void GetBotMenuItem(int i)
        {
            var bot = BotDescriptors.ElementAt(i - 1);
            var bid = bot.ResourceId;
            var cid = Client.BotsPostAsync(Environment8.Unrestricted, bid, null, null).Result;
            SayInfoLineIfDebug("Started conversation with bot {0} with id {1}.", bid, cid);
            SetVar("BOT_NAME", bot.Name);
            SetVar("BOT_ID", bid);
            SetVar("CONVERSATION_ID", cid);
            Controller.SetContext($"BOTS_{bid}_{cid}");
            GetBotOutput();
            SetPrompt("|$>");
        }

        protected void GetBotOutput()
        {
            //var qa = (JArray)outputs["quickReplies"];
            var bid = GetVar("BOT_ID");
            var cid = GetVar("CONVERSATION_ID");
            var convo = Client.ConversationstoreConversationsSimpleAsync(cid, false, true, null).Result;
            var outputs = convo.ConversationOutputs.First();
            var output = ((JArray)(outputs["output"])).ToObject<string[]>();
            var actions = ((JArray)(outputs["actions"])).ToObject<string[]>();
            SayInfoLine(output.Aggregate((s1, s2) => s1 + " " + s2));
            SayInfoLineIfDebug("Bot actions: {0}", actions.Aggregate((s1, s2) => s1 + " " + s2));
        }

        protected void DispatchBotInput(string input)
        {
            var bid = GetVar("BOT_ID");
            var cid = GetVar("CONVERSATION_ID");
            Client.BotsPostAsync(Environment7.Unrestricted, bid, cid, false, true, null, body: new InputData() { Input = input });
            var status = EDDIClient.LastStatusCode;
            if (status != 200)
            {
                SayErrorLine("E.D.D.I server returned code {0}", status);
            }
            else
            {
                GetBotOutput();
            }
            SetPrompt("|$>");
        }
        protected bool IsBotsContext => CurrentContext.StartsWith("BOTS_");
        #endregion

    }
}
