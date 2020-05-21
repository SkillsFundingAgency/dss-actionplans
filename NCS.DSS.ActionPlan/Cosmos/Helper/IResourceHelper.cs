using System;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.Cosmos.Helper
{
    public interface IResourceHelper
    {
        Task<bool> DoesCustomerExist(Guid customerId);
        bool IsCustomerReadOnly();
        bool DoesInteractionExistAndBelongToCustomer(Guid interactionGuid, Guid customerGuid);
        bool DoesSessionExistAndBelongToCustomer(Guid sessionId, Guid interactionId, Guid customerId);
        Task<DateTime?> GetDateAndTimeOfSession(Guid sessionId);
    }
}