using System;
using Microsoft.Extensions.DependencyInjection;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Service;
using NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Helpers;
using NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Validation;


namespace NCS.DSS.ActionPlan.Ioc
{
    public class RegisterServiceProvider
    {
        public IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddTransient<IGetActionPlanHttpTriggerService, GetActionPlanHttpTriggerService>();
            services.AddTransient<IGetActionPlanByIdHttpTriggerService, GetActionPlanByIdHttpTriggerService>();
            services.AddTransient<IPostActionPlanHttpTriggerService, PostActionPlanHttpTriggerService>();
            services.AddTransient<IPatchActionPlanHttpTriggerService, PatchActionPlanHttpTriggerService>();
            services.AddTransient<IResourceHelper, ResourceHelper>();
            services.AddTransient<IValidate, Validate>();
            services.AddTransient<IHttpRequestMessageHelper, HttpRequestMessageHelper>();
            return services.BuildServiceProvider(true);
        }
    }
}
