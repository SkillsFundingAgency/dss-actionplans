﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using NCS.DSS.ActionPlan.Cosmos.Client;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using Newtonsoft.Json.Linq;

namespace NCS.DSS.ActionPlan.Cosmos.Provider
{
    public class DocumentDBProvider : IDocumentDBProvider
    {
        public async Task<bool> DoesCustomerResourceExist(Guid customerId)
        {
            var documentUri = DocumentDBHelper.CreateCustomerDocumentUri(customerId);

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return false;
            try
            {
                var response = await client.ReadDocumentAsync(documentUri);
                if (response.Resource != null)
                    return true;
            }
            catch (DocumentClientException)
            {
                return false;
            }

            return false;
        }
        
        public bool DoesInteractionResourceExistAndBelongToCustomer(Guid interactionId, Guid customerId)
        {
            var collectionUri = DocumentDBHelper.CreateInteractionDocumentCollectionUri();

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return false;

            try
            {
                var query = client.CreateDocumentQuery<long>(collectionUri, new SqlQuerySpec()
                {
                    QueryText = "SELECT VALUE COUNT(1) FROM interactions i " +
                                "WHERE i.id = @interactionId " +
                                "AND i.CustomerId = @customerId",

                    Parameters = new SqlParameterCollection()
                    {
                        new SqlParameter("@interactionId", interactionId),
                        new SqlParameter("@customerId", customerId)
                    }
                }).AsEnumerable().FirstOrDefault();

                return Convert.ToBoolean(Convert.ToInt16(query));
            }
            catch (DocumentQueryException)
            {
                return false;
            }

        }

        public async Task<bool> DoesCustomerHaveATerminationDate(Guid customerId)
        {
            var documentUri = DocumentDBHelper.CreateCustomerDocumentUri(customerId);

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return false;

            try
            {
                var response = await client.ReadDocumentAsync(documentUri);

                var dateOfTermination = response.Resource?.GetPropertyValue<DateTime?>("DateOfTermination");

                return dateOfTermination.HasValue;
            }
            catch (DocumentClientException)
            {
                return false;
            }
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

        public async Task<string> GetActionPlanForCustomerToUpdateAsync(Guid customerId, Guid actionPlanId)
        {
            var collectionUri = DocumentDBHelper.CreateDocumentCollectionUri();

            var client = DocumentDBClient.CreateDocumentClient();

            var actionPlanForCustomerQuery = client
                ?.CreateDocumentQuery<Models.ActionPlan>(collectionUri, new FeedOptions { MaxItemCount = 1 })
                .Where(x => x.CustomerId == customerId && x.ActionPlanId == actionPlanId)
                .AsDocumentQuery();

            if (actionPlanForCustomerQuery == null)
                return null;

            var actionPlans = await actionPlanForCustomerQuery.ExecuteNextAsync();

            return actionPlans?.FirstOrDefault()?.ToString();
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

        public async Task<ResourceResponse<Document>> UpdateActionPlanAsync(string actionPlanJson, Guid actionPlanId)
        {
            var documentUri = DocumentDBHelper.CreateDocumentUri(actionPlanId);

            var client = DocumentDBClient.CreateDocumentClient();

            if (client == null)
                return null;

            var actionPlanDocumentJObject = JObject.Parse(actionPlanJson);

            var response = await client.ReplaceDocumentAsync(documentUri, actionPlanDocumentJObject);

            return response;
        }
    }
}