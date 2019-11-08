using System;
using Xunit;

namespace Victor.Tests
{
    public class OpenShiftTests : BaseTests
    {
        protected OpenShift oc;
        
        public OpenShiftTests() : base() 
        {
            oc = new OpenShift();
        }

        [Fact]
        public void CanGetPods()
        {
            oc.GetResources();
        }
    }
}
