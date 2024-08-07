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
    /// ImageLayer represents a single layer of the image. Some images may
    /// have multiple layers. Some may have none.
    /// </summary>
    public partial class Comgithubopenshiftapiimagev1ImageLayer
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapiimagev1ImageLayer class.
        /// </summary>
        public Comgithubopenshiftapiimagev1ImageLayer() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapiimagev1ImageLayer class.
        /// </summary>
        public Comgithubopenshiftapiimagev1ImageLayer(string mediaType, string name, long size)
        {
            MediaType = mediaType;
            Name = name;
            Size = size;
        }

        /// <summary>
        /// MediaType of the referenced object.
        /// </summary>
        [JsonProperty(PropertyName = "mediaType")]
        public string MediaType { get; set; }

        /// <summary>
        /// Name of the layer as defined by the underlying store.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Size of the layer in bytes as defined by the underlying store.
        /// </summary>
        [JsonProperty(PropertyName = "size")]
        public long Size { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (MediaType == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "MediaType");
            }
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
        }
    }
}
