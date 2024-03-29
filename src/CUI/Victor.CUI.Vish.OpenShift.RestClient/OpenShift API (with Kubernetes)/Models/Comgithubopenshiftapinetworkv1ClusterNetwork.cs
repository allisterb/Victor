﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.16.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Victor.CUI.Vish.OpenShift.Client.Models
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Microsoft.Rest;
    using Microsoft.Rest.Serialization;

    /// <summary>
    /// ClusterNetwork describes the cluster network. There is normally only
    /// one object of this type, named "default", which is created by the SDN
    /// network plugin based on the master configuration when the cluster is
    /// brought up for the first time.
    /// </summary>
    public partial class Comgithubopenshiftapinetworkv1ClusterNetwork
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapinetworkv1ClusterNetwork class.
        /// </summary>
        public Comgithubopenshiftapinetworkv1ClusterNetwork() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapinetworkv1ClusterNetwork class.
        /// </summary>
        public Comgithubopenshiftapinetworkv1ClusterNetwork(IList<Comgithubopenshiftapinetworkv1ClusterNetworkEntry> clusterNetworks, string serviceNetwork, string apiVersion = default(string), long? hostsubnetlength = default(long?), string kind = default(string), Iok8sapimachinerypkgapismetav1ObjectMeta metadata = default(Iok8sapimachinerypkgapismetav1ObjectMeta), string network = default(string), string pluginName = default(string))
        {
            ApiVersion = apiVersion;
            ClusterNetworks = clusterNetworks;
            Hostsubnetlength = hostsubnetlength;
            Kind = kind;
            Metadata = metadata;
            Network = network;
            PluginName = pluginName;
            ServiceNetwork = serviceNetwork;
        }

        /// <summary>
        /// APIVersion defines the versioned schema of this representation of
        /// an object. Servers should convert recognized schemas to the
        /// latest internal value, and may reject unrecognized values. More
        /// info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#resources
        /// </summary>
        [JsonProperty(PropertyName = "apiVersion")]
        public string ApiVersion { get; set; }

        /// <summary>
        /// ClusterNetworks is a list of ClusterNetwork objects that defines
        /// the global overlay network's L3 space by specifying a set of CIDR
        /// and netmasks that the SDN can allocate addressed from.
        /// </summary>
        [JsonProperty(PropertyName = "clusterNetworks")]
        public IList<Comgithubopenshiftapinetworkv1ClusterNetworkEntry> ClusterNetworks { get; set; }

        /// <summary>
        /// HostSubnetLength is the number of bits of network to allocate to
        /// each node. eg, 8 would mean that each node would have a /24 slice
        /// of the overlay network for its pods
        /// </summary>
        [JsonProperty(PropertyName = "hostsubnetlength")]
        public long? Hostsubnetlength { get; set; }

        /// <summary>
        /// Kind is a string value representing the REST resource this object
        /// represents. Servers may infer this from the endpoint the client
        /// submits requests to. Cannot be updated. In CamelCase. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#types-kinds
        /// </summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        /// <summary>
        /// Standard object's metadata.
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public Iok8sapimachinerypkgapismetav1ObjectMeta Metadata { get; set; }

        /// <summary>
        /// Network is a CIDR string specifying the global overlay network's
        /// L3 space
        /// </summary>
        [JsonProperty(PropertyName = "network")]
        public string Network { get; set; }

        /// <summary>
        /// PluginName is the name of the network plugin being used
        /// </summary>
        [JsonProperty(PropertyName = "pluginName")]
        public string PluginName { get; set; }

        /// <summary>
        /// ServiceNetwork is the CIDR range that Service IP addresses are
        /// allocated from
        /// </summary>
        [JsonProperty(PropertyName = "serviceNetwork")]
        public string ServiceNetwork { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (ClusterNetworks == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ClusterNetworks");
            }
            if (ServiceNetwork == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ServiceNetwork");
            }
            if (this.ClusterNetworks != null)
            {
                foreach (var element in this.ClusterNetworks)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
            if (this.Metadata != null)
            {
                this.Metadata.Validate();
            }
        }
    }
}
