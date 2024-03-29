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
    /// ImageImportStatus describes the result of an image import.
    /// </summary>
    public partial class Comgithubopenshiftapiimagev1ImageImportStatus
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapiimagev1ImageImportStatus class.
        /// </summary>
        public Comgithubopenshiftapiimagev1ImageImportStatus() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapiimagev1ImageImportStatus class.
        /// </summary>
        public Comgithubopenshiftapiimagev1ImageImportStatus(Iok8sapimachinerypkgapismetav1Status status, Comgithubopenshiftapiimagev1Image image = default(Comgithubopenshiftapiimagev1Image), string tag = default(string))
        {
            Image = image;
            Status = status;
            Tag = tag;
        }

        /// <summary>
        /// Image is the metadata of that image, if the image was located
        /// </summary>
        [JsonProperty(PropertyName = "image")]
        public Comgithubopenshiftapiimagev1Image Image { get; set; }

        /// <summary>
        /// Status is the status of the image import, including errors
        /// encountered while retrieving the image
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public Iok8sapimachinerypkgapismetav1Status Status { get; set; }

        /// <summary>
        /// Tag is the tag this image was located under, if any
        /// </summary>
        [JsonProperty(PropertyName = "tag")]
        public string Tag { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (Status == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Status");
            }
            if (this.Image != null)
            {
                this.Image.Validate();
            }
        }
    }
}
