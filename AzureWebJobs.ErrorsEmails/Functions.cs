using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions;
using SendGrid.Helpers.Mail;

namespace AzureWebJobs.ErrorsEmails
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("queue")] string message, TextWriter log)
        {
            log.WriteLine(message);

            ProcessMessage(message);
        }

        /// <summary>
        /// Triggered when an error is reported in other functions.
        /// Called whenever 2 errors occur within a 3 minutes sliding window (throttled at a maximum of 2 notifications per 10 minutes).
        /// </summary>
        public static void GlobalErrorMonitor([ErrorTrigger("0:03:00", 2, Throttle = "0:10:00")] TraceFilter filter, TextWriter log, [SendGrid(From = "no-reply@anydomainxyz.com", To = "anybody@anydomainxyz.com")] out Mail mail)
        {
            mail = new Mail();

            mail.Subject = "WebJob - Warning - An error has been detected in a job";
            mail.AddContent(new Content("text/plain", filter.GetDetailedMessage(1)));

            Console.Error.WriteLine("An error has been detected in a function.");

            log.WriteLine(filter.GetDetailedMessage(1));
        }

        private static void ProcessMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException(nameof(message));
            }

            //Do some work here...
        }
    }
}
