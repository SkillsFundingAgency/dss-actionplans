using System;

namespace NCS.DSS.ActionPlan.Cosmos.Helper
{
    public interface IResourceHelper
    {
        bool DoesCustomerExist(Guid customerId);
        bool DoesInteractionExist(Guid interactionId);
    }
}