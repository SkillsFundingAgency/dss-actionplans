using DFC.JSON.Standard;
using Moq;
using NCS.DSS.ActionPlan.Models;
using NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.ReferenceData;
using Newtonsoft.Json;
using NUnit.Framework;
using System;

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
            _jsonHelper = new JsonHelper();
            _actionPlanPatchService = new ActionPlanPatchService(_jsonHelper);
            _actionPlanPatch = new ActionPlanPatch();

            _json = JsonConvert.SerializeObject(_actionPlanPatch);
        }

        [Test]
        public void ActionPlanPatchServiceTests_ReturnsNull_WhenActionPlanPatchIsNull()
        {
            var result = _actionPlanPatchService.Patch(string.Empty, It.IsAny<ActionPlanPatch>());

            // Assert
            Assert.That(result,Is.Null);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckDateActionPlanCreatedIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch() { DateActionPlanCreated = DateTime.MaxValue };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            // Assert
            Assert.That(DateTime.MaxValue == actionPlan.DateActionPlanCreated);
        }


        [Test]
        public void ActionPlanPatchServiceTests_CheckCustomerCharterShownToCustomerIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { CustomerCharterShownToCustomer = true };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            // Assert
            Assert.That(true == actionPlan.CustomerCharterShownToCustomer);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckDateAndTimeCharterShownIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new Models.ActionPlanPatch { DateAndTimeCharterShown = DateTime.MaxValue };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            // Assert
            Assert.That(DateTime.MaxValue == actionPlan.DateAndTimeCharterShown);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckDateActionPlanSentToCustomerIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new Models.ActionPlanPatch { DateActionPlanSentToCustomer = DateTime.MaxValue };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            // Assert
            Assert.That(DateTime.MaxValue == actionPlan.DateActionPlanSentToCustomer);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckActionPlanDeliveryMethodIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { ActionPlanDeliveryMethod = ActionPlanDeliveryMethod.Paper };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            // Assert
            Assert.That(ActionPlanDeliveryMethod.Paper == actionPlan.ActionPlanDeliveryMethod);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckDateActionPlanAcknowledgedIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new Models.ActionPlanPatch { DateActionPlanAcknowledged = DateTime.MaxValue };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            // Assert
            Assert.That(DateTime.MaxValue == actionPlan.DateActionPlanAcknowledged);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckCurrentSituationIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { CurrentSituation = "Current" };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            // Assert
            Assert.That("Current" == actionPlan.CurrentSituation);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckLastModifiedDateIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { LastModifiedDate = DateTime.MaxValue };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            // Assert
            Assert.That(DateTime.MaxValue == actionPlan.LastModifiedDate);
        }

        [Test]
        public void ActionPlanPatchServiceTests_CheckLastModifiedTouchpointIdIsUpdated_WhenPatchIsCalled()
        {
            var actionPlanPatch = new ActionPlanPatch { LastModifiedTouchpointId = "0000000111" };

            var patchedActionPlan = _actionPlanPatchService.Patch(_json, actionPlanPatch);

            var actionPlan = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);

            // Assert
            Assert.That("0000000111" == actionPlan.LastModifiedTouchpointId);
        }
        [Test]
        public void CustomerPatchServiceTests_CheckCustomerSatisfactionUpdated_WhenPatchIsCalled()
        {
            // Arrange
            var satisfactionPatch = new ActionPlanPatch { CustomerSatisfaction = CustomerSatisfaction.Satisfied };

            // Act
            var patchedCustomer = _actionPlanPatchService.Patch(_json, satisfactionPatch);
            var satisfaction = JsonConvert.DeserializeObject<Models.ActionPlanPatch>(patchedCustomer);

            // Assert
            Assert.That(CustomerSatisfaction.Satisfied == satisfaction.CustomerSatisfaction);
        }
    }
}