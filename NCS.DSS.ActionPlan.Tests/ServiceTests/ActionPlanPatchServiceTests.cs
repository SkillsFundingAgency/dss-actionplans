using System;
using DFC.JSON.Standard;
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

            var actionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var dateActionPlanCreated = actionPlan.DateActionPlanCreated;

            // Assert
            Assert.AreEqual(DateTime.MaxValue, dateActionPlanCreated);
        }


        [Test]
        public void ActionPlanPatchServiceTests_CheckCustomerCharterShownToCustomerIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { CustomerCharterShownToCustomer = true };

            var actionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var customerCharterShownToCustomer = actionPlan.CustomerCharterShownToCustomer;

            // Assert
            Assert.AreEqual(true, customerCharterShownToCustomer);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckDateAndTimeCharterShownIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new Models.ActionPlanPatch { DateAndTimeCharterShown = DateTime.MaxValue };

            var actionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var dateAndTimeCharterShown = actionPlan.DateAndTimeCharterShown;

            // Assert
            Assert.AreEqual(DateTime.MaxValue, dateAndTimeCharterShown);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckDateActionPlanSentToCustomerIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new Models.ActionPlanPatch { DateActionPlanSentToCustomer = DateTime.MaxValue };

            var actionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var dateActionPlanSentToCustomer = actionPlan.DateActionPlanSentToCustomer;

            // Assert
            Assert.AreEqual(DateTime.MaxValue, dateActionPlanSentToCustomer);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckActionPlanDeliveryMethodIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { ActionPlanDeliveryMethod = ActionPlanDeliveryMethod.Paper };
            
            var actionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlanDeliveryMethod = actionPlan.ActionPlanDeliveryMethod;

            // Assert
            Assert.AreEqual(ActionPlanDeliveryMethod.Paper, actionPlanDeliveryMethod);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckDateActionPlanAcknowledgedIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new Models.ActionPlanPatch { DateActionPlanAcknowledged = DateTime.MaxValue };

            var actionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var dateActionPlanAcknowledged = actionPlan.DateActionPlanAcknowledged;

            // Assert
            Assert.AreEqual(DateTime.MaxValue, dateActionPlanAcknowledged);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckPriorityCustomerIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new Models.ActionPlanPatch { PriorityCustomer = PriorityCustomer.NotAPriorityCustomer };

            var actionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var priorityCustomer = actionPlan.PriorityCustomer;
            
            // Assert
            Assert.AreEqual(PriorityCustomer.NotAPriorityCustomer, priorityCustomer);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckCurrentSituationIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { CurrentSituation = "Current" };

            var actionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var currentSituation = actionPlan.CurrentSituation;

            // Assert
            Assert.AreEqual("Current", currentSituation);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckLastModifiedDateIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { LastModifiedDate = DateTime.MaxValue };
            
            var actionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var lastModifiedDate = actionPlan.LastModifiedDate;

            // Assert
            Assert.AreEqual(DateTime.MaxValue, lastModifiedDate);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckLastModifiedTouchpointIdIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { LastModifiedTouchpointId = "0000000111" };

            var actionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var lastModifiedTouchpointId = actionPlan.LastModifiedTouchpointId;
            
            // Assert
            Assert.AreEqual("0000000111", lastModifiedTouchpointId);
        }

    }
}
