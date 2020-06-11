
namespace AwsCSLibrary
{
    public class AwsConfig
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string Region { get; set; }

        public string Bucket { get; set; }
        public string Queue { get; set; }

        public string Table { get; set; }
    }
}
