using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;
using AwsServicesCSLibrary;

namespace AwsServicesCSLibraryTests
{
    [TestClass]
    public class AwsManagersTests
    {
        private AwsManagers aws;
        private Helpers hlp;

        [TestInitialize]
        public void SetEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("S3KEY", "accessKey");
            Environment.SetEnvironmentVariable("S3SECRET", "secretKey");
            Environment.SetEnvironmentVariable("S3BUCKET", "bucketName");
            Environment.SetEnvironmentVariable("PendingQueue", "sqsQueueUrl");

            var awsOptions = new AWSOptions
            {
                Credentials = new BasicAWSCredentials(Environment.GetEnvironmentVariable("S3KEY"),
                                                     Environment.GetEnvironmentVariable("S3SECRET")),
                Region = RegionEndpoint.EUWest2
            };

            var awsConfig = new AwsConfig
            {
                Bucket = Environment.GetEnvironmentVariable("S3BUCKET"),
                Queue = Environment.GetEnvironmentVariable("PendingQueue")
            };

            aws = new AwsManagers(awsOptions, awsConfig);
            hlp = new Helpers(awsOptions, awsConfig);


        }

        [TestMethod]
        public async Task UploadFileToBucket()
        {
            // create and upload
            string filename = hlp.CreateOnePagePdfOnLocalTemp();
            string key = "testfile.pdf";

            await aws.UploadFileAsync(filename, key);

            File.Delete(filename);

            // test exists
            bool exists = await aws.IsFileExistInS3bucketAsync(key);

            // cleanup testfile in bucket
            await aws.DeleteS3ObjectAsync(key);

            Assert.IsTrue(exists);
        }

        [TestMethod]
        public async Task DownloadFileFromBucket()
        {
            //upload
            string key = await hlp.UploadToBucketForTest("testfile");
            //download
            string filename = Path.Combine(Path.GetTempPath(), "downloadedFile");
            aws.DownloadFile(filename, key);

            bool downloaded = false;
            if (File.Exists(filename))
            {
                downloaded = true;
                File.Delete(filename);
            }

            Assert.IsTrue(downloaded);
        }

        [TestMethod]
        public async Task DeleteFileInBucket()
        {
            string key = await hlp.UploadToBucketForTest("testfile");
            await aws.DeleteS3ObjectAsync(key);

            bool exists = await aws.IsFileExistInS3bucketAsync(key);
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public async Task MoveObjectWithinBucket()
        {
            // upload to sourceKey
            string sourceKey = await hlp.UploadToBucketForTest("pending/testfile");
            string destinationKey = "processed/testfile";

            // move
            await aws.MoveS3ObjectAsync(sourceKey, destinationKey);

            // see if exist in detinationKey  true
            bool existsDestination = await aws.IsFileExistInS3bucketAsync(destinationKey);
            Assert.IsTrue(existsDestination);

            // see if exist in sourceKey false
            bool existsSource = await aws.IsFileExistInS3bucketAsync(sourceKey);
            Assert.IsFalse(existsSource);
        }
    }
}
