using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.Helpers
{
    public interface IHttpRequestMessageHelper
    {
        Task<T> GetActionPlanFromRequest<T>(HttpRequestMessage req);
        Guid? GetTouchpointId(HttpRequestMessage req);
    }
}