using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AwsServicesCSLibrary.Interfaces;

namespace AwsServicesCSLibrary
{
    public class AwsManagers:IAwsManagers
    {
        // in constructor set up client for every services?
        private readonly IAmazonS3 s3Client;
        private readonly string bucketName;

        public AwsManagers(AWSOptions options, AwsConfig config)
        {
            s3Client = options.CreateServiceClient<IAmazonS3>();
            bucketName = config.Bucket;
        }

        // s3 creates folders automatically
        public async Task UploadFileAsync(string filePath, string key)
        {
            using (var fileTransferUtility = new TransferUtility(s3Client))
                await fileTransferUtility.UploadAsync(filePath, bucketName, key);
        }

        public void DownloadFile(string filePath, string key)
        {
            using (var fileTransferUtility = new TransferUtility(s3Client))
                fileTransferUtility.Download(filePath, bucketName, key);
        }

        public async Task DeleteS3ObjectAsync(string key)
        {
            DeleteObjectRequest request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };
            await s3Client.DeleteObjectAsync(request);
        }

        public async Task<bool> IsFileExistInS3bucketAsync(string key)
        {
            ListObjectsRequest listRequest = new ListObjectsRequest
            {
                BucketName = bucketName,
            };
            ListObjectsResponse listResponse = await s3Client.ListObjectsAsync(listRequest); ;
            foreach (S3Object obj in listResponse.S3Objects)
            {
                if (obj.Key == key)
                    return true;
            }
            return false;
        }

        public async Task CopyS3ObjectWithinBucketAsync(string sourceKey, string destinationKey)
        {
            CopyObjectRequest request = new CopyObjectRequest
            {
                SourceKey = sourceKey,
                SourceBucket = bucketName,
                DestinationBucket = bucketName,
                DestinationKey = destinationKey
            };
            await s3Client.CopyObjectAsync(request);
        }

        public async Task MoveS3ObjectAsync(string sourceKey, string destinationKey)
        {
            await CopyS3ObjectWithinBucketAsync(sourceKey, destinationKey);
            await DeleteS3ObjectAsync(sourceKey);
        }

        
    }
}
