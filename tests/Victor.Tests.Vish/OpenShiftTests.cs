using System;
using System.Collections.Generic;
using Xunit;

using Victor.CUI.Vish.OpenShift.Client.Models;

namespace Victor.Tests
{
    public class OpenShiftTests : BaseTests
    {
        protected OpenShift oc;
        
        public OpenShiftTests() : base() 
        {
            oc = new OpenShift(null, Api.Ct);
        }

        [Fact]
        public void CanGetProjects()
        {
            var p = oc.FetchProjects();
            Assert.NotEmpty(p.Items);
        }
        [Fact]
        public void CanGetPods()
        {
            var r = oc.FetchPods("evals25-shared-7daa", null);
            Assert.NotEmpty(r.Items);
        }

        [Fact]
        public void CanGetBuilds()
        {
            var builds = oc.GetBuilds("evals25-shared-7daa");
            Assert.NotEmpty(builds.Items);
        }
    }
}
