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
    /// DEPRECATED 1.9 - This group version of NetworkPolicyPeer is deprecated
    /// by networking/v1/NetworkPolicyPeer.
    /// </summary>
    public partial class Iok8sapiextensionsv1beta1NetworkPolicyPeer
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapiextensionsv1beta1NetworkPolicyPeer class.
        /// </summary>
        public Iok8sapiextensionsv1beta1NetworkPolicyPeer() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapiextensionsv1beta1NetworkPolicyPeer class.
        /// </summary>
        public Iok8sapiextensionsv1beta1NetworkPolicyPeer(Iok8sapiextensionsv1beta1IPBlock ipBlock = default(Iok8sapiextensionsv1beta1IPBlock), Iok8sapimachinerypkgapismetav1LabelSelector namespaceSelector = default(Iok8sapimachinerypkgapismetav1LabelSelector), Iok8sapimachinerypkgapismetav1LabelSelector podSelector = default(Iok8sapimachinerypkgapismetav1LabelSelector))
        {
            IpBlock = ipBlock;
            NamespaceSelector = namespaceSelector;
            PodSelector = podSelector;
        }

        /// <summary>
        /// IPBlock defines policy on a particular IPBlock
        /// </summary>
        [JsonProperty(PropertyName = "ipBlock")]
        public Iok8sapiextensionsv1beta1IPBlock IpBlock { get; set; }

        /// <summary>
        /// Selects Namespaces using cluster scoped-labels.  This matches all
        /// pods in all namespaces selected by this label selector. This
        /// field follows standard label selector semantics. If present but
        /// empty, this selector selects all namespaces.
        /// </summary>
        [JsonProperty(PropertyName = "namespaceSelector")]
        public Iok8sapimachinerypkgapismetav1LabelSelector NamespaceSelector { get; set; }

        /// <summary>
        /// This is a label selector which selects Pods in this namespace.
        /// This field follows standard label selector semantics. If present
        /// but empty, this selector selects all pods in this namespace.
        /// </summary>
        [JsonProperty(PropertyName = "podSelector")]
        public Iok8sapimachinerypkgapismetav1LabelSelector PodSelector { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (this.IpBlock != null)
            {
                this.IpBlock.Validate();
            }
        }
    }
}
