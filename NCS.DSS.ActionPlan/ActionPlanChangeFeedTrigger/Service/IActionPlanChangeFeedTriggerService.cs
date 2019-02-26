using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace NCS.DSS.ActionPlan.ActionPlanChangeFeedTrigger.Service
{
    public interface IActionPlanChangeFeedTriggerService
    {
        Task SendMessageToChangeFeedQueueAsync(Document document);
    }
}