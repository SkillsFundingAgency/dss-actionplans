using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.Helpers
{
    public class HttpRequestMessageHelper : IHttpRequestMessageHelper
    {
        public async Task<T> GetActionPlanFromRequest<T>(HttpRequestMessage req)
        {
            return await req.Content.ReadAsAsync<T>();
        }
    }
}
