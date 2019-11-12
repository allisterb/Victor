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
    /// ContainerState holds a possible state of container. Only one of its
    /// members may be specified. If none of them is specified, the default
    /// one is ContainerStateWaiting.
    /// </summary>
    public partial class Iok8sapicorev1ContainerState
    {
        /// <summary>
        /// Initializes a new instance of the Iok8sapicorev1ContainerState
        /// class.
        /// </summary>
        public Iok8sapicorev1ContainerState() { }

        /// <summary>
        /// Initializes a new instance of the Iok8sapicorev1ContainerState
        /// class.
        /// </summary>
        public Iok8sapicorev1ContainerState(Iok8sapicorev1ContainerStateRunning running = default(Iok8sapicorev1ContainerStateRunning), Iok8sapicorev1ContainerStateTerminated terminated = default(Iok8sapicorev1ContainerStateTerminated), Iok8sapicorev1ContainerStateWaiting waiting = default(Iok8sapicorev1ContainerStateWaiting))
        {
            Running = running;
            Terminated = terminated;
            Waiting = waiting;
        }

        /// <summary>
        /// Details about a running container
        /// </summary>
        [JsonProperty(PropertyName = "running")]
        public Iok8sapicorev1ContainerStateRunning Running { get; set; }

        /// <summary>
        /// Details about a terminated container
        /// </summary>
        [JsonProperty(PropertyName = "terminated")]
        public Iok8sapicorev1ContainerStateTerminated Terminated { get; set; }

        /// <summary>
        /// Details about a waiting container
        /// </summary>
        [JsonProperty(PropertyName = "waiting")]
        public Iok8sapicorev1ContainerStateWaiting Waiting { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (this.Terminated != null)
            {
                this.Terminated.Validate();
            }
        }
    }
}