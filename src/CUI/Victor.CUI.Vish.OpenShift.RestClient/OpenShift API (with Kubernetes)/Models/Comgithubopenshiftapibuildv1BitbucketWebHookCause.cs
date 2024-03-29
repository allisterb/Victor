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
    /// BitbucketWebHookCause has information about a Bitbucket webhook that
    /// triggered a build.
    /// </summary>
    public partial class Comgithubopenshiftapibuildv1BitbucketWebHookCause
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapibuildv1BitbucketWebHookCause class.
        /// </summary>
        public Comgithubopenshiftapibuildv1BitbucketWebHookCause() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapibuildv1BitbucketWebHookCause class.
        /// </summary>
        public Comgithubopenshiftapibuildv1BitbucketWebHookCause(Comgithubopenshiftapibuildv1SourceRevision revision = default(Comgithubopenshiftapibuildv1SourceRevision), string secret = default(string))
        {
            Revision = revision;
            Secret = secret;
        }

        /// <summary>
        /// Revision is the git source revision information of the trigger.
        /// </summary>
        [JsonProperty(PropertyName = "revision")]
        public Comgithubopenshiftapibuildv1SourceRevision Revision { get; set; }

        /// <summary>
        /// Secret is the obfuscated webhook secret that triggered a build.
        /// </summary>
        [JsonProperty(PropertyName = "secret")]
        public string Secret { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (this.Revision != null)
            {
                this.Revision.Validate();
            }
        }
    }
}
