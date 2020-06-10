using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AwsCSLibrary;

namespace AwsCSLibraryTests
{
    [TestClass]
    public partial class AwsManagersTests
    {
        private AwsManagers aws;
        private Helpers hlp;

        [TestInitialize]
        public void SetEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("KEY", "accessKey");
            Environment.SetEnvironmentVariable("SECRET", "secretKey");
            Environment.SetEnvironmentVariable("Bucket", "bucketName");
            Environment.SetEnvironmentVariable("Queue", "sqsQueueUrl");

            var awsOptions = new AWSOptions
            {
                Credentials = new BasicAWSCredentials(Environment.GetEnvironmentVariable("KEY"),
                                                     Environment.GetEnvironmentVariable("SECRET")),
                Region = RegionEndpoint.EUWest2
            };

            var awsConfig = new AwsConfig
            {
                Bucket = Environment.GetEnvironmentVariable("Bucket"),
                Queue = Environment.GetEnvironmentVariable("Queue")
            };

            aws = new AwsManagers(awsOptions, awsConfig);
            hlp = new Helpers(awsOptions, awsConfig);
        }
    }
}
