using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using NCS.DSS.ActionPlan.Cosmos.Client;
using NCS.DSS.ActionPlan.Cosmos.Helper;

namespace NCS.DSS.ActionPlan.Cosmos.Provider
{
    public class DocumentDBProvider : IDocumentDBProvider
    {
        public bool DoesCustomerResourceExist(Guid customerId)
        {
            var collectionUri = DocumentDBHelper.CreateCustomerDocumentCollectionUri();

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return false;

            var customerQuery = client.CreateDocumentQuery<Document>(collectionUri, new FeedOptions() { MaxItemCount = 1 });
            return customerQuery.Where(x => x.Id == customerId.ToString()).Select(x => x.Id).AsEnumerable().Any();
        }

        public async Task<bool> DoesCustomerHaveATerminationDate(Guid customerId)
        {
            var collectionUri = DocumentDBHelper.CreateCustomerDocumentCollectionUri();

            var client = DocumentDBClient.CreateDocumentClient();

            var customerByIdQuery = client
                ?.CreateDocumentQuery<Document>(collectionUri, new FeedOptions { MaxItemCount = 1 })
                .Where(x => x.Id == customerId.ToString())
                .AsDocumentQuery();

            if (customerByIdQuery == null)
                return false;

            var customerQuery = await customerByIdQuery.ExecuteNextAsync<Document>();

            var customer = customerQuery?.FirstOrDefault();

            if (customer == null)
                return false;

            var dateOfTermination = customer.GetPropertyValue<DateTime?>("DateOfTermination");

            return dateOfTermination.HasValue;
        }

        public bool DoesInteractionResourceExist(Guid interactionId)
        {
            var collectionUri = DocumentDBHelper.CreateInteractionDocumentCollectionUri();

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return false;

            var interactionQuery = client.CreateDocumentQuery<Document>(collectionUri, new FeedOptions() { MaxItemCount = 1 });
            return interactionQuery.Where(x => x.Id == interactionId.ToString()).Select(x => x.Id).AsEnumerable().Any();
        }

        public async Task<List<Models.ActionPlan>> GetActionPlansForCustomerAsync(Guid customerId)
        {
            var collectionUri = DocumentDBHelper.CreateDocumentCollectionUri();

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return null;

            var actionPlansQuery = client.CreateDocumentQuery<Models.ActionPlan>(collectionUri)
                .Where(so => so.CustomerId == customerId).AsDocumentQuery();

            var actionPlans = new List<Models.ActionPlan>();

            while (actionPlansQuery.HasMoreResults)
            {
                var response = await actionPlansQuery.ExecuteNextAsync<Models.ActionPlan>();
                actionPlans.AddRange(response);
            }

            return actionPlans.Any() ? actionPlans : null;
        }

        public async Task<Models.ActionPlan> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId)
        {
            var collectionUri = DocumentDBHelper.CreateDocumentCollectionUri();

            var client = DocumentDBClient.CreateDocumentClient();

            var actionPlanForCustomerQuery = client
                ?.CreateDocumentQuery<Models.ActionPlan>(collectionUri, new FeedOptions { MaxItemCount = 1 })
                .Where(x => x.CustomerId == customerId && x.ActionPlanId == actionPlanId)
                .AsDocumentQuery();

            if (actionPlanForCustomerQuery == null)
                return null;

            var actionPlans = await actionPlanForCustomerQuery.ExecuteNextAsync<Models.ActionPlan>();

            return actionPlans?.FirstOrDefault();
        }

        public async Task<ResourceResponse<Document>> CreateActionPlanAsync(Models.ActionPlan actionPlan)
        {

            var collectionUri = DocumentDBHelper.CreateDocumentCollectionUri();

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return null;

            var response = await client.CreateDocumentAsync(collectionUri, actionPlan);

            return response;

        }

        public async Task<ResourceResponse<Document>> UpdateActionPlanAsync(Models.ActionPlan actionPlan)
        {
            var documentUri = DocumentDBHelper.CreateDocumentUri(actionPlan.ActionPlanId.GetValueOrDefault());

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return null;

            var response = await client.ReplaceDocumentAsync(documentUri, actionPlan);

            return response;
        }
    }
}