using Amazon.SQS.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace AwsCSLibraryTests
{
    public partial class AwsManagersTests
    {
        [TestMethod]
        public async Task SendMessage()
        {
            string JobId = "testmessage";
            SendMessageResponse response = await aws.SendMessageToQueue(JobId);

            // # how to clean up using code?
            Assert.IsNotNull(response);
        }
    }
}
