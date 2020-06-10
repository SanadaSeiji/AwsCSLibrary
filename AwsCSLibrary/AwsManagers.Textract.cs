using Amazon.Runtime;
using Amazon.Textract;
using Amazon.Textract.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AwsCSLibrary.Interfaces;
using Amazon.Extensions.NETCore.Setup;

namespace AwsCSLibrary
{
    public partial class AwsManagers:IAwsManagers
    {
        public async Task<string> StartDocumentAnalysis(string key, string featureType, int maxRetry)
        {
            var request = new StartDocumentAnalysisRequest();
            var s3Object = new S3Object
            {
                Bucket = bucketName,
                Name = key
            };
            request.DocumentLocation = new DocumentLocation
            {
                S3Object = s3Object
            };

            int retryTime = 0;

            request.FeatureTypes = new List<string> { featureType };

            try
            {
                var response = await textractClient.StartDocumentAnalysisAsync(request);
                return response.JobId;
            }
            catch(AmazonServiceException) //Open jobs exceed maximum concurrent job limit{
            {
                while (retryTime < maxRetry)
                {
                    try
                    {
                        retryTime++;
                        Console.WriteLine("retry -----" + retryTime.ToString() + " times");
                        Thread.Sleep(30000); // 30s
                        var response = await textractClient.StartDocumentAnalysisAsync(request);
                        return response.JobId;
                    }
                    catch (AmazonServiceException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }    
                throw new Exception("More than 2 jobs using AWS Textract! Retried " + maxRetry.ToString() + " times already.");
            }         
        }
    }
}
