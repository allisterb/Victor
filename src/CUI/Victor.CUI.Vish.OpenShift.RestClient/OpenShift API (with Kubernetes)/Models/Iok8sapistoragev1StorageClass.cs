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
    /// StorageClass describes the parameters for a class of storage for which
    /// PersistentVolumes can be dynamically provisioned.
    /// 
    /// StorageClasses are non-namespaced; the name of the storage class
    /// according to etcd is in ObjectMeta.Name.
    /// </summary>
    public partial class Iok8sapistoragev1StorageClass
    {
        /// <summary>
        /// Initializes a new instance of the Iok8sapistoragev1StorageClass
        /// class.
        /// </summary>
        public Iok8sapistoragev1StorageClass() { }

        /// <summary>
        /// Initializes a new instance of the Iok8sapistoragev1StorageClass
        /// class.
        /// </summary>
        public Iok8sapistoragev1StorageClass(string provisioner, bool? allowVolumeExpansion = default(bool?), string apiVersion = default(string), string kind = default(string), Iok8sapimachinerypkgapismetav1ObjectMeta metadata = default(Iok8sapimachinerypkgapismetav1ObjectMeta), IList<string> mountOptions = default(IList<string>), IDictionary<string, string> parameters = default(IDictionary<string, string>), string reclaimPolicy = default(string), string volumeBindingMode = default(string))
        {
            AllowVolumeExpansion = allowVolumeExpansion;
            ApiVersion = apiVersion;
            Kind = kind;
            Metadata = metadata;
            MountOptions = mountOptions;
            Parameters = parameters;
            Provisioner = provisioner;
            ReclaimPolicy = reclaimPolicy;
            VolumeBindingMode = volumeBindingMode;
        }

        /// <summary>
        /// AllowVolumeExpansion shows whether the storage class allow volume
        /// expand
        /// </summary>
        [JsonProperty(PropertyName = "allowVolumeExpansion")]
        public bool? AllowVolumeExpansion { get; set; }

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
        /// Kind is a string value representing the REST resource this object
        /// represents. Servers may infer this from the endpoint the client
        /// submits requests to. Cannot be updated. In CamelCase. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#types-kinds
        /// </summary>
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        /// <summary>
        /// Standard object's metadata. More info:
        /// https://git.k8s.io/community/contributors/devel/api-conventions.md#metadata
        /// </summary>
        [JsonProperty(PropertyName = "metadata")]
        public Iok8sapimachinerypkgapismetav1ObjectMeta Metadata { get; set; }

        /// <summary>
        /// Dynamically provisioned PersistentVolumes of this storage class
        /// are created with these mountOptions, e.g. ["ro", "soft"]. Not
        /// validated - mount of the PVs will simply fail if one is invalid.
        /// </summary>
        [JsonProperty(PropertyName = "mountOptions")]
        public IList<string> MountOptions { get; set; }

        /// <summary>
        /// Parameters holds the parameters for the provisioner that should
        /// create volumes of this storage class.
        /// </summary>
        [JsonProperty(PropertyName = "parameters")]
        public IDictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// Provisioner indicates the type of the provisioner.
        /// </summary>
        [JsonProperty(PropertyName = "provisioner")]
        public string Provisioner { get; set; }

        /// <summary>
        /// Dynamically provisioned PersistentVolumes of this storage class
        /// are created with this reclaimPolicy. Defaults to Delete.
        /// </summary>
        [JsonProperty(PropertyName = "reclaimPolicy")]
        public string ReclaimPolicy { get; set; }

        /// <summary>
        /// VolumeBindingMode indicates how PersistentVolumeClaims should be
        /// provisioned and bound.  When unset, VolumeBindingImmediate is
        /// used. This field is alpha-level and is only honored by servers
        /// that enable the VolumeScheduling feature.
        /// </summary>
        [JsonProperty(PropertyName = "volumeBindingMode")]
        public string VolumeBindingMode { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (Provisioner == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Provisioner");
            }
            if (this.Metadata != null)
            {
                this.Metadata.Validate();
            }
        }
    }
}
