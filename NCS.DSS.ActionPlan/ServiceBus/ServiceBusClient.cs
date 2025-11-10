using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCS.DSS.ActionPlan.Models;
using Newtonsoft.Json;
using System.Text;

namespace NCS.DSS.ActionPlan.ServiceBus
{
    public class ActionPlanServiceBusClient : IActionPlanServiceBusClient
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ILogger<ActionPlanServiceBusClient> _logger;
        private readonly string _queueName;

        public ActionPlanServiceBusClient(ServiceBusClient serviceBusClient,
            IOptions<ActionPlanConfigurationSettings> configOptions,
            ILogger<ActionPlanServiceBusClient> logger)
        {
            var config = configOptions.Value;
            if (string.IsNullOrEmpty(config.QueueName))
            {
                throw new ArgumentNullException(nameof(config.QueueName), "QueueName cannot be null or empty.");
            }

            _serviceBusClient = serviceBusClient;
            _queueName = config.QueueName;
            _logger = logger;
        }

        public async Task SendPostMessageAsync(Models.ActionPlan actionPlan, string reqUrl)
        {
            var serviceBusSender = _serviceBusClient.CreateSender(_queueName);

            var messageModel = new MessageModel()
            {
                TitleMessage = "New Action Plan record {" + actionPlan.ActionPlanId + "} added at " + DateTime.UtcNow,
                CustomerGuid = actionPlan.CustomerId,
                LastModifiedDate = actionPlan.LastModifiedDate,
                URL = reqUrl + "/" + actionPlan.ActionPlanId,
                IsNewCustomer = false,
                TouchpointId = actionPlan.LastModifiedTouchpointId
            };

            var msg = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageModel)))
            {
                ContentType = "application/json",
                MessageId = actionPlan.CustomerId + " " + DateTime.UtcNow
            };

            _logger.LogTrace("Attempting to send POST message to service bus. Action Plan ID: {Action Plan Id}", actionPlan.ActionPlanId);

            await serviceBusSender.SendMessageAsync(msg);

            _logger.LogTrace("Successfully sent POST message to the service bus. Action Plan ID: {Action Plan Id}", actionPlan.ActionPlanId);
        }

        public async Task SendPatchMessageAsync(Models.ActionPlan actionPlan, Guid customerId, string reqUrl)
        {
            var serviceBusSender = _serviceBusClient.CreateSender(_queueName);

            var messageModel = new MessageModel
            {
                TitleMessage = "Action Plan record modification for {" + customerId + "} at " + DateTime.UtcNow,
                CustomerGuid = customerId,
                LastModifiedDate = actionPlan.LastModifiedDate,
                URL = reqUrl,
                IsNewCustomer = false,
                TouchpointId = actionPlan.LastModifiedTouchpointId
            };

            var msg = new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageModel)))
            {
                ContentType = "application/json",
                MessageId = customerId + " " + DateTime.UtcNow
            };

            _logger.LogTrace("Attempting to send PATCH message to service bus. Action Plan ID: {Action Plan Id}", actionPlan.ActionPlanId);

            await serviceBusSender.SendMessageAsync(msg);

            _logger.LogTrace("Successfully sent PATCH message to the service bus. Action Plan ID: {Action Plan Id}", actionPlan.ActionPlanId);
        }
    }
}

