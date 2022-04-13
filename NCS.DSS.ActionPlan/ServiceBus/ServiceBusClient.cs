using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace NCS.DSS.ActionPlan.ServiceBus
{
    public static class ServiceBusClient
    {
        public static readonly string QueueName = Environment.GetEnvironmentVariable("QueueName");
        public static readonly string ServiceBusConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");

        public static async Task SendPostMessageAsync(Models.ActionPlan actionPlan, string reqUrl)
        {
            var queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            var messageModel = new MessageModel()
            {
                TitleMessage = "New Action Plan record {" + actionPlan.ActionPlanId + "} added at " + DateTime.UtcNow,
                CustomerGuid = actionPlan.CustomerId,
                LastModifiedDate = actionPlan.LastModifiedDate,
                URL = reqUrl + "/" + actionPlan.ActionPlanId,
                IsNewCustomer = false,
                TouchpointId = actionPlan.LastModifiedTouchpointId,
                SubcontractorId = actionPlan.SubcontractorId
            };

            var msg = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageModel)))
            {
                ContentType = "application/json",
                MessageId = actionPlan.CustomerId + " " + DateTime.UtcNow
            };

            await queueClient.SendAsync(msg);
        }

        public static async Task SendPatchMessageAsync(Models.ActionPlan actionPlan, Guid customerId, string reqUrl)
        {
            var queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            var messageModel = new MessageModel
            {
                TitleMessage = "Action Plan record modification for {" + customerId + "} at " + DateTime.UtcNow,
                CustomerGuid = customerId,
                LastModifiedDate = actionPlan.LastModifiedDate,
                URL = reqUrl,
                IsNewCustomer = false,
                TouchpointId = actionPlan.LastModifiedTouchpointId,
                SubcontractorId = actionPlan.SubcontractorId
            };

            var msg = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageModel)))
            {
                ContentType = "application/json",
                MessageId = customerId + " " + DateTime.UtcNow
            };

            await queueClient.SendAsync(msg);
        }

    }

    public class MessageModel
    {
        public string TitleMessage { get; set; }
        public Guid? CustomerGuid { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string URL { get; set; }
        public bool IsNewCustomer { get; set; }
        public string TouchpointId { get; set; }
        public string SubcontractorId { get; set; }

    }

}

