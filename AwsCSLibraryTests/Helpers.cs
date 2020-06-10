using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.Logging;
using PdfSharp.Pdf;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using AwsCSLibrary;
using AwsCSLibrary.Interfaces;

namespace AwsCSLibraryTests
{
    internal class Helpers
    {
        private readonly IAwsManagers aws;
        internal Helpers() { 
        }

        internal Helpers(AWSOptions options, AwsConfig config)
        {
            aws = new AwsManagers(options, config);
        }
        internal ILogger<FileProcessorApp> GetLogger()
        {
            ILoggerFactory loggerFactory = new LoggerFactory().AddConsole().AddDebug();
            return loggerFactory.CreateLogger<FileProcessorApp>();
        }

        internal string CreateOnePagePdfOnLocalTemp()
        {
            // Create a new PDF document
            using (PdfDocument document = new PdfDocument()) {

                // Create an empty page
                document.AddPage();

                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                // Save the document  
                string filename = Path.Combine(Path.GetTempPath(), "samplePdfForTextractProcessor.pdf");
                document.Save(filename);
                return filename;
            }          
        }

        internal string CreateZipWithOneEntry(string filePath)
        {
            string zipPath = Path.Combine(Path.GetTempPath(), "testzip.zip");
            using (ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(filePath, "something.txt");
            }
            return zipPath;
        }


        internal async Task<string> UploadToBucketForTest(string key)
        {
            string filename = CreateOnePagePdfOnLocalTemp();
            await aws.UploadFileAsync(filename, key);
            File.Delete(filename);

            return key;
        }
    }
}
