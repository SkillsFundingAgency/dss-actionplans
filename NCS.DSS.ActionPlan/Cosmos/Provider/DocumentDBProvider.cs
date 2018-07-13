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
        private readonly DocumentDBHelper _documentDbHelper;
        private readonly DocumentDBClient _databaseClient;

        public DocumentDBProvider()
        {
            _documentDbHelper = new DocumentDBHelper();
            _databaseClient = new DocumentDBClient();
        }

        public bool DoesCustomerResourceExist(Guid customerId)
        {
            var collectionUri = _documentDbHelper.CreateCustomerDocumentCollectionUri();

            var client = _databaseClient.CreateDocumentClient();

            if (client == null)
                return false;

            var customerQuery = client.CreateDocumentQuery<Document>(collectionUri, new FeedOptions() { MaxItemCount = 1 });
            return customerQuery.Where(x => x.Id == customerId.ToString()).Select(x => x.Id).AsEnumerable().Any();
        }

        public bool DoesInteractionResourceExist(Guid interactionId)
        {
            var collectionUri = _documentDbHelper.CreateInteractionDocumentCollectionUri();

            var client = _databaseClient.CreateDocumentClient();

            if (client == null)
                return false;

            var interactionQuery = client.CreateDocumentQuery<Document>(collectionUri, new FeedOptions() { MaxItemCount = 1 });
            return interactionQuery.Where(x => x.Id == interactionId.ToString()).Select(x => x.Id).AsEnumerable().Any();
        }

        public async Task<List<Models.ActionPlan>> GetActionPlansForCustomerAsync(Guid customerId)
        {
            var collectionUri = _documentDbHelper.CreateDocumentCollectionUri();

            var client = _databaseClient.CreateDocumentClient();

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
            var collectionUri = _documentDbHelper.CreateDocumentCollectionUri();

            var client = _databaseClient.CreateDocumentClient();

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

            var collectionUri = _documentDbHelper.CreateDocumentCollectionUri();

            var client = _databaseClient.CreateDocumentClient();

            if (client == null)
                return null;

            var response = await client.CreateDocumentAsync(collectionUri, actionPlan);

            return response;

        }

        public async Task<ResourceResponse<Document>> UpdateActionPlanAsync(Models.ActionPlan actionPlan)
        {
            var documentUri = _documentDbHelper.CreateDocumentUri(actionPlan.ActionPlanId.GetValueOrDefault());

            var client = _databaseClient.CreateDocumentClient();

            if (client == null)
                return null;

            var response = await client.ReplaceDocumentAsync(documentUri, actionPlan);

            return response;
        }
    }
}