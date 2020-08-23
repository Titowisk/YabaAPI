using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yaba.WebJob
{
    public class Functions
    {
        public static void ProcessQueueMessage([QueueTrigger("yabadev")] string message, ILogger logger)
        {
            logger.LogInformation(message);
        }
    }
}
