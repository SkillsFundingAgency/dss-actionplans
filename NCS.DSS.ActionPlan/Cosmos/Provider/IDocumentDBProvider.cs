﻿using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.Cosmos.Provider
{
    public interface IDocumentDBProvider
    {
        string GetCustomerJson();
        string GetSessionForCustomerJson();
        Task<bool> DoesCustomerResourceExist(Guid customerId);
        bool DoesInteractionResourceExistAndBelongToCustomer(Guid interactionId, Guid customerId);
        bool DoesSessionResourceExistAndBelongToCustomer(Guid sessionId, Guid interactionId, Guid customerId);
        Task<List<Models.ActionPlan>> GetActionPlansForCustomerAsync(Guid customerId);
        Task<string> GetActionPlanForCustomerToUpdateAsync(Guid customerId, Guid actionPlanId);
        Task<Models.ActionPlan> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId);
        Task<ResourceResponse<Document>> CreateActionPlanAsync(Models.ActionPlan actionPlan);
        Task<ResourceResponse<Document>> UpdateActionPlanAsync(string actionPlanJson, Guid actionPlanId);
        Task<DateTime?> GetDateAndTimeOfSessionFromSessionResource(Guid sessionId);
    }
}