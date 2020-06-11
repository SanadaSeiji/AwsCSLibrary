using Amazon.Extensions.NETCore.Setup;
using AwsCSLibrary.Interfaces;
using Amazon.S3;
using Amazon.SQS;
using Amazon.Textract;
using Amazon.DynamoDBv2;
using Amazon;
using Amazon.Runtime;

namespace AwsCSLibrary
{
    public partial class AwsManagers:IAwsManagers
    {
        // in constructor set up client for every services?
        private readonly IAmazonS3 s3Client;
        private readonly string bucketName;

        private readonly IAmazonSQS sqsCLient;
        private readonly string queueUrl;

        private readonly IAmazonTextract textractClient;

        private readonly AmazonDynamoDBClient dbClient;
        private readonly string tableName;

        public AwsManagers(AwsConfig config)
        {
            var options = new AWSOptions
            {
                Credentials = new BasicAWSCredentials(config.AccessKey,config.SecretKey),
                Region = RegionEndpoint.GetBySystemName(config.Region)
            };

            s3Client = options.CreateServiceClient<IAmazonS3>();
            sqsCLient = options.CreateServiceClient<IAmazonSQS>();         
            textractClient = options.CreateServiceClient<IAmazonTextract>();


            dbClient = new AmazonDynamoDBClient(config.AccessKey, config.SecretKey, RegionEndpoint.GetBySystemName(config.Region));

            bucketName = config.Bucket;
            queueUrl = config.Queue;
            tableName = config.Table;
        }
    }
}
