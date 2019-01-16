using System;
using DFC.JSON.Standard;
using NCS.DSS.ActionPlan.Models;
using NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.ReferenceData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace NCS.DSS.ActionPlan.Tests.ServiceTests
{
    [TestFixture]
    public class ActionPlanPatchServiceTests
    {
        private IJsonHelper _jsonHelper;
        private IActionPlanPatchService _actionPlanPatchService;
        private ActionPlanPatch _actionPlanPatch;
        private string _json;


        [SetUp]
        public void Setup()
        {
            _jsonHelper = Substitute.For<JsonHelper>();
            _actionPlanPatchService = Substitute.For<ActionPlanPatchService>(_jsonHelper);
            _actionPlanPatch = Substitute.For<ActionPlanPatch>();

            _json = JsonConvert.SerializeObject(_actionPlanPatch);
        }

        [Test]
        public void ActionPlanPatchServiceTests_ReturnsNull_WhenActionPlanPatchIsNull()
        {
            var result = _actionPlanPatchService.Patch(string.Empty, Arg.Any<ActionPlanPatch>());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckDateActionPlanCreatedIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch() { DateActionPlanCreated = DateTime.MaxValue };

            var updated = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var jsonObject = (JObject) JsonConvert.DeserializeObject(updated);

            var dateActionPlanCreated = (DateTime) jsonObject["DateActionPlanCreated"];

            // Assert
            Assert.AreEqual(DateTime.MaxValue, dateActionPlanCreated);
        }


        [Test]
        public void ActionPlanPatchServiceTests_CheckCustomerCharterShownToCustomerIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { CustomerCharterShownToCustomer = true };

            var updated = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var jsonObject = (JObject)JsonConvert.DeserializeObject(updated);

            var customerCharterShownToCustomer = (bool) jsonObject["CustomerCharterShownToCustomer"];

            // Assert
            Assert.AreEqual(true, customerCharterShownToCustomer);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckDateAndTimeCharterShownIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new Models.ActionPlanPatch { DateAndTimeCharterShown = DateTime.MaxValue };

            var updated = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var jsonObject = (JObject)JsonConvert.DeserializeObject(updated);

            var dateAndTimeCharterShown = (DateTime)jsonObject["DateAndTimeCharterShown"];

            // Assert
            Assert.AreEqual(DateTime.MaxValue, dateAndTimeCharterShown);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckDateActionPlanSentToCustomerIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new Models.ActionPlanPatch { DateActionPlanSentToCustomer = DateTime.MaxValue };

            var updated = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var jsonObject = (JObject)JsonConvert.DeserializeObject(updated);

            var dateActionPlanSentToCustomer = (DateTime)jsonObject["DateActionPlanSentToCustomer"];

            // Assert
            Assert.AreEqual(DateTime.MaxValue, dateActionPlanSentToCustomer);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckActionPlanDeliveryMethodIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { ActionPlanDeliveryMethod = ActionPlanDeliveryMethod.Paper };
            
            var updated = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var jsonObject = (JObject)JsonConvert.DeserializeObject(updated);

            var actionPlanDeliveryMethod = (ActionPlanDeliveryMethod) int.Parse(jsonObject["ActionPlanDeliveryMethod"].ToString());

            // Assert
            Assert.AreEqual(ActionPlanDeliveryMethod.Paper, actionPlanDeliveryMethod);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckDateActionPlanAcknowledgedIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new Models.ActionPlanPatch { DateActionPlanAcknowledged = DateTime.MaxValue };

            var updated = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var jsonObject = (JObject)JsonConvert.DeserializeObject(updated);

            var dateActionPlanAcknowledged = (DateTime)jsonObject["DateActionPlanAcknowledged"];

            // Assert
            Assert.AreEqual(DateTime.MaxValue, dateActionPlanAcknowledged);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckPriorityCustomerIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new Models.ActionPlanPatch { PriorityCustomer = PriorityCustomer.NotAPriorityCustomer };

            var updated = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var jsonObject = (JObject)JsonConvert.DeserializeObject(updated);

            var priorityCustomer = (PriorityCustomer)int.Parse(jsonObject["PriorityCustomer"].ToString());
            
            // Assert
            Assert.AreEqual(PriorityCustomer.NotAPriorityCustomer, priorityCustomer);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckCurrentSituationIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { CurrentSituation = "Current" };

            var updated = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var jsonObject = (JObject)JsonConvert.DeserializeObject(updated);

            var currentSituation = jsonObject["CurrentSituation"].ToString();

            // Assert
            Assert.AreEqual("Current", currentSituation);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckLastModifiedDateIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { LastModifiedDate = DateTime.MaxValue };
            
            var updated = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var jsonObject = (JObject)JsonConvert.DeserializeObject(updated);

            var lastModifiedDate = (DateTime)jsonObject["LastModifiedDate"];

            // Assert
            Assert.AreEqual(DateTime.MaxValue, lastModifiedDate);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckLastModifiedTouchpointIdIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { LastModifiedTouchpointId = "0000000111" };

            var updated = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var jsonObject = (JObject)JsonConvert.DeserializeObject(updated);

            var lastModifiedTouchpointId = jsonObject["LastModifiedTouchpointId"].ToString();
            
            // Assert
            Assert.AreEqual("0000000111", lastModifiedTouchpointId);
        }

    }
}
