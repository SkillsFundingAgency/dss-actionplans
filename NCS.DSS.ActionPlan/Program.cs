using Azure.Identity;
using Azure.Messaging.ServiceBus;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.Cosmos.Provider;
using NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Service;
using NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Models;
using NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.ServiceBus;
using NCS.DSS.ActionPlan.Validation;

namespace NCS.DSS.ActionPlan
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWebApplication()
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;
                    services.AddOptions<ActionPlanConfigurationSettings>()
                        .Bind(configuration);

                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.ConfigureFunctionsApplicationInsights();
                    services.AddSingleton<IResourceHelper, ResourceHelper>();
                    services.AddSingleton<IValidate, Validate>();
                    services.AddSingleton<IHttpRequestHelper, HttpRequestHelper>();
                    services.AddSingleton<IHttpResponseMessageHelper, HttpResponseMessageHelper>();
                    services.AddSingleton<IJsonHelper, JsonHelper>();
                    services.AddSingleton<ICosmosDbProvider, CosmosDbProvider>();
                    services.AddScoped<IActionPlanPatchService, ActionPlanPatchService>();
                    services.AddScoped<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();
                    services.AddScoped<IGetActionPlanHttpTriggerService, GetActionPlanHttpTriggerService>();
                    services.AddScoped<IGetActionPlanByIdHttpTriggerService, GetActionPlanByIdHttpTriggerService>();
                    services.AddScoped<IPostActionPlanHttpTriggerService, PostActionPlanHttpTriggerService>();
                    services.AddScoped<IPatchActionPlanHttpTriggerService, PatchActionPlanHttpTriggerService>();
                    services.AddSingleton<IConvertToDynamic, ConvertToDynamic>();
                    services.AddSingleton<IActionPlanServiceBusClient, ActionPlanServiceBusClient>();

                    services.AddSingleton(s =>
                    {
                        var logger = s.GetRequiredService<ILogger<Program>>();

                        var connectionString = configuration["ActionPlanConnectionString"];
                        var endpoint = configuration["CosmosDbEndpoint"];

                        var options = new CosmosClientOptions
                        {
                            ConnectionMode = ConnectionMode.Gateway
                        };

                        if (!string.IsNullOrWhiteSpace(endpoint))
                        {
                            logger.LogInformation("Using DefaultAzureCredential for Cosmos DB (managed identity)");
                            return new CosmosClient(endpoint, new DefaultAzureCredential(), options);
                        }
                        else if (!string.IsNullOrWhiteSpace(connectionString))
                        {
                            logger.LogInformation("No managed identity found: using Cosmos DB connection string (local development)");
                            return new CosmosClient(connectionString, options);
                        }
                        else
                        {
                            throw new InvalidOperationException("Neither CosmosDbEndpoint or a ConnectionString are configured");
                        }
                    });

                    services.AddSingleton(s =>
                    {
                        var settings = s.GetRequiredService<IOptions<ActionPlanConfigurationSettings>>().Value;

                        return new ServiceBusClient(settings.ServiceBusConnectionString);
                    });

                    services.Configure<LoggerFilterOptions>(options =>
                    {
                        LoggerFilterRule toRemove = options.Rules.FirstOrDefault(rule => rule.ProviderName
                            == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
                        options.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
                        if (toRemove is not null)
                        {
                            options.Rules.Remove(toRemove);
                        }
                    });
                })
                .Build();
            await host.RunAsync();
        }
    }
}
