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
    /// DeploymentConfigRollback provides the input to rollback generation.
    /// </summary>
    public partial class Comgithubopenshiftapiappsv1DeploymentConfigRollback
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapiappsv1DeploymentConfigRollback class.
        /// </summary>
        public Comgithubopenshiftapiappsv1DeploymentConfigRollback() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapiappsv1DeploymentConfigRollback class.
        /// </summary>
        public Comgithubopenshiftapiappsv1DeploymentConfigRollback(string name, Comgithubopenshiftapiappsv1DeploymentConfigRollbackSpec spec, string apiVersion = default(string), string kind = default(string), IDictionary<string, string> updatedAnnotations = default(IDictionary<string, string>))
        {
            ApiVersion = apiVersion;
            Kind = kind;
            Name = name;
            Spec = spec;
            UpdatedAnnotations = updatedAnnotations;
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
        /// Name of the deployment config that will be rolled back.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Spec defines the options to rollback generation.
        /// </summary>
        [JsonProperty(PropertyName = "spec")]
        public Comgithubopenshiftapiappsv1DeploymentConfigRollbackSpec Spec { get; set; }

        /// <summary>
        /// UpdatedAnnotations is a set of new annotations that will be added
        /// in the deployment config.
        /// </summary>
        [JsonProperty(PropertyName = "updatedAnnotations")]
        public IDictionary<string, string> UpdatedAnnotations { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
            if (Spec == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Spec");
            }
            if (this.Spec != null)
            {
                this.Spec.Validate();
            }
        }
    }
}
