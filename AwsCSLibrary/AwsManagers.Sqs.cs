using Amazon.SQS;
using Amazon.SQS.Model;
using System.Threading.Tasks;
using AwsCSLibrary.Interfaces;

namespace AwsCSLibrary
{
    public partial class AwsManagers:IAwsManagers
    {
        public async Task<SendMessageResponse> SendMessageToQueue(string JobId)
        {
            // send to queue
            var request = new SendMessageRequest
            {
                QueueUrl = queueUrl,
                MessageBody = JobId
            };

            var response = await sqsCLient.SendMessageAsync(request);
            // # failed to send message? 
            return response;
        }
    }
}
