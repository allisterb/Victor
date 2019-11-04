namespace Victor.CUI.RHDM.KIE
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class ListContainersResponse
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("result")]
        public ListContainerResult Result { get; set; }
    }

    public partial class ListContainerResult
    {
        [JsonProperty("kie-containers")]
        public KieContainers KieContainers { get; set; }
    }

    public partial class KieContainers
    {
        [JsonProperty("kie-container")]
        public KieContainer[] KieContainer { get; set; }
    }

    public partial class KieContainer
    {
        [JsonProperty("container-id")]
        public string ContainerId { get; set; }

        [JsonProperty("release-id")]
        public ReleaseId ReleaseId { get; set; }

        [JsonProperty("resolved-release-id")]
        public ReleaseId ResolvedReleaseId { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("scanner")]
        public Scanner Scanner { get; set; }

        [JsonProperty("config-items")]
        public KieContainerConfigItem[] ConfigItems { get; set; }

        [JsonProperty("messages")]
        public Message[] Messages { get; set; }

        [JsonProperty("container-alias")]
        public string ContainerAlias { get; set; }
    }

    public partial class KieContainerConfigItem
    {
        [JsonProperty("itemName")]
        public string ItemName { get; set; }

        [JsonProperty("itemValue")]
        public string ItemValue { get; set; }

        [JsonProperty("itemType")]
        public string ItemType { get; set; }
    }

    public partial class Message
    {
        [JsonProperty("severity")]
        public string Severity { get; set; }

        [JsonProperty("timestamp")]
        public Timestamp Timestamp { get; set; }

        [JsonProperty("content")]
        public string[] Content { get; set; }
    }

    public partial class Timestamp
    {
        [JsonProperty("java.util.Date")]
        public long JavaUtilDate { get; set; }
    }

    public partial class ReleaseId
    {
        [JsonProperty("group-id")]
        public string GroupId { get; set; }

        [JsonProperty("artifact-id")]
        public string ArtifactId { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public partial class Scanner
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("poll-interval")]
        public object PollInterval { get; set; }
    }
}
