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
    /// A StatefulSetSpec is the specification of a StatefulSet.
    /// </summary>
    public partial class Iok8sapiappsv1StatefulSetSpec
    {
        /// <summary>
        /// Initializes a new instance of the Iok8sapiappsv1StatefulSetSpec
        /// class.
        /// </summary>
        public Iok8sapiappsv1StatefulSetSpec() { }

        /// <summary>
        /// Initializes a new instance of the Iok8sapiappsv1StatefulSetSpec
        /// class.
        /// </summary>
        public Iok8sapiappsv1StatefulSetSpec(Iok8sapimachinerypkgapismetav1LabelSelector selector, string serviceName, Iok8sapicorev1PodTemplateSpec template, string podManagementPolicy = default(string), int? replicas = default(int?), int? revisionHistoryLimit = default(int?), Iok8sapiappsv1StatefulSetUpdateStrategy updateStrategy = default(Iok8sapiappsv1StatefulSetUpdateStrategy), IList<Iok8sapicorev1PersistentVolumeClaim> volumeClaimTemplates = default(IList<Iok8sapicorev1PersistentVolumeClaim>))
        {
            PodManagementPolicy = podManagementPolicy;
            Replicas = replicas;
            RevisionHistoryLimit = revisionHistoryLimit;
            Selector = selector;
            ServiceName = serviceName;
            Template = template;
            UpdateStrategy = updateStrategy;
            VolumeClaimTemplates = volumeClaimTemplates;
        }

        /// <summary>
        /// podManagementPolicy controls how pods are created during initial
        /// scale up, when replacing pods on nodes, or when scaling down. The
        /// default policy is `OrderedReady`, where pods are created in
        /// increasing order (pod-0, then pod-1, etc) and the controller will
        /// wait until each pod is ready before continuing. When scaling
        /// down, the pods are removed in the opposite order. The alternative
        /// policy is `Parallel` which will create pods in parallel to match
        /// the desired scale without waiting, and on scale down will delete
        /// all pods at once.
        /// </summary>
        [JsonProperty(PropertyName = "podManagementPolicy")]
        public string PodManagementPolicy { get; set; }

        /// <summary>
        /// replicas is the desired number of replicas of the given Template.
        /// These are replicas in the sense that they are instantiations of
        /// the same Template, but individual replicas also have a consistent
        /// identity. If unspecified, defaults to 1.
        /// </summary>
        [JsonProperty(PropertyName = "replicas")]
        public int? Replicas { get; set; }

        /// <summary>
        /// revisionHistoryLimit is the maximum number of revisions that will
        /// be maintained in the StatefulSet's revision history. The revision
        /// history consists of all revisions not represented by a currently
        /// applied StatefulSetSpec version. The default value is 10.
        /// </summary>
        [JsonProperty(PropertyName = "revisionHistoryLimit")]
        public int? RevisionHistoryLimit { get; set; }

        /// <summary>
        /// selector is a label query over pods that should match the replica
        /// count. It must match the pod template's labels. More info:
        /// https://kubernetes.io/docs/concepts/overview/working-with-objects/labels/#label-selectors
        /// </summary>
        [JsonProperty(PropertyName = "selector")]
        public Iok8sapimachinerypkgapismetav1LabelSelector Selector { get; set; }

        /// <summary>
        /// serviceName is the name of the service that governs this
        /// StatefulSet. This service must exist before the StatefulSet, and
        /// is responsible for the network identity of the set. Pods get
        /// DNS/hostnames that follow the pattern:
        /// pod-specific-string.serviceName.default.svc.cluster.local where
        /// "pod-specific-string" is managed by the StatefulSet controller.
        /// </summary>
        [JsonProperty(PropertyName = "serviceName")]
        public string ServiceName { get; set; }

        /// <summary>
        /// template is the object that describes the pod that will be created
        /// if insufficient replicas are detected. Each pod stamped out by
        /// the StatefulSet will fulfill this Template, but have a unique
        /// identity from the rest of the StatefulSet.
        /// </summary>
        [JsonProperty(PropertyName = "template")]
        public Iok8sapicorev1PodTemplateSpec Template { get; set; }

        /// <summary>
        /// updateStrategy indicates the StatefulSetUpdateStrategy that will
        /// be employed to update Pods in the StatefulSet when a revision is
        /// made to Template.
        /// </summary>
        [JsonProperty(PropertyName = "updateStrategy")]
        public Iok8sapiappsv1StatefulSetUpdateStrategy UpdateStrategy { get; set; }

        /// <summary>
        /// volumeClaimTemplates is a list of claims that pods are allowed to
        /// reference. The StatefulSet controller is responsible for mapping
        /// network identities to claims in a way that maintains the identity
        /// of a pod. Every claim in this list must have at least one
        /// matching (by name) volumeMount in one container in the template.
        /// A claim in this list takes precedence over any volumes in the
        /// template, with the same name.
        /// </summary>
        [JsonProperty(PropertyName = "volumeClaimTemplates")]
        public IList<Iok8sapicorev1PersistentVolumeClaim> VolumeClaimTemplates { get; set; }

        /// <summary>
        /// Validate the object. Throws ValidationException if validation fails.
        /// </summary>
        public virtual void Validate()
        {
            if (Selector == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Selector");
            }
            if (ServiceName == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "ServiceName");
            }
            if (Template == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Template");
            }
            if (this.Template != null)
            {
                this.Template.Validate();
            }
            if (this.VolumeClaimTemplates != null)
            {
                foreach (var element in this.VolumeClaimTemplates)
                {
                    if (element != null)
                    {
                        element.Validate();
                    }
                }
            }
        }
    }
}
