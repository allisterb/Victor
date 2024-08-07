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
    /// ResourceRequirements describes the compute resource requirements.
    /// </summary>
    public partial class Iok8sapicorev1ResourceRequirements
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapicorev1ResourceRequirements class.
        /// </summary>
        public Iok8sapicorev1ResourceRequirements() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapicorev1ResourceRequirements class.
        /// </summary>
        public Iok8sapicorev1ResourceRequirements(IDictionary<string, string> limits = default(IDictionary<string, string>), IDictionary<string, string> requests = default(IDictionary<string, string>))
        {
            Limits = limits;
            Requests = requests;
        }

        /// <summary>
        /// Limits describes the maximum amount of compute resources allowed.
        /// More info:
        /// https://kubernetes.io/docs/concepts/configuration/manage-compute-resources-container/
        /// </summary>
        [JsonProperty(PropertyName = "limits")]
        public IDictionary<string, string> Limits { get; set; }

        /// <summary>
        /// Requests describes the minimum amount of compute resources
        /// required. If Requests is omitted for a container, it defaults to
        /// Limits if that is explicitly specified, otherwise to an
        /// implementation-defined value. More info:
        /// https://kubernetes.io/docs/concepts/configuration/manage-compute-resources-container/
        /// </summary>
        [JsonProperty(PropertyName = "requests")]
        public IDictionary<string, string> Requests { get; set; }

    }
}
