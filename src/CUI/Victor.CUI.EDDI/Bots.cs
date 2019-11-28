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
                else if (Int32.TryParse(input, out int result) && QuickReplies != null && (result - 1) < QuickReplies.Length)
                {
                    DispatchBotInput(QuickReplies[result - 1]);
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
                SayInfoLine("{0}. {1}", ++i, b.Name);
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
        
        public string[] LastOutput { get; set; }

        public Dictionary<string, IDictionary<string, object>> ConversationStart { get; set; } = new Dictionary<string, IDictionary<string, object>>();
        
        public string[] QuickReplies { get; set; }
        #endregion

        #region Methods
        protected void GetBotMenuItem(int i)
        {
            var bot = BotDescriptors.ElementAt(i - 1);
            var bid = bot.ResourceId;
            var cid = Client.BotsPostAsync(Environment8.Test, bid, null, null).Result;
            SayInfoLineIfDebug("Started conversation with bot {0} with id {1}.", bid, cid);
            SetVar("BOT_NAME", bot.Name);
            SetVar("BOT_ID", bid);
            SetVar("CONVERSATION_ID", cid);
            Controller.SetContext($"BOTS_{bid}_{cid}");
            if (!string.IsNullOrEmpty(bot.Description))
            {
                SayInfoLine(bot.Description);
            }
            GetBotOutputs();
            SetPrompt("|$>");
        }

        protected void GetBotOutputs()
        {
            var bid = GetVar("BOT_ID");
            var cid = GetVar("CONVERSATION_ID");
            var convo = Client.ConversationstoreConversationsSimpleAsync(cid, false, true, null).Result;
            if (!ConversationStart.ContainsKey(cid))
            {
                ConversationStart[cid] = convo.ConversationOutputs.First();
            }
            var outputs = convo.ConversationOutputs.Last();
            var quickReplies = outputs.ContainsKey("quickReplies") ? (JArray) outputs["quickReplies"] : null;
            var input = outputs.ContainsKey("input") ? (string)(outputs["input"]) : "";
            var expressions = outputs.ContainsKey("expressions") ? (string)(outputs["expressions"]) : "";
            var intents = outputs.ContainsKey("intents") ? ((JArray)(outputs["intents"])).ToObject<string[]>() : Array.Empty<string>();
            var output = outputs.ContainsKey("output") ? ((JArray)(outputs["output"])).ToObject<string[]>() : Array.Empty<string>();
            var actions = outputs.ContainsKey("actions") ? ((JArray)(outputs["actions"])).ToObject<string[]>() : Array.Empty<string>();
            var httpCalls = outputs.ContainsKey("httpCalls") ? ((JObject)(outputs["httpCalls"])) : null;

            if (output.Length > 0)
            {
                foreach(var o in output)
                {
                    SayInfoLine(o);
                }
            }
            if (quickReplies != null)
            {
                QuickReplies = new string[quickReplies.Count];
                int i = 0;
                foreach(dynamic qr in quickReplies)
                {
                    SayInfoLine("{0}. {1}", i + 1, qr.value);
                    QuickReplies[i++] = qr.value;
                }
            }
            else
            {
                QuickReplies = null;
            }
            if (output.Length > 0)
            {
                LastOutput = output;
            }
            if (actions.Length > 0)
            {
                SayInfoLineIfDebug("Bot actions: {0}", actions.Aggregate((s1, s2) => s1 + " " + s2));
            }
            if (intents.Length > 0)
            {
                SayInfoLineIfDebug("Bot intents: {0}", intents.Aggregate((s1, s2) => s1 + " " + s2));
            }
            if(!string.IsNullOrEmpty(expressions))
            {
                SayInfoLineIfDebug("Bot expressions: {0}.", expressions);
            }
            if (httpCalls != null)
            {
                SayInfoLineIfDebug("HTTP calls {0}", httpCalls.ToString());
            }
        }

        protected void DispatchBotInput(string input)
        {
            var bid = GetVar("BOT_ID");
            var cid = GetVar("CONVERSATION_ID");
            Client.BotsPostAsync(Environment7.Test, bid, cid, false, true, null, body: new InputData() { Input = input }).Wait();
            var status = EDDIClient.LastStatusCode;
            if (status != 200)
            {
                SayErrorLine("E.D.D.I server returned code {0}", status);
            }
            else
            {
                GetBotOutputs();
            }
            SetPrompt("|$>");
        }
        protected bool IsBotsContext => CurrentContext.StartsWith("BOTS_");
        #endregion

    }
}
