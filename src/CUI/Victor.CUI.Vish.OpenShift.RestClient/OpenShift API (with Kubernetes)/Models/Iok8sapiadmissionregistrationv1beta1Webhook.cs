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
    /// Webhook describes an admission webhook and the resources and
    /// operations it applies to.
    /// </summary>
    public partial class Iok8sapiadmissionregistrationv1beta1Webhook
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapiadmissionregistrationv1beta1Webhook class.
        /// </summary>
        public Iok8sapiadmissionregistrationv1beta1Webhook() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapiadmissionregistrationv1beta1Webhook class.
        /// </summary>
        public Iok8sapiadmissionregistrationv1beta1Webhook(Iok8sapiadmissionregistrationv1beta1WebhookClientConfig clientConfig, string name, string failurePolicy = default(string), Iok8sapimachinerypkgapismetav1LabelSelector namespaceSelector = default(Iok8sapimachinerypkgapismetav1LabelSelector), IList<Iok8sapiadmissionregistrationv1beta1RuleWithOperations> rules = default(IList<Iok8sapiadmissionregistrationv1beta1RuleWithOperations>))
        {
            ClientConfig = clientConfig;
            FailurePolicy = failurePolicy;
            Name = name;
            NamespaceSelector = namespaceSelector;
            Rules = rules;
        }

        /// <summary>
        /// ClientConfig defines how to communicate with the hook. Required
        /// </summary>
        [JsonProperty(PropertyName = "clientConfig")]
        public Iok8sapiadmissionregistrationv1beta1WebhookClientConfig ClientConfig { get; set; }

        /// <summary>
        /// FailurePolicy defines how unrecognized errors from the admission
        /// endpoint are handled - allowed values are Ignore or Fail.
        /// Defaults to Ignore.
        /// </summary>
        [JsonProperty(PropertyName = "failurePolicy")]
        public string FailurePolicy { get; set; }

        /// <summary>
        /// The name of the admission webhook. Name should be fully qualified,
        /// e.g., imagepolicy.kubernetes.io, where "imagepolicy" is the name
        /// of the webhook, and kubernetes.io is the name of the
        /// organization. Required.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// NamespaceSelector decides whether to run the webhook on an object
        /// based on whether the namespace for that object matches the
        /// selector. If the object itself is a namespace, the matching is
        /// performed on object.metadata.labels. If the object is other
        /// cluster scoped resource, it is not subjected to the webhook.
        /// 
        /// For example, to run the webhook on any objects whose namespace is
        /// not associated with "runlevel" of "0" or "1";  you will set the
        /// selector as follows: "namespaceSelector": {
        /// "matchExpressions": [
        /// {
        /// "key": "runlevel",
        /// "operator": "NotIn",
        /// "values": [
        /// "0",
        /// "1"
        /// ]
        /// }
        /// ]
        /// }
        /// 
        /// If instead you want to only run the webhook on any objects whose
        /// namespace is associated with the "environment" of "prod" or
        /// "staging"; you will set the selector as follows:
        /// "namespaceSelector": {
        /// "matchExpressions": [
        /// {
        /// "key": "environment",
        /// "operator": "In",
        /// "values": [
        /// "prod",
        /// "staging"
        /// ]
        /// }
        /// ]
        /// }
        /// 
        /// See
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/labels/
        /// for more examples of label selectors.
        /// 
        /// Default to the empty LabelSelector, which matches everything.
        /// </summary>
        [JsonProperty(PropertyName = "namespaceSelector")]
        public Iok8sapimachinerypkgapismetav1LabelSelector NamespaceSelector { get; set; }

        /// <summary>
        /// Rules describes what operations on what resources/subresources the
        /// webhook cares about. The webhook cares about an operation if it
        /// matches _any_ Rule.
        /// </summary>
        [JsonProperty(PropertyName = "rules")]
        public IList<Iok8sapiadmissionregistrationv1beta1RuleWithOperations> Rules { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (ClientConfig == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ClientConfig");
            }
            if (Name == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Name");
            }
            if (this.ClientConfig != null)
            {
                this.ClientConfig.Validate();
            }
        }
    }
}