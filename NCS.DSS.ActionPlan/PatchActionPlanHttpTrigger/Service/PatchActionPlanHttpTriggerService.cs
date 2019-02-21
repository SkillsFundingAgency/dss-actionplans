using System;
using System.Net;
using System.Threading.Tasks;
using NCS.DSS.ActionPlan.Cosmos.Provider;
using NCS.DSS.ActionPlan.Models;
using NCS.DSS.ActionPlan.ServiceBus;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service
{
    public class PatchActionPlanHttpTriggerService : IPatchActionPlanHttpTriggerService
    {
        private readonly IActionPlanPatchService _actionPlanPatchService;
        private readonly IDocumentDBProvider _documentDbProvider;

        public PatchActionPlanHttpTriggerService(IActionPlanPatchService actionPlanPatchService, IDocumentDBProvider documentDbProvider)
        {
            _actionPlanPatchService = actionPlanPatchService;
            _documentDbProvider = documentDbProvider;
        }

        public string PatchResource(string actionPlanJson, ActionPlanPatch actionPlanPatch)
        {
            if (string.IsNullOrEmpty(actionPlanJson))
                return null;

            if (actionPlanPatch == null)
                return null;

            actionPlanPatch.SetDefaultValues();

            var updatedActionPlan = _actionPlanPatchService.Patch(actionPlanJson, actionPlanPatch);

            return updatedActionPlan;
        }

        public async Task<Models.ActionPlan> UpdateCosmosAsync(string actionPlanJson, Guid actionPlanId)
        {
            if (string.IsNullOrEmpty(actionPlanJson))
                return null;

            var response = await _documentDbProvider.UpdateActionPlanAsync(actionPlanJson, actionPlanId);

            var responseStatusCode = response?.StatusCode;

            return responseStatusCode == HttpStatusCode.OK ? (dynamic)response.Resource : null;
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