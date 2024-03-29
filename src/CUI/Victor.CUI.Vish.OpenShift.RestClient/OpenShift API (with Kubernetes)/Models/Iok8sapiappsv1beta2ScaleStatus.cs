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
    /// ScaleStatus represents the current status of a scale subresource.
    /// </summary>
    public partial class Iok8sapiappsv1beta2ScaleStatus
    {
        /// <summary>
        /// Initializes a new instance of the Iok8sapiappsv1beta2ScaleStatus
        /// class.
        /// </summary>
        public Iok8sapiappsv1beta2ScaleStatus() { }

        /// <summary>
        /// Initializes a new instance of the Iok8sapiappsv1beta2ScaleStatus
        /// class.
        /// </summary>
        public Iok8sapiappsv1beta2ScaleStatus(int replicas, IDictionary<string, string> selector = default(IDictionary<string, string>), string targetSelector = default(string))
        {
            Replicas = replicas;
            Selector = selector;
            TargetSelector = targetSelector;
        }

        /// <summary>
        /// actual number of observed instances of the scaled object.
        /// </summary>
        [JsonProperty(PropertyName = "replicas")]
        public int Replicas { get; set; }

        /// <summary>
        /// label query over pods that should match the replicas count. More
        /// info: http://kubernetes.io/docs/user-guide/labels#label-selectors
        /// </summary>
        [JsonProperty(PropertyName = "selector")]
        public IDictionary<string, string> Selector { get; set; }

        /// <summary>
        /// label selector for pods that should match the replicas count. This
        /// is a serializated version of both map-based and more expressive
        /// set-based selectors. This is done to avoid introspection in the
        /// clients. The string will be in the same format as the query-param
        /// syntax. If the target type only supports map-based selectors,
        /// both this field and map-based selector field are populated. More
        /// info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/labels/#label-selectors
        /// </summary>
        [JsonProperty(PropertyName = "targetSelector")]
        public string TargetSelector { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
        }
    }
}
