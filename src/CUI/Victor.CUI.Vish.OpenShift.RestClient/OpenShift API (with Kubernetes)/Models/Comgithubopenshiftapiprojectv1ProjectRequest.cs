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
    /// ProjecRequest is the set of options necessary to fully qualify a
    /// project request
    /// </summary>
    public partial class Comgithubopenshiftapiprojectv1ProjectRequest
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapiprojectv1ProjectRequest class.
        /// </summary>
        public Comgithubopenshiftapiprojectv1ProjectRequest() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapiprojectv1ProjectRequest class.
        /// </summary>
        public Comgithubopenshiftapiprojectv1ProjectRequest(string apiVersion = default(string), string description = default(string), string displayName = default(string), string kind = default(string), Iok8sapimachinerypkgapismetav1ObjectMeta metadata = default(Iok8sapimachinerypkgapismetav1ObjectMeta))
        {
            ApiVersion = apiVersion;
            Description = description;
            DisplayName = displayName;
            Kind = kind;
            Metadata = metadata;
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
        /// Description is the description to apply to a project
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// DisplayName is the display name to apply to a project
        /// </summary>
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }

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
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (this.Metadata != null)
            {
                this.Metadata.Validate();
            }
        }
    }
}
