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
    /// ServiceStatus represents the current status of a service.
    /// </summary>
    public partial class Iok8sapicorev1ServiceStatus
    {
        /// <summary>
        /// Initializes a new instance of the Iok8sapicorev1ServiceStatus
        /// class.
        /// </summary>
        public Iok8sapicorev1ServiceStatus() { }

        /// <summary>
        /// Initializes a new instance of the Iok8sapicorev1ServiceStatus
        /// class.
        /// </summary>
        public Iok8sapicorev1ServiceStatus(Iok8sapicorev1LoadBalancerStatus loadBalancer = default(Iok8sapicorev1LoadBalancerStatus))
        {
            LoadBalancer = loadBalancer;
        }

        /// <summary>
        /// LoadBalancer contains the current status of the load-balancer, if
        /// one is present.
        /// </summary>
        [JsonProperty(PropertyName = "loadBalancer")]
        public Iok8sapicorev1LoadBalancerStatus LoadBalancer { get; set; }

    }
}
