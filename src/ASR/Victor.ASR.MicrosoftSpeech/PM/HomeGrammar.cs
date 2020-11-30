using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;

namespace Victor.ASR.MicrosoftSpeech.PM
{
    class HomeGrammar
    {
        public HomeGrammar()
        {
            Choices home = new Choices();
            home.Add("menu", "help");
            GrammarBuilder gb = new GrammarBuilder();
            //gb.ap
            //Grammar g = new Grammar()
            /*
            Choices ch_StartStopCommands = new Choices();
            ch_StartStopCommands.Add("speech on");
            ch_StartStopCommands.Add("speech off");
            GrammarBuilder gb_StartStop = new GrammarBuilder();
            gb_StartStop.Append(ch_StartStopCommands);
            Grammar g_StartStop = new Grammar(gb_StartStop);
            Choices menu = new Choices();
            SemanticResultValue itemSRV;
            itemSRV = new SemanticResultValue("I E", "explorer");
            item.Add(itemSRV);
            itemSRV = new SemanticResultValue("explorer", "explorer");
            item.Add(itemSRV);
            itemSRV = new SemanticResultValue("firefox", "firefox");
            item.Add(itemSRV);
            itemSRV = new SemanticResultValue("mozilla", "firefox");
            item.Add(itemSRV);
            itemSRV = new SemanticResultValue("chrome", "chrome");
            item.Add(itemSRV);
            itemSRV = new SemanticResultValue("google chrome", "chrome");
            item.Add(itemSRV);
            SemanticResultKey itemSemKey = new SemanticResultKey("item", item);
            */
        }
    }
}
