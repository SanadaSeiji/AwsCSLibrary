using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AwsCSLibrary;
using AwsCSLibrary.Interfaces;

namespace AwsCSLibraryTests
{
    [TestClass]
    public partial class AwsManagersTests
    {
        private IAwsManagers aws;
        private Helpers hlp;

        [TestInitialize]
        public void SetEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("KEY", "accessKey");
            Environment.SetEnvironmentVariable("SECRET", "secretKey");
            Environment.SetEnvironmentVariable("REGION", "region"); 
            Environment.SetEnvironmentVariable("Table", "tableName");
            Environment.SetEnvironmentVariable("Bucket", "bucketName");
            Environment.SetEnvironmentVariable("Queue", "sqsQueueUrl");


            var awsConfig = new AwsConfig
            {
                AccessKey = Environment.GetEnvironmentVariable("KEY"),
                SecretKey = Environment.GetEnvironmentVariable("SECRET"),
                Region = Environment.GetEnvironmentVariable("REGION"),
                Bucket = Environment.GetEnvironmentVariable("Bucket"),
                Table = Environment.GetEnvironmentVariable("Table"),
                Queue = Environment.GetEnvironmentVariable("Queue")
            };

            aws = new AwsManagers(awsConfig);
            hlp = new Helpers(awsConfig);
        }
    }
}
