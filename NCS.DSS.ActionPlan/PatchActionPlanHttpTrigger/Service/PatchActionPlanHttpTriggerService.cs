using System;
using System.Net;
using System.Threading.Tasks;
using NCS.DSS.ActionPlan.Cosmos.Provider;
using NCS.DSS.ActionPlan.Models;
using NCS.DSS.ActionPlan.ServiceBus;
using Newtonsoft.Json;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service
{
    public class PatchActionPlanHttpTriggerService : IPatchActionPlanHttpTriggerService
    {
        private IActionPlanPatchService _actionPlanPatchService;
        private readonly IDocumentDBProvider _documentDbProvider;

        public PatchActionPlanHttpTriggerService(IActionPlanPatchService actionPlanPatchService, IDocumentDBProvider documentDbProvider)
        {
            _actionPlanPatchService = actionPlanPatchService;
            _documentDbProvider = documentDbProvider;
        }
        public async Task<Models.ActionPlan> UpdateAsync(string actionPlanJson, ActionPlanPatch actionPlanPatch, Guid actionPlanId)
        {
            if (string.IsNullOrEmpty(actionPlanJson))
                return null;

            actionPlanPatch.SetDefaultValues();

            var updatedJson = _actionPlanPatchService.Patch(actionPlanJson, actionPlanPatch);

            var response = await _documentDbProvider.UpdateActionPlanAsync(updatedJson, actionPlanId);

            var responseStatusCode = response.StatusCode;

            return responseStatusCode == HttpStatusCode.OK ? JsonConvert.DeserializeObject<Models.ActionPlan>(updatedJson) : null;
        }

        public async Task<string> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId)
        {
            var actionPlan = await _documentDbProvider.GetActionPlanForCustomerToUpdateAsync(customerId, actionPlanId);

            return actionPlan;
        }

        public async Task SendToServiceBusQueueAsync(Models.ActionPlan actionPlan, Guid customerId, string reqUrl)
        {
            await ServiceBusClient.SendPatchMessageAsync(actionPlan, customerId, reqUrl);
        }
    }
}