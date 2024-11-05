using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Kev.Xrm.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (ShimsContext.Create())
            {
                //var service = new Microsoft.Xrm.Sdk.Fakes.StubIOrganizationService();

                //service.RetrieveStringGuidColumnSet = (name, id, columns) => new Entity("account") {Attributes = { {"accountnumber","toto"} }};

                //var accountService = new AccountService(service, service);
                //var result = accountService.TestMethod2(new Entity());

                //Assert.AreEqual(result, true);
            }
        }
    }
}