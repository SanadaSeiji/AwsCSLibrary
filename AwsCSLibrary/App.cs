using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AwsCSLibrary.Interfaces;

namespace AwsCSLibrary
{
    public class App
    {
        private readonly ILogger logger;

        private readonly IAwsManagers aws;

        public App(IAwsManagers aws, ILogger<App> logger)
        {
            this.aws = aws ?? throw new ArgumentNullException(nameof(aws));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            logger.LogInformation("Logger Initialized");
        }

        public async Task RunAsync(string messageJson)
        {
            logger.LogInformation("App Processing Begun");
            if (string.IsNullOrEmpty(messageJson)) throw new Exception("Message is empty");

            // retrieve info from message 

            // do somehting need await and aws

            logger.LogInformation("App Processing Complete");
        }
    }
}
