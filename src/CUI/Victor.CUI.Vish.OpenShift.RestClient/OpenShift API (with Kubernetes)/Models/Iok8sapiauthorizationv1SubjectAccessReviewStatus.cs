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
    /// SubjectAccessReviewStatus
    /// </summary>
    public partial class Iok8sapiauthorizationv1SubjectAccessReviewStatus
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapiauthorizationv1SubjectAccessReviewStatus class.
        /// </summary>
        public Iok8sapiauthorizationv1SubjectAccessReviewStatus() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapiauthorizationv1SubjectAccessReviewStatus class.
        /// </summary>
        public Iok8sapiauthorizationv1SubjectAccessReviewStatus(bool allowed, bool? denied = default(bool?), string evaluationError = default(string), string reason = default(string))
        {
            Allowed = allowed;
            Denied = denied;
            EvaluationError = evaluationError;
            Reason = reason;
        }

        /// <summary>
        /// Allowed is required. True if the action would be allowed, false
        /// otherwise.
        /// </summary>
        [JsonProperty(PropertyName = "allowed")]
        public bool Allowed { get; set; }

        /// <summary>
        /// Denied is optional. True if the action would be denied, otherwise
        /// false. If both allowed is false and denied is false, then the
        /// authorizer has no opinion on whether to authorize the action.
        /// Denied may not be true if Allowed is true.
        /// </summary>
        [JsonProperty(PropertyName = "denied")]
        public bool? Denied { get; set; }

        /// <summary>
        /// EvaluationError is an indication that some error occurred during
        /// the authorization check. It is entirely possible to get an error
        /// and be able to continue determine authorization status in spite
        /// of it. For instance, RBAC can be missing a role, but enough roles
        /// are still present and bound to reason about the request.
        /// </summary>
        [JsonProperty(PropertyName = "evaluationError")]
        public string EvaluationError { get; set; }

        /// <summary>
        /// Reason is optional.  It indicates why a request was allowed or
        /// denied.
        /// </summary>
        [JsonProperty(PropertyName = "reason")]
        public string Reason { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            //Nothing to validate
        }
    }
}
