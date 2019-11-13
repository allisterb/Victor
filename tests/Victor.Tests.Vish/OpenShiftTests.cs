using System;
using System.Collections.Generic;
using Xunit;

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
        public void CanGetPods()
        {
            //var r = oc.GetPods().Result;
            //Assert.NotEmpty(r.Items);
        }

        [Fact]
        public void CanGetBuilds()
        {
            //var builds = oc.GetBuilds();
            //Assert.True(builds != null);
        }
    }
}
