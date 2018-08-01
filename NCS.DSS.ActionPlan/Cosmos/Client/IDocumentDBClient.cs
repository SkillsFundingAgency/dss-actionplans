using Microsoft.Azure.Documents.Client;

namespace NCS.DSS.ActionPlan.Cosmos.Client
{
    public interface IDocumentDBClient
    {
        DocumentClient CreateDocumentClient();
    }
}