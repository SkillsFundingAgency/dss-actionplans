using System;
using System.Threading.Tasks;
using DFC.JSON.Standard;
using NCS.DSS.ActionPlan.Cosmos.Provider;

namespace NCS.DSS.ActionPlan.Cosmos.Helper
{
    public class ResourceHelper : IResourceHelper
    {
        private readonly IDocumentDBProvider _documentDbProvider;
        private readonly IJsonHelper _jsonHelper;

        public ResourceHelper(IDocumentDBProvider documentDbProvider, IJsonHelper jsonHelper)
        {
            _documentDbProvider = documentDbProvider;
            _jsonHelper = jsonHelper;
        }

        public async Task<bool> DoesCustomerExist(Guid customerId)
        {
            return await _documentDbProvider.DoesCustomerResourceExist(customerId);
        }

        public bool IsCustomerReadOnly()
        {
            var customerJson = _documentDbProvider.GetCustomerJson();

            if (string.IsNullOrWhiteSpace(customerJson))
                return false;

            var dateOfTermination = _jsonHelper.GetValue(customerJson, "DateOfTermination");

            return !string.IsNullOrWhiteSpace(dateOfTermination);
        }

        public bool DoesInteractionExistAndBelongToCustomer(Guid interactionId, Guid customerId)
        {
            return _documentDbProvider.DoesInteractionResourceExistAndBelongToCustomer(interactionId, customerId);
        }

        public bool DoesSessionExistAndBelongToCustomer(Guid sessionId, Guid interactionId, Guid customerId)
        {
            return _documentDbProvider.DoesSessionResourceExistAndBelongToCustomer(sessionId, interactionId, customerId);
        }

        public async Task<DateTime?> GetDateAndTimeOfSession(Guid sessionId)
        {
            var dateAndTimeOfSession = await _documentDbProvider.GetDateAndTimeOfSessionFromSessionResource(sessionId);

            return dateAndTimeOfSession;
        }

    }
}
