using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace NCS.DSS.ActionPlan.Cosmos.Provider
{
    public interface IDocumentDBProvider
    {
        Task<bool> DoesCustomerResourceExist(Guid customerId);
        Task<bool> DoesInteractionResourceExist(Guid interactionId);
        Task<bool> DoesCustomerHaveATerminationDate(Guid customerId);
        Task<List<Models.ActionPlan>> GetActionPlansForCustomerAsync(Guid customerId);
        Task<Models.ActionPlan> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId);
        Task<ResourceResponse<Document>> CreateActionPlanAsync(Models.ActionPlan actionPlan);
        Task<ResourceResponse<Document>> UpdateActionPlanAsync(Models.ActionPlan actionPlan);
    }
}