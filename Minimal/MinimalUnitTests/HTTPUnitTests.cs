using Microsoft.VisualStudio.TestTools.UnitTesting;
using Minimal.Common;
using Minimal.Network;
using Minimal.Utility;
using System.Text;

namespace MinimalUnitTests
{
    [TestClass]
    public class HTTP_sendRequestTest
    {
        [TestMethod]
        public void HTTPUnitTests_sendRequest()
        {
            var whcb = WebHeaderCollectionBuilder.Factory();
            whcb.add("Accept", "text/html");
            var response = HTTP.sendRequest(
                TestDataFactory.GetGoogle(), 
                HTTPVerbs.GET, 
                whcb.ToWebHeaderCollection(), 
                null, 
                null, 
                null);
            var stream = response.GetResponseStream();
            var buffer = StreamUtility.ReadFullStream(stream);
            var result = new UTF8Encoding().GetString(buffer);
            Assert.AreNotEqual(string.Empty, result);
        }
    }
}
