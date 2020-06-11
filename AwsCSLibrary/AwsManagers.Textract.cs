using Amazon.Runtime;
using Amazon.Textract.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AwsCSLibrary.Interfaces;
using System.Text;

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
		
		
		public bool IsJobComplete(string jobId)
        {
            var response = textractClient.GetDocumentAnalysisAsync(new GetDocumentAnalysisRequest
            {
                JobId = jobId
            });
            response.Wait();
            return !response.Result.JobStatus.Equals("IN_PROGRESS");
        }
		
		
		public GetDocumentAnalysisResponse GetJobResults(string jobId)
        {
            var response = textractClient.GetDocumentAnalysisAsync(new GetDocumentAnalysisRequest
            {
                JobId = jobId
            });
            response.Wait();
            return response.Result;
        }


        public string GetRawText(GetDocumentAnalysisResponse response)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var element in response.Blocks)
            {
                if (element.BlockType == "LINE")
                {
                    sb.AppendLine(element.Text);
                }
            }
            return sb.ToString();
        }

        // result can be stored in .tsv
        // (which does not use , or ; as separator - which can be appeared in key/value)
        public string CreateKeyValueText(GetDocumentAnalysisResponse response)
        {
            StringBuilder sb = new StringBuilder();
            string seperator = "\t";
            sb.AppendLine("Key" + seperator + "Value");
            var document = new TextractDocument(response);
            document.Pages.ForEach(page => {
                page.Form.Fields.ForEach(f => {
                    sb.AppendLine(f.Key + seperator + f.Value);
                });
            });
            return sb.ToString();
        }
    }
}
