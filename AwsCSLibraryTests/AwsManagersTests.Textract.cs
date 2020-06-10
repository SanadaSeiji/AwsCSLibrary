using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace AwsCSLibraryTests
{
    public partial class AwsManagersTests
    {
        [TestMethod]
        public async Task GetJobId()
        {
            string key = await hlp.UploadToBucketForTest("testfile");
            string jobId = await aws.StartDocumentAnalysis(key, "FORMS",5);

            //cleanup
            await aws.DeleteS3ObjectAsync(key);

            Assert.IsNotNull(jobId);
            Console.WriteLine(jobId);
        }
    }
}
