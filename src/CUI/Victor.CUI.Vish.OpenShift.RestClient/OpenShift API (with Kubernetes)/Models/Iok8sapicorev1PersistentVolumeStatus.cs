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
    /// PersistentVolumeStatus is the current status of a persistent volume.
    /// </summary>
    public partial class Iok8sapicorev1PersistentVolumeStatus
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapicorev1PersistentVolumeStatus class.
        /// </summary>
        public Iok8sapicorev1PersistentVolumeStatus() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapicorev1PersistentVolumeStatus class.
        /// </summary>
        public Iok8sapicorev1PersistentVolumeStatus(string message = default(string), string phase = default(string), string reason = default(string))
        {
            Message = message;
            Phase = phase;
            Reason = reason;
        }

        /// <summary>
        /// A human-readable message indicating details about why the volume
        /// is in this state.
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// Phase indicates if a volume is available, bound to a claim, or
        /// released by a claim. More info:
        /// https://kubernetes.io/docs/concepts/storage/persistent-volumes#phase
        /// </summary>
        [JsonProperty(PropertyName = "phase")]
        public string Phase { get; set; }

        /// <summary>
        /// Reason is a brief CamelCase string that describes any failure and
        /// is meant for machine parsing and tidy display in the CLI.
        /// </summary>
        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }

    }
}
