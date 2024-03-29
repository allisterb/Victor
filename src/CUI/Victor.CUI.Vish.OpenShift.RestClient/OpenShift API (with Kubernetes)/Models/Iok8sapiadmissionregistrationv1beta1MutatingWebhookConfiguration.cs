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
    /// MutatingWebhookConfiguration describes the configuration of and
    /// admission webhook that accept or reject and may change the object.
    /// </summary>
    public partial class Iok8sapiadmissionregistrationv1beta1MutatingWebhookConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapiadmissionregistrationv1beta1MutatingWebhookConfiguration
        /// class.
        /// </summary>
        public Iok8sapiadmissionregistrationv1beta1MutatingWebhookConfiguration() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapiadmissionregistrationv1beta1MutatingWebhookConfiguration
        /// class.
        /// </summary>
        public Iok8sapiadmissionregistrationv1beta1MutatingWebhookConfiguration(string apiVersion = default(string), string kind = default(string), Iok8sapimachinerypkgapismetav1ObjectMeta metadata = default(Iok8sapimachinerypkgapismetav1ObjectMeta), IList<Iok8sapiadmissionregistrationv1beta1Webhook> webhooks = default(IList<Iok8sapiadmissionregistrationv1beta1Webhook>))
        {
            ApiVersion = apiVersion;
            Kind = kind;
            Metadata = metadata;
            Webhooks = webhooks;
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
        /// Kind is a string value representing the REST resource this object
        /// represents. Servers may infer this from the endpoint the client
        /// submits requests to. Cannot be updated. In CamelCase. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#types-kinds
        /// </summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        /// <summary>
        /// Standard object metadata; More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#metadata.
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public Iok8sapimachinerypkgapismetav1ObjectMeta Metadata { get; set; }

        /// <summary>
        /// Webhooks is a list of webhooks and the affected resources and
        /// operations.
        /// </summary>
        [JsonProperty(PropertyName = "webhooks")]
        public IList<Iok8sapiadmissionregistrationv1beta1Webhook> Webhooks { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (this.Metadata != null)
            {
                this.Metadata.Validate();
            }
            if (this.Webhooks != null)
            {
                foreach (var element in this.Webhooks)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
        }
    }
}
