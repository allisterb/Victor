using System;
using System.Threading;
namespace Victor
{
    public class RedHatNews : Api
    {
        #region Constructors
        public RedHatNews(CancellationToken ct) : base(ct)
        {

        }

        public RedHatNews() : this(Ct) { }
        #endregion

        #region Properties
        public Uri BaseUrl { get; } = new Uri("https://www.redhat.com/en/about/newsroom");
        public string[] Regions { get; } = { "Global", "Europe", "Middle East", "Africa", "Asia Pacific", "North America", "Latin America" };

        public string[] Solutions { get; } = { "Infrastructure", "Application integration and development", "Infrastructure-as-a-Service", "Platform-as-a-Service" };

        public string[] Topics { get; } = {"Partners", "Customer success", "Community to enterprise", "Events", "OpenStack", "Awards", "Containers", "Training", "Open source", "Security", "Cloud",
            "Executives", "DevOps", "Consulting", "Management", "Cloud management", "Corporate", "xPaaS", "Big data", "Kubernetes", "Linux",
            "Migration", "Development", "Open source communities", "Automation", "Emerging technology", "Internet of Things", "Mobile", "Virtualization"};
        
        //public string[] Products { get; } = { "Red Hat Enterprise Linux", "Red Hat Virtualization", "Red Hat JBoss "}
        #endregion
        
        #region Methods
        public void GetNewsRoomResults(string region = null, string solution = null, string topic = null)
        {

        }
        #endregion
    }
}
