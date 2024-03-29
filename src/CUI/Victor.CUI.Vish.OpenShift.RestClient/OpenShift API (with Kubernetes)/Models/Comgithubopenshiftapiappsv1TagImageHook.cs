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
    /// TagImageHook is a request to tag the image in a particular container
    /// onto an ImageStreamTag.
    /// </summary>
    public partial class Comgithubopenshiftapiappsv1TagImageHook
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapiappsv1TagImageHook class.
        /// </summary>
        public Comgithubopenshiftapiappsv1TagImageHook() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapiappsv1TagImageHook class.
        /// </summary>
        public Comgithubopenshiftapiappsv1TagImageHook(string containerName, Iok8sapicorev1ObjectReference to)
        {
            ContainerName = containerName;
            To = to;
        }

        /// <summary>
        /// ContainerName is the name of a container in the deployment config
        /// whose image value will be used as the source of the tag. If there
        /// is only a single container this value will be defaulted to the
        /// name of that container.
        /// </summary>
        [JsonProperty(PropertyName = "containerName")]
        public string ContainerName { get; set; }

        /// <summary>
        /// To is the target ImageStreamTag to set the container's image onto.
        /// </summary>
        [JsonProperty(PropertyName = "to")]
        public Iok8sapicorev1ObjectReference To { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (ContainerName == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ContainerName");
            }
            if (To == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "To");
            }
        }
    }
}
