using Amazon.SQS.Model;
using Amazon.Textract.Model;
using System.Threading.Tasks;

namespace AwsCSLibrary.Interfaces
{
    public interface IAwsManagers
    {
        // s3
        Task UploadFileAsync(string filePath, string key);

        void DownloadFile(string filePath, string key);
        Task DeleteS3ObjectAsync(string key);

        Task<bool> IsFileExistInS3bucketAsync(string key);
        Task MoveS3ObjectAsync(string key, string destinationFolder);

        // sqs
        Task<SendMessageResponse> SendMessageToQueue(string JobId);

        // textract
        Task<string> StartDocumentAnalysis(string key, string featureType, int maxRetry);
		bool IsJobComplete(string jobId)
		GetDocumentAnalysisResponse GetJobResults(string jobId)
		string GetRawText(GetDocumentAnalysisResponse response)
		string CreateKeyValueText(GetDocumentAnalysisResponse response)
    }
}
