using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Service;
using NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Ioc;
using NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Validation;

[assembly: WebJobsStartup(typeof(WebJobsExtensionStartup), "Web Jobs Extension Startup")]

namespace NCS.DSS.ActionPlan.Ioc
{
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddDependencyInjection();

            builder.Services.AddScoped<IResourceHelper, ResourceHelper>();
            builder.Services.AddScoped<IValidate, Validate>();
            builder.Services.AddScoped<ILoggerHelper, LoggerHelper>();
            builder.Services.AddScoped<IHttpRequestHelper, HttpRequestHelper>();
            builder.Services.AddScoped<IHttpResponseMessageHelper, HttpResponseMessageHelper>();
            builder.Services.AddScoped<IJsonHelper, JsonHelper>();
            builder.Services.AddScoped<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();
            builder.Services.AddScoped<IGetActionPlanHttpTriggerService, GetActionPlanHttpTriggerService>();
            builder.Services.AddScoped<IGetActionPlanByIdHttpTriggerService, GetActionPlanByIdHttpTriggerService>();
            builder.Services.AddScoped<IPostActionPlanHttpTriggerService, PostActionPlanHttpTriggerService>();
            builder.Services.AddScoped<IPatchActionPlanHttpTriggerService, PatchActionPlanHttpTriggerService>();
        }
    }
}