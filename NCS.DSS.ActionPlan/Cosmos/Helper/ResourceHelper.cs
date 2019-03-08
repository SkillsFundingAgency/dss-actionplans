﻿using System;
using System.Threading.Tasks;
using NCS.DSS.ActionPlan.Cosmos.Provider;

namespace NCS.DSS.ActionPlan.Cosmos.Helper
{
    public class ResourceHelper : IResourceHelper
    {
        private readonly IDocumentDBProvider _documentDbProvider;
        public ResourceHelper(IDocumentDBProvider documentDbProvider)
        {
            _documentDbProvider = documentDbProvider;
        }

        public async Task<bool> DoesCustomerExist(Guid customerId)
        {
            var doesCustomerExist = await _documentDbProvider.DoesCustomerResourceExist(customerId);

            return doesCustomerExist;
        }

        public async Task<bool> IsCustomerReadOnly(Guid customerId)
        {
            var isCustomerReadOnly = await _documentDbProvider.DoesCustomerHaveATerminationDate(customerId);

            return isCustomerReadOnly;
        }

        public bool DoesInteractionExistAndBelongToCustomer(Guid interactionId, Guid customerId)
        {
            var doesInteractionExist = _documentDbProvider.DoesInteractionResourceExistAndBelongToCustomer(interactionId, customerId);

            return doesInteractionExist;
        }

        public async Task<DateTime?> GetDateAndTimeOfSession(Guid sessionId)
        {
            var dateAndTimeOfSession = await _documentDbProvider.GetDateAndTimeOfSessionFromSessionResource(sessionId);

            return dateAndTimeOfSession;
        }
    }
}
