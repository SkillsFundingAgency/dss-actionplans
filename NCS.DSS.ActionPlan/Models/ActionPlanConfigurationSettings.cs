namespace NCS.DSS.ActionPlan.Models
{
    public class ActionPlanConfigurationSettings
    {
        public required string CosmosDbEndpoint { get; set; }
        public required string ActionPlanConnectionString { get; set; }
        public required string CollectionId { get; set; }
        public required string CustomerCollectionId { get; set; }
        public required string CustomerDatabaseId { get; set; }
        public required string DatabaseId { get; set; }
        public required string InteractionCollectionId { get; set; }
        public required string InteractionDatabaseId { get; set; }
        public required string QueueName { get; set; }
        public required string ServiceBusConnectionString { get; set; }
        public required string SessionCollectionId { get; set; }        
        public required string SessionDatabaseId { get; set; }
    }
}
