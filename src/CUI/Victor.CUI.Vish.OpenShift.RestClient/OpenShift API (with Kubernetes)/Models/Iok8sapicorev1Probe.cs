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
    /// Probe describes a health check to be performed against a container to
    /// determine whether it is alive or ready to receive traffic.
    /// </summary>
    public partial class Iok8sapicorev1Probe
    {
        /// <summary>
        /// Initializes a new instance of the Iok8sapicorev1Probe class.
        /// </summary>
        public Iok8sapicorev1Probe() { }

        /// <summary>
        /// Initializes a new instance of the Iok8sapicorev1Probe class.
        /// </summary>
        public Iok8sapicorev1Probe(Iok8sapicorev1ExecAction exec = default(Iok8sapicorev1ExecAction), int? failureThreshold = default(int?), Iok8sapicorev1HTTPGetAction httpGet = default(Iok8sapicorev1HTTPGetAction), int? initialDelaySeconds = default(int?), int? periodSeconds = default(int?), int? successThreshold = default(int?), Iok8sapicorev1TCPSocketAction tcpSocket = default(Iok8sapicorev1TCPSocketAction), int? timeoutSeconds = default(int?))
        {
            Exec = exec;
            FailureThreshold = failureThreshold;
            HttpGet = httpGet;
            InitialDelaySeconds = initialDelaySeconds;
            PeriodSeconds = periodSeconds;
            SuccessThreshold = successThreshold;
            TcpSocket = tcpSocket;
            TimeoutSeconds = timeoutSeconds;
        }

        /// <summary>
        /// One and only one of the following should be specified. Exec
        /// specifies the action to take.
        /// </summary>
        [JsonProperty(PropertyName = "exec")]
        public Iok8sapicorev1ExecAction Exec { get; set; }

        /// <summary>
        /// Minimum consecutive failures for the probe to be considered failed
        /// after having succeeded. Defaults to 3. Minimum value is 1.
        /// </summary>
        [JsonProperty(PropertyName = "failureThreshold")]
        public int? FailureThreshold { get; set; }

        /// <summary>
        /// HTTPGet specifies the http request to perform.
        /// </summary>
        [JsonProperty(PropertyName = "httpGet")]
        public Iok8sapicorev1HTTPGetAction HttpGet { get; set; }

        /// <summary>
        /// Number of seconds after the container has started before liveness
        /// probes are initiated. More info:
        /// https://kubernetes.io/docs/concepts/workloads/pods/pod-lifecycle#container-probes
        /// </summary>
        [JsonProperty(PropertyName = "initialDelaySeconds")]
        public int? InitialDelaySeconds { get; set; }

        /// <summary>
        /// How often (in seconds) to perform the probe. Default to 10
        /// seconds. Minimum value is 1.
        /// </summary>
        [JsonProperty(PropertyName = "periodSeconds")]
        public int? PeriodSeconds { get; set; }

        /// <summary>
        /// Minimum consecutive successes for the probe to be considered
        /// successful after having failed. Defaults to 1. Must be 1 for
        /// liveness. Minimum value is 1.
        /// </summary>
        [JsonProperty(PropertyName = "successThreshold")]
        public int? SuccessThreshold { get; set; }

        /// <summary>
        /// TCPSocket specifies an action involving a TCP port. TCP hooks not
        /// yet supported
        /// </summary>
        [JsonProperty(PropertyName = "tcpSocket")]
        public Iok8sapicorev1TCPSocketAction TcpSocket { get; set; }

        /// <summary>
        /// Number of seconds after which the probe times out. Defaults to 1
        /// second. Minimum value is 1. More info:
        /// https://kubernetes.io/docs/concepts/workloads/pods/pod-lifecycle#container-probes
        /// </summary>
        [JsonProperty(PropertyName = "timeoutSeconds")]
        public int? TimeoutSeconds { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (this.HttpGet != null)
            {
                this.HttpGet.Validate();
            }
            if (this.TcpSocket != null)
            {
                this.TcpSocket.Validate();
            }
        }
    }
}
