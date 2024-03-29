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
    /// ISCSIPersistentVolumeSource represents an ISCSI disk. ISCSI volumes
    /// can only be mounted as read/write once. ISCSI volumes support
    /// ownership management and SELinux relabeling.
    /// </summary>
    public partial class Iok8sapicorev1ISCSIPersistentVolumeSource
    {
        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapicorev1ISCSIPersistentVolumeSource class.
        /// </summary>
        public Iok8sapicorev1ISCSIPersistentVolumeSource() { }

        /// <summary>
        /// Initializes a new instance of the
        /// Iok8sapicorev1ISCSIPersistentVolumeSource class.
        /// </summary>
        public Iok8sapicorev1ISCSIPersistentVolumeSource(string iqn, int lun, string targetPortal, bool? chapAuthDiscovery = default(bool?), bool? chapAuthSession = default(bool?), string fsType = default(string), string initiatorName = default(string), string iscsiInterface = default(string), IList<string> portals = default(IList<string>), bool? readOnlyProperty = default(bool?), Iok8sapicorev1SecretReference secretRef = default(Iok8sapicorev1SecretReference))
        {
            ChapAuthDiscovery = chapAuthDiscovery;
            ChapAuthSession = chapAuthSession;
            FsType = fsType;
            InitiatorName = initiatorName;
            Iqn = iqn;
            IscsiInterface = iscsiInterface;
            Lun = lun;
            Portals = portals;
            ReadOnlyProperty = readOnlyProperty;
            SecretRef = secretRef;
            TargetPortal = targetPortal;
        }

        /// <summary>
        /// whether support iSCSI Discovery CHAP authentication
        /// </summary>
        [JsonProperty(PropertyName = "chapAuthDiscovery")]
        public bool? ChapAuthDiscovery { get; set; }

        /// <summary>
        /// whether support iSCSI Session CHAP authentication
        /// </summary>
        [JsonProperty(PropertyName = "chapAuthSession")]
        public bool? ChapAuthSession { get; set; }

        /// <summary>
        /// Filesystem type of the volume that you want to mount. Tip: Ensure
        /// that the filesystem type is supported by the host operating
        /// system. Examples: "ext4", "xfs", "ntfs". Implicitly inferred to
        /// be "ext4" if unspecified. More info:
        /// https://kubernetes.io/docs/concepts/storage/volumes#iscsi
        /// </summary>
        [JsonProperty(PropertyName = "fsType")]
        public string FsType { get; set; }

        /// <summary>
        /// Custom iSCSI Initiator Name. If initiatorName is specified with
        /// iscsiInterface simultaneously, new iSCSI interface &lt;target
        /// portal&gt;:&lt;volume name&gt; will be created for the connection.
        /// </summary>
        [JsonProperty(PropertyName = "initiatorName")]
        public string InitiatorName { get; set; }

        /// <summary>
        /// Target iSCSI Qualified Name.
        /// </summary>
        [JsonProperty(PropertyName = "iqn")]
        public string Iqn { get; set; }

        /// <summary>
        /// iSCSI Interface Name that uses an iSCSI transport. Defaults to
        /// 'default' (tcp).
        /// </summary>
        [JsonProperty(PropertyName = "iscsiInterface")]
        public string IscsiInterface { get; set; }

        /// <summary>
        /// iSCSI Target Lun number.
        /// </summary>
        [JsonProperty(PropertyName = "lun")]
        public int Lun { get; set; }

        /// <summary>
        /// iSCSI Target Portal List. The Portal is either an IP or
        /// ip_addr:port if the port is other than default (typically TCP
        /// ports 860 and 3260).
        /// </summary>
        [JsonProperty(PropertyName = "portals")]
        public IList<string> Portals { get; set; }

        /// <summary>
        /// ReadOnly here will force the ReadOnly setting in VolumeMounts.
        /// Defaults to false.
        /// </summary>
        [JsonProperty(PropertyName = "readOnly")]
        public bool? ReadOnlyProperty { get; set; }

        /// <summary>
        /// CHAP Secret for iSCSI target and initiator authentication
        /// </summary>
        [JsonProperty(PropertyName = "secretRef")]
        public Iok8sapicorev1SecretReference SecretRef { get; set; }

        /// <summary>
        /// iSCSI Target Portal. The Portal is either an IP or ip_addr:port if
        /// the port is other than default (typically TCP ports 860 and 3260).
        /// </summary>
        [JsonProperty(PropertyName = "targetPortal")]
        public string TargetPortal { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (Iqn == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Iqn");
            }
            if (TargetPortal == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "TargetPortal");
            }
        }
    }
}
