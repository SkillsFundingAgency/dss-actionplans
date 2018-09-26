using System;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.Cosmos.Helper
{
    public interface IResourceHelper
    {
        Task<bool> DoesCustomerExist(Guid customerId);
        Task<bool> IsCustomerReadOnly(Guid customerId);
        bool DoesInteractionExistAndBelongToCustomer(Guid interactionId, Guid customerId);
    }
}