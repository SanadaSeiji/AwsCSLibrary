using System.Threading.Tasks;

namespace AwsServicesCSLibrary.Interfaces
{
    public interface IAwsManagers
    {
        Task UploadFileAsync(string filePath, string key);

        void DownloadFile(string filePath, string key);
        Task DeleteS3ObjectAsync(string key);

        Task<bool> IsFileExistInS3bucketAsync(string key);
        Task MoveS3ObjectAsync(string key, string destinationFolder);
    }
}
