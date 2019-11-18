using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rest;
using Newtonsoft.Json;

using Victor.CUI.Vish.OpenShift.Client;
using Victor.CUI.Vish.OpenShift.Client.Models;

namespace Victor
{
    public class OpenShift : CUIPackage
    {
        #region Constructors
        public OpenShift(CUIController controller, CancellationToken ct) : base("OpenShift", new SnipsNLUEngine(Path.Combine("Engines", "openshift")),  controller, ct)
        {
            Intents.Add("list", List);
            MenuHandlers["OPENSHIFT_OBJECTS"] = GetOpenShiftMenuSelection;
            MenuIndexes["OPENSHIFT_OBJECTS"] = 5;
            ItemsDescriptionHandlers["OPENSHIFT_PODS"] = DescribePods;
            ApiUrl = Config("CUI_VISH_OPENSHIFT_URL");
            ApiToken = Config("CUI_VISH_OPENSHIFT_TOKEN");
            if (!string.IsNullOrEmpty(ApiToken) && !string.IsNullOrEmpty(ApiUrl))
            {
                var handler = new HttpClientHandler {};
                Client = new OpenShiftAPIwithKubernetes(new Uri(ApiUrl), new TokenCredentials(ApiToken), handler); 
                Initialized = true;
            }
            else if (string.IsNullOrEmpty(ApiUrl))
            {
                SayErrorLine("I could not determine your OpenShift API URL. Please ensure the item exists in your config.json configuration file.");
            }
            else if (string.IsNullOrEmpty(ApiToken))
            {
                SayErrorLine("I could not determine your OpenShift service API token. Please ensure the item exists in your config.json configuration file");
            }
        }

        public OpenShift(CUIController controller) : this(controller, Ct) {}
        #endregion

        #region Overriden members
        public override string[] VariableNames { get; } = { "PROJECT" };
         
        public override string[] MenuNames { get; } = { "OBJECTS","PROJECTS", "PODS", "BUILD_CONFIGS", "BUILDS", "DEPLOYMENT_CONFIGS" };

        public override string[] ItemNames { get; } = { "PROJECTS", "PODS", "BUILD_CONFIGS", "BUILDS", "DEPLOYMENT_CONFIGS" };

        #region Intents

        public override void Help(Intent intent)
        {
            if (intent == null || intent.Entities.Length == 0)
            {
                SayInfoLine("This package allows you to administer an OpenShift cluser. Say something like {0} to get help with a specific topic.", "help podss");
            }
            else if (!intent.Entities.Any(e => e.Entity == "openshift_object"))
            {
                SayInfoLine("Sorry I did not understand what you said. You can say something like {0} or {1} to get background information on an OpenShift topic.", "info pods", "what are projects");
            }
            else
            {
                var entity = intent.Entities.First(e => e.Entity == "openshift_object");

                switch (entity.Value)
                {
                    case "pod":
                        SayInfoLine("Pod is a collection of containers that can run on a host. This resource is created by clients and scheduled onto hosts.");
                        break;
                    default:
                        break;

                }
            }
        }

        public override void Info(Intent intent)
        {
            if (intent.Entities.Length == 0)
            {
                SayInfoLine("OpenShift is an open source container application platform by Red Hat based on the Kubernetes container orchestrator for enterprise app development.");
            }
            else if (!intent.Entities.Any(e => e.Entity == "openshift_object"))
            {
                SayInfoLine("Sorry I did not understand what you said. You can say something like {0} or {1} to get background information on an OpenShift topic.", "info pods", "what are projects");
            }
            else
            {
                var entity = intent.Entities.First(e => e.Entity == "openshift_object");

                switch (entity.Value)
                {
                    case "pod":
                        SayInfoLine("Pod is a collection of containers that can run on a host. This resource is created by clients and scheduled onto hosts.");
                        break;
                    default:
                        break;

                }
            }
        }

        public override void Menu(Intent intent)
        {
            Controller.SetContext("MENU_OPENSHIFT_OBJECTS");
            SayInfoLine("Red Hat OpenShift");
            SayInfoLine("1 {0}", "Projects");
            SayInfoLine("2 {0}", "Pods");
        }
        #endregion

        #endregion

        #region Properties
        protected string ApiToken { get; }

        protected string ApiUrl { get; }
        
        protected OpenShiftAPIwithKubernetes Client { get; }
        #endregion

        #region Methods

        #region Intents
        
        public void Projects(Intent intent)
        {
            SetContext("PROJECTS");
            SayInfoLine("Fetching projects for your cluster...");
            Controller.StartBeeper();
            var projects = FetchProjects();
            Controller.StopBeeper();
            SetItem("PROJECTS", projects);
            SayInfoLine("Fetched {0} projects.", projects.Items.Count);
        }

        public void Pods(Intent intent)
        {
            SetContext("PODS");
            if (string.IsNullOrEmpty(GetVar("PROJECT")))
            {
                SayWarningLine("The {0} variable is not set. Enter the name of the OpenShift project.", Prefixed("PROJECT"));
                GetInput("PROJECT", Pods, intent);
                return;
            }
            else
            {
                FetchPods(GetVar("PROJECT"));
            }
        }

        public void List(Intent intent)
        {
            var (task, command, objects) = GetIntentTaskCommandObjects(intent);
        }        
        #endregion

        protected void GetOpenShiftMenuSelection(int i)
        {
            switch (i - 1)
            {
                case 0:
                    DispatchIntent(null, Projects);
                    break;
                case 1:
                    DispatchIntent(null, Pods);
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }

        protected void DescribePods(int page, CUIPackage package)
        {
            var oc = (OpenShift)package;
            var pods = oc.GetItem<Iok8sapicorev1PodList>("PODS");
            int start = (page - 1)* oc.ItemsPageSize["OPENSHIFT_PODS"];
            int end = start + oc.ItemsPageSize["OPENSHIFT_PODS"];
            if (end > pods.Items.Count) end = pods.Items.Count;
            for(int i = start; i < end; i++)
            {
                var pod = pods.Items[i];
                oc.SayInfoLine("{0} Name: {1}", i, pod.Metadata.Name);
            }
        }
        #region OpenShift API
        public Iok8sapicorev1PodList FetchPods(string ns, string label = null)
        {
            ThrowIfNotInitialized();
            SayInfoLine("Fetching pods for project {0}.", Variables["OPENSHIFT_PROJECT"]);
            Controller.StartBeeper();
            var pods = Client.ListCoreV1NamespacedPod(namespaceParameter: ns, labelSelector: label);
            Controller.StopBeeper();
            SetItem("PODS", pods);
            ItemsCurrentPage["OPENSHIFT_PODS"] = pods.Items.Count / 10 + 1;
            SayInfoLine("Fetched {0} pods for project {1}.", pods.Items.Count, Variables["OPENSHIFT_PROJECT"]);
            return pods;
        }

        public Comgithubopenshiftapiprojectv1ProjectList FetchProjects()
        {
            ThrowIfNotInitialized();
            return Client.ListProject();
            //Client.in

        }

        public Comgithubopenshiftapiroutev1RouteList GetRoutes(string ns, string label = null)
        {
            ThrowIfNotInitialized();
            return Client.ListRouteOpenshiftIoV1NamespacedRoute(namespaceParameter: ns, labelSelector: label);

        }

        public Comgithubopenshiftapibuildv1BuildList GetBuilds(string ns, string label = null)
        {
            ThrowIfNotInitialized();
            return Client.ListBuildOpenshiftIoV1NamespacedBuild(namespaceParameter: ns, labelSelector: label);
        }

        public Comgithubopenshiftapibuildv1BuildConfigList GetBuildConfigs(string ns, string label = null)
        {
            ThrowIfNotInitialized();
            return Client.ListBuildOpenshiftIoV1NamespacedBuildConfig(namespaceParameter: ns, labelSelector: label);
        }
        #endregion

        #endregion
    }
}
