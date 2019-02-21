using System;
using NCS.DSS.ActionPlan.Helpers;
using NCS.DSS.ActionPlan.Models;
using NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.ReferenceData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;

namespace NCS.DSS.ActionPlan.Tests.ServiceTests
{
    [TestFixture]
    public class ActionPlanPatchServiceTests
    {
        private JsonHelper _jsonHelper;
        private IActionPlanPatchService _actionPlanPatchService;
        private ActionPlanPatch _actionPlanPatch;
        private string _json;


        [SetUp]
        public void Setup()
        {
            _jsonHelper = Substitute.For<JsonHelper>();
            _actionPlanPatchService = Substitute.For<ActionPlanPatchService>();
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

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            var dateActionPlanCreated = actionPlan.DateActionPlanCreated;

            // Assert
            Assert.AreEqual(DateTime.MaxValue, dateActionPlanCreated);
        }


        [Test]
        public void ActionPlanPatchServiceTests_CheckCustomerCharterShownToCustomerIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { CustomerCharterShownToCustomer = true };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            var customerCharterShownToCustomer = actionPlan.CustomerCharterShownToCustomer;

            // Assert
            Assert.AreEqual(true, customerCharterShownToCustomer);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckDateAndTimeCharterShownIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new Models.ActionPlanPatch { DateAndTimeCharterShown = DateTime.MaxValue };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            var dateAndTimeCharterShown = actionPlan.DateAndTimeCharterShown;

            // Assert
            Assert.AreEqual(DateTime.MaxValue, dateAndTimeCharterShown);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckDateActionPlanSentToCustomerIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new Models.ActionPlanPatch { DateActionPlanSentToCustomer = DateTime.MaxValue };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            var dateActionPlanSentToCustomer = actionPlan.DateActionPlanSentToCustomer;

            // Assert
            Assert.AreEqual(DateTime.MaxValue, dateActionPlanSentToCustomer);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckActionPlanDeliveryMethodIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { ActionPlanDeliveryMethod = ActionPlanDeliveryMethod.Paper };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            var actionPlanDeliveryMethod = actionPlan.ActionPlanDeliveryMethod;

            // Assert
            Assert.AreEqual(ActionPlanDeliveryMethod.Paper, actionPlanDeliveryMethod);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckDateActionPlanAcknowledgedIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new Models.ActionPlanPatch { DateActionPlanAcknowledged = DateTime.MaxValue };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            var dateActionPlanAcknowledged = actionPlan.DateActionPlanAcknowledged;

            // Assert
            Assert.AreEqual(DateTime.MaxValue, dateActionPlanAcknowledged);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckPriorityCustomerIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new Models.ActionPlanPatch { PriorityCustomer = PriorityCustomer.NotAPriorityCustomer };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            var priorityCustomer = actionPlan.PriorityCustomer;
            
            // Assert
            Assert.AreEqual(PriorityCustomer.NotAPriorityCustomer, priorityCustomer);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckCurrentSituationIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { CurrentSituation = "Current" };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            var currentSituation = actionPlan.CurrentSituation;

            // Assert
            Assert.AreEqual("Current", currentSituation);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckLastModifiedDateIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { LastModifiedDate = DateTime.MaxValue };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            var lastModifiedDate = actionPlan.LastModifiedDate;

            // Assert
            Assert.AreEqual(DateTime.MaxValue, lastModifiedDate);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckLastModifiedTouchpointIdIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { LastModifiedTouchpointId = "0000000111" };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            var lastModifiedTouchpointId = actionPlan.LastModifiedTouchpointId;
            
            // Assert
            Assert.AreEqual("0000000111", lastModifiedTouchpointId);
        }

    }
}
