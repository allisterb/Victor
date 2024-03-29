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
    /// Maps a string key to a path within a volume.
    /// </summary>
    public partial class Iok8sapicorev1KeyToPath
    {
        /// <summary>
        /// Initializes a new instance of the Iok8sapicorev1KeyToPath class.
        /// </summary>
        public Iok8sapicorev1KeyToPath() { }

        /// <summary>
        /// Initializes a new instance of the Iok8sapicorev1KeyToPath class.
        /// </summary>
        public Iok8sapicorev1KeyToPath(string key, string path, int? mode = default(int?))
        {
            Key = key;
            Mode = mode;
            Path = path;
        }

        /// <summary>
        /// The key to project.
        /// </summary>
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }

        /// <summary>
        /// Optional: mode bits to use on this file, must be a value between 0
        /// and 0777. If not specified, the volume defaultMode will be used.
        /// This might be in conflict with other options that affect the file
        /// mode, like fsGroup, and the result can be other mode bits set.
        /// </summary>
        [JsonProperty(PropertyName = "mode")]
        public int? Mode { get; set; }

        /// <summary>
        /// The relative path of the file to map the key to. May not be an
        /// absolute path. May not contain the path element '..'. May not
        /// start with the string '..'.
        /// </summary>
        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (Key == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Key");
            }
            if (Path == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Path");
            }
        }
    }
}
