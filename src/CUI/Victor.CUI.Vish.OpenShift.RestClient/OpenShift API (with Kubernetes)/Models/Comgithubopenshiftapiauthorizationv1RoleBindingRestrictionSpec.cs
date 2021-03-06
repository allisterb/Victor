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
    /// RoleBindingRestrictionSpec defines a rolebinding restriction.  Exactly
    /// one field must be non-nil.
    /// </summary>
    public partial class Comgithubopenshiftapiauthorizationv1RoleBindingRestrictionSpec
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapiauthorizationv1RoleBindingRestrictionSpec
        /// class.
        /// </summary>
        public Comgithubopenshiftapiauthorizationv1RoleBindingRestrictionSpec() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapiauthorizationv1RoleBindingRestrictionSpec
        /// class.
        /// </summary>
        public Comgithubopenshiftapiauthorizationv1RoleBindingRestrictionSpec(Comgithubopenshiftapiauthorizationv1GroupRestriction grouprestriction, Comgithubopenshiftapiauthorizationv1ServiceAccountRestriction serviceaccountrestriction, Comgithubopenshiftapiauthorizationv1UserRestriction userrestriction)
        {
            Grouprestriction = grouprestriction;
            Serviceaccountrestriction = serviceaccountrestriction;
            Userrestriction = userrestriction;
        }

        /// <summary>
        /// GroupRestriction matches against group subjects.
        /// </summary>
        [JsonProperty(PropertyName = "grouprestriction")]
        public Comgithubopenshiftapiauthorizationv1GroupRestriction Grouprestriction { get; set; }

        /// <summary>
        /// ServiceAccountRestriction matches against service-account subjects.
        /// </summary>
        [JsonProperty(PropertyName = "serviceaccountrestriction")]
        public Comgithubopenshiftapiauthorizationv1ServiceAccountRestriction Serviceaccountrestriction { get; set; }

        /// <summary>
        /// UserRestriction matches against user subjects.
        /// </summary>
        [JsonProperty(PropertyName = "userrestriction")]
        public Comgithubopenshiftapiauthorizationv1UserRestriction Userrestriction { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (Grouprestriction == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Grouprestriction");
            }
            if (Serviceaccountrestriction == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Serviceaccountrestriction");
            }
            if (Userrestriction == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Userrestriction");
            }
            if (this.Grouprestriction != null)
            {
                this.Grouprestriction.Validate();
            }
            if (this.Serviceaccountrestriction != null)
            {
                this.Serviceaccountrestriction.Validate();
            }
            if (this.Userrestriction != null)
            {
                this.Userrestriction.Validate();
            }
        }
    }
}
