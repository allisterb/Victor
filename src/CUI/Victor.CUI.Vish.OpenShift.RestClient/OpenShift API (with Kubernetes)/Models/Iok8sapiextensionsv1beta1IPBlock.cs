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
    /// DEPRECATED 1.9 - This group version of IPBlock is deprecated by
    /// networking/v1/IPBlock. IPBlock describes a particular CIDR (Ex.
    /// "192.168.1.1/24") that is allowed to the pods matched by a
    /// NetworkPolicySpec's podSelector. The except entry describes CIDRs
    /// that should not be included within this rule.
    /// </summary>
    public partial class Iok8sapiextensionsv1beta1IPBlock
    {
        /// <summary>
        /// Initializes a new instance of the Iok8sapiextensionsv1beta1IPBlock
        /// class.
        /// </summary>
        public Iok8sapiextensionsv1beta1IPBlock() { }

        /// <summary>
        /// Initializes a new instance of the Iok8sapiextensionsv1beta1IPBlock
        /// class.
        /// </summary>
        public Iok8sapiextensionsv1beta1IPBlock(string cidr, IList<string> except = default(IList<string>))
        {
            Cidr = cidr;
            Except = except;
        }

        /// <summary>
        /// CIDR is a string representing the IP Block Valid examples are
        /// "192.168.1.1/24"
        /// </summary>
        [JsonProperty(PropertyName = "cidr")]
        public string Cidr { get; set; }

        /// <summary>
        /// Except is a slice of CIDRs that should not be included within an
        /// IP Block Valid examples are "192.168.1.1/24" Except values will
        /// be rejected if they are outside the CIDR range
        /// </summary>
        [JsonProperty(PropertyName = "except")]
        public IList<string> Except { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (Cidr == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Cidr");
            }
        }
    }
}
