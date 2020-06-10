using Amazon.Extensions.NETCore.Setup;
using AwsCSLibrary.Interfaces;
using Amazon.S3;
using Amazon.SQS;
using Amazon.Textract;


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

        public AwsManagers(AWSOptions options, AwsConfig config)
        {
            s3Client = options.CreateServiceClient<IAmazonS3>();
            sqsCLient = options.CreateServiceClient<IAmazonSQS>();         
            textractClient = options.CreateServiceClient<IAmazonTextract>();

            bucketName = config.Bucket;
            queueUrl = config.Queue;
        }
    }
}
