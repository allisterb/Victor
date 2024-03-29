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
    /// PodsMetricStatus indicates the current value of a metric describing
    /// each pod in the current scale target (for example,
    /// transactions-processed-per-second).
    /// </summary>
    public partial class Iok8sapiautoscalingv2beta1PodsMetricStatus
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapiautoscalingv2beta1PodsMetricStatus class.
        /// </summary>
        public Iok8sapiautoscalingv2beta1PodsMetricStatus() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapiautoscalingv2beta1PodsMetricStatus class.
        /// </summary>
        public Iok8sapiautoscalingv2beta1PodsMetricStatus(string currentAverageValue, string metricName)
        {
            CurrentAverageValue = currentAverageValue;
            MetricName = metricName;
        }

        /// <summary>
        /// currentAverageValue is the current value of the average of the
        /// metric across all relevant pods (as a quantity)
        /// </summary>
        [JsonProperty(PropertyName = "currentAverageValue")]
        public string CurrentAverageValue { get; set; }

        /// <summary>
        /// metricName is the name of the metric in question
        /// </summary>
        [JsonProperty(PropertyName = "metricName")]
        public string MetricName { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (CurrentAverageValue == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "CurrentAverageValue");
            }
            if (MetricName == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "MetricName");
            }
        }
    }
}
