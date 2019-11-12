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
    /// ImageSourcePath describes a path to be copied from a source image and
    /// its destination within the build directory.
    /// </summary>
    public partial class Comgithubopenshiftapibuildv1ImageSourcePath
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapibuildv1ImageSourcePath class.
        /// </summary>
        public Comgithubopenshiftapibuildv1ImageSourcePath() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapibuildv1ImageSourcePath class.
        /// </summary>
        public Comgithubopenshiftapibuildv1ImageSourcePath(string destinationDir, string sourcePath)
        {
            DestinationDir = destinationDir;
            SourcePath = sourcePath;
        }

        /// <summary>
        /// destinationDir is the relative directory within the build
        /// directory where files copied from the image are placed.
        /// </summary>
        [JsonProperty(PropertyName = "destinationDir")]
        public string DestinationDir { get; set; }

        /// <summary>
        /// sourcePath is the absolute path of the file or directory inside
        /// the image to copy to the build directory.  If the source path
        /// ends in /. then the content of the directory will be copied, but
        /// the directory itself will not be created at the destination.
        /// </summary>
        [JsonProperty(PropertyName = "sourcePath")]
        public string SourcePath { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (DestinationDir == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "DestinationDir");
            }
            if (SourcePath == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "SourcePath");
            }
        }
    }
}