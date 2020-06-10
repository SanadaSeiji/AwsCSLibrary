using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Logging;
using Amazon.Lambda.SQSEvents;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon;
using AwsCSLibrary.Interfaces;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AwsCSLibrary
{
    public class Function //Example Lambda function using AwsManagers through DI
    {

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance.
        /// </summary>
        public Function()
        {
            try
            {
                TaskScheduler.UnobservedTaskException += (object sender, UnobservedTaskExceptionEventArgs eventArgs) =>
                {
                    eventArgs.SetObserved();
                    ((AggregateException)eventArgs.Exception).Handle(ex =>
                    {
                        Console.WriteLine("Unobserved Exception type: {0}", ex.GetType());
                        Console.WriteLine("Unobserved Message: {0}", ex.Message);
                        Console.WriteLine("Unobserved StackTrace: {0}", ex.StackTrace);
                        return true;
                    });
                };
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
        /// to respond to SQS messages.
        /// </summary>
        /// <param name="sqsEvent"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent sqsEvent, ILambdaContext context)
        {
            try
            {
                if (sqsEvent == null || sqsEvent.Records == null)
                    return;

                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);
                var serviceProvider = serviceCollection.BuildServiceProvider();

                foreach (var message in sqsEvent.Records)
                {
                    await ProcessMessageAsync(serviceProvider, message, context);
                }
            }
            catch (Exception e)
            {
                //would time out be caught?
                Console.WriteLine(e.Message);
            }
        }

        private async Task ProcessMessageAsync(IServiceProvider serviceProvider, SQSEvent.SQSMessage message, ILambdaContext context)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string version = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
            context.Logger.LogLine($"version: {version} Processed message {message.Body} @ Start of Job");

            var logger = serviceProvider.GetService<ILogger<Function>>();

            try
            {
                await serviceProvider.GetService<App>().RunAsync(message.Body);
            }
            catch (Exception e)
            {
                context.Logger.LogLine(e.Message + " " + e.StackTrace);
                logger.Log(LogLevel.Error, e.Message + " " + e.StackTrace);
            }

            context.Logger.LogLine($"Flushing Log @ End of Job");
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("KEY")))
                throw new Exception("Missing KEY environment variable");
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SECRET")))
                throw new Exception("Missing SECRET environment variable");
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("REGION")))
                throw new Exception("Missing REGION environment variable");

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Bucket")))
                throw new Exception("Missing Bucket environment variable");         
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Queue")))
                throw new Exception("Missing Queue environment variable");

            serviceCollection.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });

            var awsOptions = new AWSOptions
            {
                Credentials = new BasicAWSCredentials(Environment.GetEnvironmentVariable("KEY"),
                                                      Environment.GetEnvironmentVariable("SECRET")),
                Region = RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("REGION"))
        };
            serviceCollection.AddSingleton(awsOptions);
            var awsConfig = new AwsConfig
            {
                Bucket = Environment.GetEnvironmentVariable("Bucket"),
                Queue = Environment.GetEnvironmentVariable("Queue")
            };
            serviceCollection.AddSingleton(awsConfig);
            serviceCollection.AddTransient<IAwsManagers, AwsManagers>();
            serviceCollection.AddSingleton<App>();
        }
    }
}
