using System;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.Cosmos.Helper
{
    public interface IResourceHelper
    {
        Task<bool> DoesCustomerExist(Guid customerId);
        Task<bool> IsCustomerReadOnly(Guid customerId);
        bool DoesInteractionExistAndBelongToCustomer(Guid interactionGuid, Guid customerGuid);
        Task<DateTime?> GetDateAndTimeOfSession(Guid sessionId);
    }
}