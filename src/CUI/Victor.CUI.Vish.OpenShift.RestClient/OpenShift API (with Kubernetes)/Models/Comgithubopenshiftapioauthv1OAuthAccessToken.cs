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
    /// OAuthAccessToken describes an OAuth access token
    /// </summary>
    public partial class Comgithubopenshiftapioauthv1OAuthAccessToken
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapioauthv1OAuthAccessToken class.
        /// </summary>
        public Comgithubopenshiftapioauthv1OAuthAccessToken() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Comgithubopenshiftapioauthv1OAuthAccessToken class.
        /// </summary>
        public Comgithubopenshiftapioauthv1OAuthAccessToken(string apiVersion = default(string), string authorizeToken = default(string), string clientName = default(string), long? expiresIn = default(long?), int? inactivityTimeoutSeconds = default(int?), string kind = default(string), Iok8sapimachinerypkgapismetav1ObjectMeta metadata = default(Iok8sapimachinerypkgapismetav1ObjectMeta), string redirectURI = default(string), string refreshToken = default(string), IList<string> scopes = default(IList<string>), string userName = default(string), string userUID = default(string))
        {
            ApiVersion = apiVersion;
            AuthorizeToken = authorizeToken;
            ClientName = clientName;
            ExpiresIn = expiresIn;
            InactivityTimeoutSeconds = inactivityTimeoutSeconds;
            Kind = kind;
            Metadata = metadata;
            RedirectURI = redirectURI;
            RefreshToken = refreshToken;
            Scopes = scopes;
            UserName = userName;
            UserUID = userUID;
        }

        /// <summary>
        /// APIVersion defines the versioned schema of this representation of
        /// an object. Servers should convert recognized schemas to the
        /// latest internal value, and may reject unrecognized values. More
        /// info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#resources
        /// </summary>
        [JsonProperty(PropertyName = "apiVersion")]
        public string ApiVersion { get; set; }

        /// <summary>
        /// AuthorizeToken contains the token that authorized this token
        /// </summary>
        [JsonProperty(PropertyName = "authorizeToken")]
        public string AuthorizeToken { get; set; }

        /// <summary>
        /// ClientName references the client that created this token.
        /// </summary>
        [JsonProperty(PropertyName = "clientName")]
        public string ClientName { get; set; }

        /// <summary>
        /// ExpiresIn is the seconds from CreationTime before this token
        /// expires.
        /// </summary>
        [JsonProperty(PropertyName = "expiresIn")]
        public long? ExpiresIn { get; set; }

        /// <summary>
        /// InactivityTimeoutSeconds is the value in seconds, from the
        /// CreationTimestamp, after which this token can no longer be used.
        /// The value is automatically incremented when the token is used.
        /// </summary>
        [JsonProperty(PropertyName = "inactivityTimeoutSeconds")]
        public int? InactivityTimeoutSeconds { get; set; }

        /// <summary>
        /// Kind is a string value representing the REST resource this object
        /// represents. Servers may infer this from the endpoint the client
        /// submits requests to. Cannot be updated. In CamelCase. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#types-kinds
        /// </summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        /// <summary>
        /// Standard object's metadata.
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public Iok8sapimachinerypkgapismetav1ObjectMeta Metadata { get; set; }

        /// <summary>
        /// RedirectURI is the redirection associated with the token.
        /// </summary>
        [JsonProperty(PropertyName = "redirectURI")]
        public string RedirectURI { get; set; }

        /// <summary>
        /// RefreshToken is the value by which this token can be renewed. Can
        /// be blank.
        /// </summary>
        [JsonProperty(PropertyName = "refreshToken")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Scopes is an array of the requested scopes.
        /// </summary>
        [JsonProperty(PropertyName = "scopes")]
        public IList<string> Scopes { get; set; }

        /// <summary>
        /// UserName is the user name associated with this token
        /// </summary>
        [JsonProperty(PropertyName = "userName")]
        public string UserName { get; set; }

        /// <summary>
        /// UserUID is the unique UID associated with this token
        /// </summary>
        [JsonProperty(PropertyName = "userUID")]
        public string UserUID { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (this.Metadata != null)
            {
                this.Metadata.Validate();
            }
        }
    }
}
