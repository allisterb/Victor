﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Azure.AI.FormRecognizer.DocumentAnalysis;

using Victor.NLU;
using Victor.Vision;

namespace Victor.CUI.DU
{
    public class DUHome : Package
    {
        #region Constructors
        public DUHome(Controller controller) : base("DOCUMENTS", new SnipsNLUEngine(Path.Combine(Api.AssemblyDirectory.FullName, "Engines", "DU")), controller)
        {
            Features = Menus[Prefixed("FEATURES")] = new Menu(Prefixed("FEATURES"), GetFeaturesMenuItem, "Open", "Scan", "Ask");
            DocType = Menus[Prefixed("DOC_TYPE")] = new Menu(Prefixed("DOC_TYPE"), GetDocTypeMenuItem, "Invoice", "Receipt", "W-2 Tax Form", "Business Card", "Training Manual");
            DocAnalysis = Menus[Prefixed("DOC_ANALYSIS")] = new Menu(Prefixed("DOC_ANALYSIS"), GetDocAnalysisMenuItem, "Lines", "Fields", "Tables", "Layout", "Done");
            DocLines = Items[Prefixed("DOC_LINES")] = new Items(Prefixed("DOC_LINES"), typeof(DocumentLine), ListFields, DescribeField);
            DocFields = Items[Prefixed("DOC_FIELDS")] = new Items(Prefixed("DOC_FIELDS"), typeof(KeyValuePair<string, DocumentField>), ListFields, DescribeField);
            DocTables = Items[Prefixed("DOC_TABLES")] = new Items(Prefixed("DOC_TABLES"), typeof(DocumentTable), ListFields, DescribeField);
            Recognizer = new AzureFormRecognizer(this.Controller, this.CancellationToken);
            QnA = new AzureQnA(this.Controller, this.CancellationToken);
            
            if (!NLUEngine.Initialized)
            {
                SayErrorLine("NLU engine for package {0} did not initialize. Exiting.", this.Name);
                Controller.Exit(ExitResult.UNKNOWN_ERROR);
            }
            
            if (!Recognizer.Initialized)
            {
                SayErrorLine("Azure Form Recognizer not initialized.");
                Controller.Exit(ExitResult.UNKNOWN_ERROR);
            }
            
            if (!QnA.Initialized)
            {
                SayErrorLine("Azure QnA not initialized.");
                Controller.Exit(ExitResult.UNKNOWN_ERROR);
            }
            SayInfoLine("Fetching knowledge bases...");
            KBs = QnA.GetKnowledgebases();
            SayInfoLine("Done.");
            DocKBs = Menus[Prefixed("KBS")] = new Menu(Prefixed("KBS"), GetDocKBMenuItem, KBs);
            Initialized = true;
        }
        #endregion

        #region Properties
        public Menu Features { get; }

        public Menu DocType { get; }

        public Menu DocAnalysis { get; }

        public Menu DocKBs { get; }

        public Items DocLines { get; }

        public Items DocFields { get; }

        public Items DocTables { get; }

        public AzureFormRecognizer Recognizer {get; }

        public AzureQnA QnA { get; }

        public List<string> KBs { get; }
        #endregion

        #region Overriden members

        #region UI
        public override string[] VariableNames { get; } = { "FILE_NAME", "CURRENT_DOC", "CURRENT_DOC_TYPE", "KBS", "KB_ID", "KB_QUESTION" };

        public override string[] MenuNames { get; } = { "FEATURES", "DOC_TYPE", "DOC_ANALYSIS" };

        public override string[] ItemNames { get; } = { "DOC_LINES", "DOC_FIELDS", "DOC_TABLES" };
        #endregion

        #region Intents
        public override void Welcome(Intent intent = null)
        {
            base.Welcome(intent);
            SayInfoLine("Welcome to Victor Document Understanding.");
            SayInfoLine("Say {0} at any time to get help. Say {1} at any to show the menu for the current feature or task. Say {2} to exit the application. Say {3} to get general information about the Victor program.", "help", "menu", "exit", "info");
        }

        protected override void Info(Intent intent = null)
        {
            var context = CurrentContext;
            if (EmptyEntities(intent))
            {
                SayInfoLine("Victor Document Understanding is an auditory user interface designed for people with vision disabilities to help with understanding, navigating and querying structured business documents.");

            }
            else
            {
                var (feature, package, function) = GetIntentFeaturePackageFunction(intent);
               
                if (!string.IsNullOrEmpty(feature))
                {
                    feature = new string(feature.Where(c => Char.IsLetterOrDigit(c)).ToArray());
                    switch (feature)
                    {
                        case null:
                        case "":
                        case "this":
                            switch (context)
                            {
                                case "WELCOME_HOME":
                                    Info(null);
                                    break;
                                case "MENU_HOME_PACKAGES":
                                    Help(null);
                                    break;
                                default:
                                    SayErrorLine("Unknown home context: {0}.", context);
                                    break;
                            }
                            break;
                        case "nlu":
                            SayInfoLine("Victor CX uses natural language understanding to understand a user's intent and the entities that are part of that intent.");
                            SayInfoLine("A user does not have to enter an exact phrase or command but can express their intent using natural language and different phrases and synonymns.");
                            SayInfoLine("You can enable debug mode by entering {0} or {1}. For each user input this will print information about the intent and entities extracted by the NLU engine.", "enable debug", "debug on");
                            break;
                        default:
                            SayInfoLine("No info so far for {0}.", feature);
                            break;
                    }
                }
            }

        }

        protected override void Help(Intent intent)
        {
            var context = CurrentContext;
            if (EmptyEntities(intent))
            {
                switch (context)
                {
                    case "WELCOME_DOCUMENTS":
                        SayInfoLine("Victor features can be accessed via menus or using text commands. Say {0} to access the main menu.", "menu");
                        break;
                    case "MENU_DOCUMENTS_FEATURES":
                        SayInfoLine("This is the main menu. Enter the number of the feature you want to use. Enter {0} to open a local document for analysis, {1} to scan a document using a connected scanner, {2} to query a document knowledge base.", 1, 2, 3);
                        break;
                    case "MENU_DOCUMENTS_DOC_ANALYSIS":
                        SayInfoLine("This is the document analysis menu. It shows the different kinds of document analysis data available. Enter the number of the document analysis data you want to see. You can also use a text command to retrieve document analysis data or to query a knowledge base.");
                        break;
                    default:
                        SayErrorLine("Unknown HELP context: {0}.", context);
                        SayInfoLine("Say {0} to enable debug mode.", "enable debug");
                        break;
                }
            }
            else if (GetIntentHelpTopic(intent) != null)
            {
                var help_topic = GetIntentHelpTopic(intent);
                switch(help_topic)
                {
                    case "tax":
                        SayInfoLine("Querying Tax Documents knowledge base...");
                        Controller.StartBeeper();
                        var ans = QnA.GetAnswer("TaxDocumentsKB", intent.Input);
                        Controller.StopBeeper();
                        SayInfoLine(ans);
                        break;
                }
            }
            else {
                var (feature, package, function) = GetIntentFeaturePackageFunction(intent);

                if (!string.IsNullOrEmpty(feature))
                {
                    switch (feature)
                    {
                        case "this":
                            Help(null);
                            break;
                        case "nlu":
                            SayInfoLine("Victor SM uses natural language understanding to understand a user's intent and the entities that are part of that intent.");
                            SayInfoLine("A user does not have to enter an exact phrase or the exact command syntax but can express their intent using natural language and different phrases and synonymns.");
                            SayInfoLine("You can enable debug mode by entering {0} or {1}. For each user input this will print information about the intent and entities extracted by the NLU engine.", "enable debug", "debug on");
                            break;
                        case "asr":
                            SayInfoLine("Victor SM can use automatic speech recognition as an input method instead of or in addition to a keyboard or character input device.");
                            SayInfoLine("To enable ASR say {0} or {1}.", "asr on", "enable asr");
                            SayInfoLine("ASR quality may vary depending on your mic and environment. To test how ASR works with your hardware and environment run {0} from a command line.", "victor asr");
                            break;
                        case "package":
                            SayInfoLine("There are 3 package categories: {0}, {1} and {2}.", "Vish", "Services", "Bots.");
                            SayInfoLine("Vish is the Voice Interactive Shell with packages to help you manage and administer your computer or network or technology products like Red Hat OpenShift.");
                            SayInfoLine("Services let you access services like news and product information that do not require much interactivity.");
                            SayInfoLine("Bots are conversational agents that help you with tasks like filling out complex forms or completing complex multi-step processes and workflows that require a lot of interactivity.");
                            SayInfoLine("Use the {0} command to bring up the packages menu. You can also jump to a package category by entering the category name like {1}.\n", "menu", "vish");
                            break;
                        case "menu":
                            SayInfoLine("Menus provide an accessible way to break up or categorize a multi-step task. Enter the number corresponding to the task or category you would like to select next.");
                            break;
                        default:
                            SayInfoLine("No help so far for feature {0}.", feature);
                            break;
                    }
                }
                else if (!string.IsNullOrEmpty(package))
                {
                    switch (package)
                    {
                        case "vish":
                            SayInfoLine("Vish is the Voice Interactive Shell with packages to help you manage and administer your computer or network or technology products like Red Hat OpenShift.");
                            break;
                        case "services":
                            SayInfoLine("Services let you access services like news and product information that do not require much interactivity.");
                            break;
                        case "bot":
                            SayInfoLine("Bots are conversational agents that help you with tasks like filling out complex forms or completing complex multi-step processes and workflows that require a lot of interactivity.");
                            SayInfoLine("You can administer Victor CX bots by running {0} from the command-line.", "victor cui");
                            break;
                        default:
                            SayInfoLine("No help so far for package {0}.", package);
                            break;
                    }
                }
            }
        }

        protected override void Menu(Intent intent)
        {
            switch(CurrentContext)
            {
                case "WELCOME_DOCUMENTS":
                case "DOCUMENTS_MAIN":
                    SetMenuContext("FEATURES");
                    SayInfoLine("Select a feature to use:");
                    SayInfoLine("1: {0}", "Open.");
                    SayInfoLine("2: {0}", "Scan.");
                    SayInfoLine("3: {0}", "Ask.");
                    break;
                case "DOCUMENTS_DOC_TYPE":
                    SetMenuContext("DOC_TYPE");
                    SayInfoLine("Select a document type:");
                    SayInfoLine("1: {0}", "Invoice.");
                    SayInfoLine("2: {0}", "Receipt.");
                    SayInfoLine("3: {0}", "W-2 Tax Form.");
                    SayInfoLine("4: {0}", "Business Card.");
                    SayInfoLine("5: {0}", "Training Manual.");
                    break;
                case "DOCUMENTS_DOC_ANALYSIS":
                    SetMenuContext("DOC_ANALYSIS");
                    SayInfoLine("Select document items to read:");
                    SayInfoLine("1: {0}", "Lines.");
                    SayInfoLine("2: {0}", "Fields.");
                    SayInfoLine("3: {0}", "Tables.");
                    SayInfoLine("4: {0}", "Layout.");
                    SayInfoLine("5: {0}", "Done.");
                    SayInfoLine("Or enter text to analyze the document or query a knowledge base.");
                    break;
                case "DOCUMENTS_KBS":
                    SetMenuContext("KBS");
                    SayInfoLine("Select knowledge base:");
                    for (int i = 0; i < KBs.Count; i++)
                    {
                        SayInfoLine((i + 1).ToString() + ": {0}.", KBs[i]);
                    }
                    break;
                default:
                    SayInfoLineIfDebug("Unknown controller context: {0}.", CurrentContext);
                    SayErrorLine("Sorry, I don't understand what you mean.");
                    break;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Intents
        public void Open(Intent intent)
        {
            Context.Pop();
            var filename = GetVar("FILE_NAME");
            if (filename.ToUpper() == "CANCEL")
            {
                Context.Pop();
                DispatchIntent(null, Menu);
            }
            else
            {
                SayInfoLineIfDebug($"File to open is: {filename}");
                if (!File.Exists(filename))
                {
                    SayErrorLine($"Sorry, I couldn't find the file {filename}. Try entering it again or say 'cancel' to cancel this operation.");
                    GetVariableInput("FILE_NAME", Open);
                }
                else
                {
                    SetContext("DOC_TYPE", null);
                    DispatchIntent(null, Menu);
                }
            }
            
        }

        public void Ask(Intent intent)
        {
            Context.Pop();
            var kbid = GetVar("KB_ID");
            var kbquestion = GetVar("KB_QUESTION");
            var ans = QnA.GetAnswer(kbid, kbquestion);

        }
        #endregion

        #region Menu Items
        protected void GetFeaturesMenuItem(int i)
        {
            switch(i - 1)
            {
                case 0:
                    SayInfoLine("Enter the file name to open.");
                    GetVariableInput("FILE_NAME", Open);
                    break;
                case 2:
                    Context.Pop();
                    SetContext("KBS");
                    DispatchIntent(null, Menu);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        protected void GetDocTypeMenuItem(int i)
        {
            var filename = GetVar("FILE_NAME");
            string modellid = "";
            switch (i - 1)
            {
                case 0:
                    modellid = "prebuilt-invoice";
                    SetVar("CURRENT_DOC_TYPE", "INVOICE");
                    break;

                case 1:
                    modellid = "prebuilt-receipt";
                    SetVar("CURRENT_DOC_TYPE", "RECEIPT");
                    break;

                case 2:
                    modellid = "prebuilt-tax.us.w2";
                    SetVar("CURRENT_DOC_TYPE", "W2 TAX FORM");
                    break;

                case 3:
                    modellid = "prebuilt-idDocument";
                    SetVar("CURRENT_DOC_TYPE", "IDENTITY DOCUMENT");
                    break;

                case 4:
                    modellid = "prebuilt-idDocument";
                    SetVar("CURRENT_DOC_TYPE", "BUSINESS CARD");
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }

            Controller.StartBeeper();
            string docType = GetVar("CURRENT_DOC_TYPE");
            SayInfoLine("Analyzing document as {0}...", docType);
            var r = Recognizer.AnalyzeDocument(modellid, filename);

            SetItems("DOC_FIELDS",
                r.Documents.First().Fields
                .Where(f => !string.IsNullOrEmpty(f.Value.Content) && f.Value.Confidence.HasValue && f.Value.Confidence.Value >= 0.5)
                .ToArray());
            if (r.Tables.Count > 0)
            {
                SetItems("DOC_TABLES", r.Tables.ToArray());
            }
            else
            {
                SetItems("DOC_TABLES", Array.Empty<DocumentTable>());
            }

            if (r.Pages.Count > 0)
            {
                SetItems("DOC_LINES", r.Pages.First().Lines.ToArray());
            }
            else
            {
                SetItems("DOC_LINES", Array.Empty<DocumentLine>());
            }

            Controller.StopBeeper();
            Context.Pop();
            SetContext("DOC_ANALYSIS", null);
            DispatchIntent(null, Menu);
        }

        protected void GetDocAnalysisMenuItem(int i)
        {
            var filename = GetVar("FILE_NAME");
            var docType = GetVar("CURRENT_DOC_TYPE");
            switch (i - 1)
            {
                case 0:
                    SayInfoLine("Printing {0} lines...", docType);
                    var lines = GetItems<DocumentLine>("DOC_LINES");
                    foreach (var l in lines)
                    {
                        SayInfoLine(l.Content);
                    }
                    break;

                case 1:
                    SayInfoLine("Printing {0} fields...", docType);
                    var fields = GetItems<KeyValuePair<string, DocumentField>>("DOC_FIELDS");
                    foreach (var f in fields)
                    {
                        if ((f.Value.ValueType != DocumentFieldType.Dictionary) && (f.Value.ValueType != DocumentFieldType.List))
                        {
                            SayInfoLine($"{f.Key}: {f.Value.Content}.");
                        }
                        else if (f.Value.ValueType == DocumentFieldType.List)
                        {
                            var fieldsList = f.Value.AsList();
                            SayInfoLine($"{f.Key}:");
                            foreach (var fi in fieldsList)
                            {
                                SayInfoLine(fi.Content);
                            }
                        }
                        else if (f.Value.ValueType == DocumentFieldType.Dictionary)
                        {
                            var fieldsList = f.Value.AsDictionary();
                            SayInfoLine($"{f.Key}:");
                            foreach (var fi in fieldsList)
                            {
                                SayInfoLine($"{fi.Key}: {fi.Value.Content}.");
                            }
                        }
                    }
                    break;
                
                case 4:
                    Context.Pop(); // Doc Analysis
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
            SayInfoLine("");
            DispatchIntent(null, Menu);
        }

        protected void GetDocKBMenuItem(int i)
        {
            var kbid = KBs[i - 1];
            SetVar("KB_ID", kbid);
            SayInfoLine($"Enter your question for knowledge base {kbid}.");
            GetVariableInput("KB_QUESTION", Ask);

        }

        protected void ListFields(Intent intent)
        {
            ThrowIfNotInitialized();
        }

        protected void DescribeField(int index)
        {
            //var a = DocItems.Get<Doc>(index);
            //SayInfoLine("Name: {0}.", a.Name);
        }
            
        #endregion

        #endregion
    }
}
