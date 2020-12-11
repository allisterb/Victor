using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Rest;
using Newtonsoft.Json;

using Victor.CUI.Vish.OpenShift.Client;
using Victor.CUI.Vish.OpenShift.Client.Models;

[assembly: InternalsVisibleTo("Victor.Tests.Vish")]
namespace Victor
{
    public class OpenShift : CUIPackage
    {
        #region Constructors
        public OpenShift(CUIController controller, CancellationToken ct) : base("OpenShift", new SnipsNLUEngine(Path.Combine("Engines", "openshift")),  controller, ct)
        {
            Intents.Add("list", List);
            Intents.Add("page", Page);
            MenuHandlers["OPENSHIFT_OBJECTS"] = GetOpenShiftMenuSelection;
            MenuIndexes["OPENSHIFT_OBJECTS"] = 5;
            ItemsDescriptionHandlers["OPENSHIFT_PODS"] = DescribePods;
            ItemsDescriptionHandlers["OPENSHIFT_PROJECTS"] = DescribeProjects;
            ItemsDescriptionHandlers["OPENSHIFT_BUILDS"] = DescribeBuilds;
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
                SayErrorLine("I could not determine your OpenShift API URL. Please ensure the value exists in your config.json configuration file or as the environment variable {0}.", "CUI_VISH_OPENSHIFT_URL");
            }
            else if (string.IsNullOrEmpty(ApiToken))
            {
                SayErrorLine("I could not determine your OpenShift service API token. Please ensure the value exists in your config.json configuration file or as the environment variable {0}.", "CUI_VISH_OPENSHIFT_TOKENs");
            }
        }

        public OpenShift(CUIController controller) : this(controller, Ct) {}
        #endregion

        #region Overriden members
        public override string[] VariableNames { get; } = { "PROJECT" };
         
        public override string[] MenuNames { get; } = { "OBJECTS" };

        public override string[] ItemNames { get; } = { "PROJECTS", "PODS", "BUILD_CONFIGS", "BUILDS", "DEPLOYMENT_CONFIGS" };

        #region Intents

        public override void Help(Intent intent)
        {
            if (Empty(intent))
            {
                SayInfoLine("This package allows you to administer an OpenShift cluser. Say something like {0} to get help with a specific topic.", "help pods");
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
            SetMenuContext("OBJECTS");
            SayInfoLine("Red Hat OpenShift");
            SayInfoLine("1 {0}", "Projects");
            SayInfoLine("2 {0}", "Pods");
            SayInfoLine("3 {0}", "Builds");
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
            ThrowIfNotInitialized();
            var projects = GetItems<Comgithubopenshiftapiprojectv1ProjectList>("PROJECTS");
            if (projects == null)
            {
                SetItems("PROJECTS", projects = projects = FetchProjects());
                ItemsCurrentPage[Prefixed("PROJECTS")] = 1;
            }
            SetItemsContext("PROJECTS");
            if (!Empty(intent) && intent.Top.Label == "list")
            {
                DescribeItems(GetItemsCurrentPage("PROJECTS"));
            }

        }

        public void Pods(Intent intent )
        {
            ThrowIfNotInitialized();
            if (string.IsNullOrEmpty(GetVar("PROJECT")))
            {
                SayWarningLine("The {0} variable is not set. Enter the name of the OpenShift project.", Prefixed("PROJECT"));
                GetVariableInput("PROJECT", Pods, intent);
                return;
            }
            else
            {
                var pods = GetItems<Iok8sapicorev1PodList>("PODS");
                if ( pods == null)
                {
                    SetItems("PODS", pods = FetchPods(GetVar("PROJECT")));
                    ItemsCurrentPage[Prefixed("PODS")] = 1;
                }
                SetItemsContext("PODS");
                if (!Empty(intent) && intent.Top.Label == "list")
                {
                    DescribeItems(GetItemsCurrentPage("PODS"));
                }
            }

        }

        public void Builds(Intent intent)
        {
            ThrowIfNotInitialized();
            if (string.IsNullOrEmpty(GetVar("PROJECT")))
            {
                SayWarningLine("The {0} variable is not set. Enter the name of the OpenShift project.", Prefixed("PROJECT"));
                GetVariableInput("PROJECT", Builds, intent);
                return;
            }
            else
            {
                var builds = GetItems<Comgithubopenshiftapibuildv1BuildList>("BUILDS");
                if (builds == null)
                {
                    SetItems("BUILDS", builds = FetchBuilds(GetVar("BUILDS")));
                    ItemsCurrentPage[Prefixed("BUILDS")] = 1;
                }
                SetItemsContext("BUILDS");
                if (!Empty(intent) && intent.Top.Label == "list")
                {
                    DescribeItems(GetItemsCurrentPage("BUILDS"));
                }
            }

        }
        public void List(Intent intent)
        {
            if (ObjectEmpty(intent))
            {
                switch (GetItemsContext())
                {
                    case "PODS":
                        DispatchIntent(intent, Pods);
                        break;
                    case "PROJECTS":
                        DispatchIntent(intent, Projects);
                        break;
                    case "BUILDS":
                        DispatchIntent(intent, Builds);
                        break;
                    default:
                        SayErrorLine("Sorry I don't know how to list that.");
                        DebugIntent(intent);
                        break;
                }
            }
            else
            {
                var (task, command, objects) = GetIntentTaskCommandObjects(intent);
                if (objects.Count == 0 )
                {
                    SayInfoLine("Sorry I don't know how to list what you are saying: {0}. Say something like {1} or say {2} to see the OpenShift menu.", intent.Top.Label, "list pods", "menu");
                }
                else
                {
                    var openshift_object = objects.First();
                    switch(openshift_object)
                    {
                        case "project":
                            DispatchIntent(intent, Projects);
                            break;
                        case "pod":
                            DispatchIntent(intent, Pods);
                            break;
                        case "build":
                            DispatchIntent(intent, Builds);
                            break;
                        default:
                            SayInfoLine("Sorry I don't know how to list what you are saying. Say something like {0} or say {1} to see the OpenShift menu.", "list pods", "menu");
                            break;
                    }
                }
            }
        }

        #endregion

        #region Menus
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
                case 2:
                    DispatchIntent(null, Builds);
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        #endregion

        #region Items
        protected void DescribePods(int page, CUIPackage package)
        {
            var oc = (OpenShift)package;
            var pageSize = oc.ItemsPageSize["OPENSHIFT_PODS"];
            var pods = oc.GetItems<Iok8sapicorev1PodList>("PODS");
            var count = pods.Items.Count;
            var pages = count / pageSize + 1;
            if (page > pages)
            {
                SayErrorLine("There are only {0} pages available.", pages);
                return;
            }
            else
            {
                int start = (page - 1) * oc.ItemsPageSize["OPENSHIFT_PODS"];
                int end = start + oc.ItemsPageSize["OPENSHIFT_PODS"];
                if (end > count) end = count;
                if (Controller.DebugEnabled)
                {
                    SayInfoLine("Count: {0}. Page: {1}. Start: {2}. End: {3}", pods.Items.Count, page, start, end);
                }
                SayInfoLine("Pods page {0} of {1}.", page, pages);
                for (int i = start; i < end; i++)
                {
                    var pod = pods.Items[i];
                    oc.SayInfoLine("{0}. Name: {1}, Namespace: {2}, Labels: {3}, Status: {4}.", i + 1, pod.Metadata.Name, pod.Metadata.NamespaceProperty, 
                        pod.Metadata.Labels?.Select(kv => kv.Key + "=" + kv.Value).Aggregate((s1, s2) => s1 + " " + s2) ?? "{}",
                        pod.Status.Phase);
                }
                oc.ItemsCurrentPage["OPENSHIFT_PODS"] = page;
            }

        }

        protected void DescribeProjects(int page, CUIPackage package)
        {
            var oc = (OpenShift)package;
            var pageSize = oc.ItemsPageSize["OPENSHIFT_PROJECTS"];
            var projects = oc.GetItems<Comgithubopenshiftapiprojectv1ProjectList>("PROJECTS");

            var count = projects.Items.Count;
            var pages = count / pageSize + 1;
            if (page > pages)
            {
                SayErrorLine("There are only {0} pages available.", pages);
                return;
            }
            else
            {
                int start = (page - 1) * oc.ItemsPageSize["OPENSHIFT_PROJECTS"];
                int end = start + oc.ItemsPageSize["OPENSHIFT_PROJECTS"];
                if (end > count) end = count;
                if (Controller.DebugEnabled)
                {
                    SayInfoLine("Count: {0}. Page: {1}. Start: {2}. End: {3}", projects.Items.Count, page, start, end);
                }
                SayInfoLine("Projects page {0} of {1}.", page, pages);
                for (int i = start; i < end; i++)
                {
                    var project = projects.Items[i];
                    oc.SayInfoLine("{0}. Name: {1}, Status: {2}, Requester: {3}, Labels: {4}.", i + 1, project.Metadata.Name, project.Status.Phase, 
                        project.Metadata.Annotations.First(a => a.Key.ToLower().Contains("requester")).Value,
                        project.Metadata.Labels?.Select(kv => kv.Key + "=" + kv.Value).Aggregate((s1, s2) => s1 + " " + s2) ?? "{}");
                }
                oc.ItemsCurrentPage["OPENSHIFT_PROJECTS"] = page;
            }
        }

        protected void DescribeBuilds(int page, CUIPackage package)
        {
            var oc = (OpenShift)package;
            var pageSize = oc.ItemsPageSize["OPENSHIFT_BUILDS"];
            var builds = oc.GetItems<Comgithubopenshiftapibuildv1BuildList>("BUILDS");
            var count = builds.Items.Count;
            var pages = count / pageSize + 1;
            if (page > pages)
            {
                SayErrorLine("There are only {0} pages available.", pages);
                return;
            }
            else
            {
                int start = (page - 1) * oc.ItemsPageSize["OPENSHIFT_BUILDS"];
                int end = start + oc.ItemsPageSize["OPENSHIFT_BUILDS"];
                if (end > count) end = count;
                if (Controller.DebugEnabled)
                {
                    SayInfoLine("Count: {0}, Page: {1}, Start: {2}, End: {3}", builds.Items.Count, page, start, end);
                }
                SayInfoLine("Builds page {0} of {1}.", page, pages);
                for (int i = start; i < end; i++)
                {
                    var build = builds.Items[i];
                    oc.SayInfoLine("{0}. Name: {1}, Namespace: {2}, Labels: {3}, Status: {4}.", i + 1, build.Metadata.Name, build.Metadata.NamespaceProperty,
                        build.Metadata.Labels?.Select(kv => kv.Key + "=" + kv.Value).Aggregate((s1, s2) => s1 + " " + s2) ?? "{}",
                        build.Status.Phase);
                }
                oc.ItemsCurrentPage["OPENSHIFT_BUILDS"] = page;
            }

        }
        #endregion

        #region OpenShift API
        internal Iok8sapicorev1PodList FetchPods(string ns, string label = null)
        {
            ThrowIfNotInitialized();
            SayInfoLine("Fetching pods for project {0}.", Variables["OPENSHIFT_PROJECT"]);
            Controller.StartBeeper();
            var pods = Client.ListCoreV1NamespacedPod(namespaceParameter: ns, labelSelector: label);
            Controller.StopBeeper();
            SayInfoLine("Fetched {0} pods.", pods.Items.Count);
            return pods;
        }

        internal Comgithubopenshiftapiprojectv1ProjectList FetchProjects()
        {
            ThrowIfNotInitialized();
            SayInfoLine("Fetching projects for your cluster...");
            Controller.StartBeeper();
            var projects = Client.ListProject();
            SetItems("PROJECTS", projects);
            Controller.StopBeeper();
            SayInfoLine("Fetched {0} projects.", projects.Items.Count);
            return projects;
        }

        internal Comgithubopenshiftapibuildv1BuildList FetchBuilds(string ns, string label = null)
        {
            ThrowIfNotInitialized();
            SayInfoLine("Fetching builds for your cluster...");
            Controller.StartBeeper();
            var builds = Client.ListBuildOpenshiftIoV1NamespacedBuild(namespaceParameter: ns, labelSelector: label);
            SetItems("PROJECTS", builds);
            Controller.StopBeeper();
            SayInfoLine("Fetched {0} builds.", builds.Items.Count);
            return builds;
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
