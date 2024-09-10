using DFC.Swagger.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using System.Reflection;

namespace NCS.DSS.ActionPlan.APIDefinition
{
    public class ApiDefinition
    {
        public const string ApiTitle = "ActionPlans";
        public const string ApiDefinitionName = "API-Definition";
        public const string ApiDefRoute = ApiTitle + "/" + ApiDefinitionName;
        public const string ApiDescription = "To support the Data Collections integration with DSS PriorityCustomer has been removed and is now " +
            "recorded in the Customer API.";

        public const string ApiVersion = "4.0.0";
        private ISwaggerDocumentGenerator _swaggerDocumentGenerator;

        public ApiDefinition(ISwaggerDocumentGenerator swaggerDocumentGenerator)
        {
            _swaggerDocumentGenerator = swaggerDocumentGenerator;
        }

        [Function(ApiDefinitionName)]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ApiDefRoute)] HttpRequest req)
        {
            var swagger = _swaggerDocumentGenerator.GenerateSwaggerDocument(req, ApiTitle, ApiDescription, ApiDefinitionName, ApiVersion, Assembly.GetExecutingAssembly());

            return new OkObjectResult(swagger);

        }
    }
}
