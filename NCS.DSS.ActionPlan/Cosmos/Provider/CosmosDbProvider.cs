using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCS.DSS.ActionPlan.Models;
using Newtonsoft.Json;

namespace NCS.DSS.ActionPlan.Cosmos.Provider
{
    public class CosmosDbProvider : ICosmosDbProvider
    {
        private readonly Container _actionPlanContainer;
        private readonly Container _customerContainer;
        private readonly Container _interactionContainer;
        private readonly Container _sessionContainer;
        private readonly ILogger<CosmosDbProvider> _logger;
        private string _customerJson;

        public CosmosDbProvider(CosmosClient cosmosClient,
            IOptions<ActionPlanConfigurationSettings> configOptions,
            ILogger<CosmosDbProvider> logger)
        {
            var config = configOptions.Value;

            _actionPlanContainer = GetContainer(cosmosClient, config.DatabaseId, config.CollectionId);
            _customerContainer = GetContainer(cosmosClient, config.CustomerDatabaseId, config.CustomerCollectionId);
            _interactionContainer = GetContainer(cosmosClient, config.InteractionDatabaseId, config.InteractionCollectionId);
            _sessionContainer = GetContainer(cosmosClient, config.SessionDatabaseId, config.SessionCollectionId);
            _logger = logger;
        }

        private static Container GetContainer(CosmosClient cosmosClient, string databaseId, string collectionId)
            => cosmosClient.GetContainer(databaseId, collectionId);        

        public async Task<bool> DoesCustomerResourceExist(Guid customerId)
        {
            try
            {
                _logger.LogTrace("Checking for customer resource. Customer ID: {CustomerId}", customerId);

                var response = await _customerContainer.ReadItemAsync<Customer>(
                    customerId.ToString(),
                    PartitionKey.None);

                if (response.Resource != null)
                {
                    _logger.LogTrace("Customer exists. Customer ID: {CustomerId}", customerId);
                    _customerJson = JsonConvert.SerializeObject(response.Resource);
                    return true;
                }

                _logger.LogInformation("Customer does not exist. Customer ID: {CustomerId}", customerId);
                return false;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogError(ex, "Customer does not exist. Customer ID: {CustomerId}", customerId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking customer resource existence. Customer ID: {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<bool> DoesInteractionResourceExistAndBelongToCustomer(Guid interactionId, Guid customerId)
        {
            try
            {
                _logger.LogInformation("Checking for interaction resource for a customer. Customer ID: {CustomerId} Interaction ID: {InteractionId}", customerId, interactionId);

                string queryText = "SELECT VALUE COUNT(1) FROM interactions i WHERE i.id = @interactionId AND i.CustomerId = @customerId";
                var queryDefinition = new QueryDefinition(queryText)
                    .WithParameter("@interactionId", interactionId.ToString())
                    .WithParameter("@customerId", customerId.ToString());

                using var iterator = _interactionContainer.GetItemQueryIterator<dynamic>(queryDefinition);

                if (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    var interactionFound = response.FirstOrDefault() > 0;

                    if (interactionFound)
                    {
                        _logger.LogInformation("Interaction for customer exists. Customer ID: {CustomerId} Interaction ID: {InteractionId}", customerId, interactionId);
                    }
                    return interactionFound;
                }

                return false;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Interaction for customer is not found. Customer ID: {CustomerId} Interaction ID: {InteractionId}", customerId, interactionId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking interaction resource for a customer. Customer ID: {CustomerId} Interaction ID: {InteractionId}", customerId, interactionId);
                throw;
            }
        }

        public async Task<bool> DoesSessionResourceExistAndBelongToCustomer(Guid sessionId, Guid interactionId, Guid customerId)
        {
            _logger.LogInformation("Checking for session resource for a customer. Customer ID: {CustomerId} Interaction ID: {InteractionId} Session ID: {SessionId}", customerId, interactionId, sessionId);
            try
            {
                string queryText = "SELECT VALUE COUNT(1) FROM sessions s WHERE s.id = @sessionId AND s.InteractionId = @interactionId AND s.CustomerId = @customerId";
                var queryDefinition = new QueryDefinition(queryText)
                    .WithParameter("@sessionId", sessionId.ToString())
                    .WithParameter("@interactionId", interactionId.ToString())
                    .WithParameter("@customerId", customerId.ToString());

                using var iterator = _sessionContainer.GetItemQueryIterator<int>(queryDefinition);

                if (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    var sessionExists = response.FirstOrDefault() > 0;
                    if (sessionExists)
                    {
                        _logger.LogInformation("Session for customer exists. Customer ID: {CustomerId} Interaction ID: {InteractionId} Session ID: {SessionId}", customerId, interactionId, sessionId);
                    }
                    return sessionExists;
                }

                return false;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Session for customer is not found. Customer ID: {CustomerId} Interaction ID: {InteractionId} Session ID: {SessionId}", customerId, interactionId, sessionId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking session resource for a customer. Customer ID: {CustomerId} Interaction ID: {InteractionId} Session ID: {SessionId}", customerId, interactionId, sessionId);
                throw;
            }
        }

        public async Task<DateTime?> GetDateAndTimeOfSessionFromSessionResource(Guid sessionId)
        {
            _logger.LogInformation("Attempting to retrieve DateAndTimeOfSession. Session ID: {SessionId}", sessionId);
            try
            {
                string queryText = "SELECT TOP 1 * FROM c WHERE c.id = @sessionId";
                var queryDefinition = new QueryDefinition(queryText)
                    .WithParameter("@sessionId", sessionId.ToString());

                using var iterator = _sessionContainer.GetItemQueryIterator<dynamic>(queryDefinition);

                var response = await iterator.ReadNextAsync();

                return GetDateAndTimeOfSessionFromResponse(response);
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("Session does not exist. Session ID: {SessionId}", sessionId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving DateAndTimeOfSession. Session ID: {SessionId}", sessionId);
                throw;
            }
        }

        private DateTime? GetDateAndTimeOfSessionFromResponse(FeedResponse<dynamic> response)
        {
            return response
                .Select(item => (DateTime?)item["DateandTimeOfSession"])
                .FirstOrDefault();
        }

        public async Task<string> GetActionPlanForCustomerToUpdateAsync(Guid customerId, Guid actionPlanId)
        {
            var actionPlan = await GetActionPlanForCustomerAsync(customerId, actionPlanId);
            
            return JsonConvert.SerializeObject(actionPlan);
        }

        public async Task<Models.ActionPlan> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId)
        {
            _logger.LogInformation("Retrieving Action Plan for Customer. Customer ID: {CustomerId}. Action Plan ID: {ActionPlanId}.", customerId, actionPlanId);

            try
            {
                var query = _actionPlanContainer.GetItemLinqQueryable<Models.ActionPlan>()
                    .Where(x => x.CustomerId == customerId && x.ActionPlanId == actionPlanId)
                    .ToFeedIterator();

                var response = await query.ReadNextAsync();
                if (response.Any())
                {
                    _logger.LogInformation("Action Plan retrieved successfully. Customer ID: {CustomerId}. Action Plan ID: {ActionPlanId}.", customerId, actionPlanId);
                    return response?.FirstOrDefault();
                }

                _logger.LogWarning("Action Plan not found. Customer ID: {CustomerId}. Action Plan ID: {ActionPlanId}.", customerId, actionPlanId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving Action Plan. Customer ID: {CustomerId}. Action Plan ID: {ActionPlanId}.", customerId, actionPlanId);
                throw;
            }
        }

        public async Task<List<Models.ActionPlan>> GetActionPlansForCustomerAsync(Guid customerId)
        {
            _logger.LogInformation("Retrieving Action Plans for Customer. Customer ID: {CustomerId}.", customerId);

            try
            {
                var actionPlans = new List<Models.ActionPlan>();
                var query = _actionPlanContainer.GetItemLinqQueryable<Models.ActionPlan>()
                    .Where(x => x.CustomerId == customerId)
                    .ToFeedIterator();

                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    actionPlans.AddRange(response);
                }

                _logger.LogInformation("Retrieved {Count} Action Plan(s) for Customer with ID: {CustomerId}.", actionPlans.Count, customerId);
                return actionPlans;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving Action Plans. Customer ID: {CustomerId}.", customerId);
                throw;
            }
        }

        public async Task<ItemResponse<Models.ActionPlan>> CreateActionPlanAsync(Models.ActionPlan actionPlan)
        {
            if (actionPlan == null)
            {
                _logger.LogError("actionPlan object is null. Creation aborted.");
                throw new ArgumentNullException(nameof(actionPlan), "ActionPlan cannot be null.");
            }

            _logger.LogInformation("Creating Action Plan with ID: {ActionPlanId}", actionPlan.ActionPlanId);

            try
            {
                var response = await _actionPlanContainer.CreateItemAsync(actionPlan, PartitionKey.None);
                _logger.LogInformation("Successfully created Action Plan with ID: {ActionPlanID}", actionPlan.ActionPlanId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Action Plan with ID: {ActionPlanId}", actionPlan.ActionPlanId);
                throw;
            }
        }

        public async Task<ItemResponse<Models.ActionPlan>> UpdateActionPlanAsync(string actionPlanJson, Guid actionPlanId)
        {
            if (string.IsNullOrEmpty(actionPlanJson))
            {
                _logger.LogError("actionPlanJson object is null. Update aborted.");
                throw new ArgumentNullException(nameof(actionPlanJson), "ActionPlan object cannot be null.");
            }

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(actionPlanJson);

            _logger.LogInformation("Updating Action Plan with ID: {ActionPlanId}", actionPlanId);

            try
            {
                var response = await _actionPlanContainer.ReplaceItemAsync(actionPlan, actionPlanId.ToString());
                _logger.LogInformation("Successfully updated Action Plan with ID: {ActionPlanId}", actionPlanId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update Action Plan with ID: {ActionPlanId}", actionPlanId);
                throw;
            }
        }

        public string GetCustomerJson()
        {
            return _customerJson;
        }
    }
}