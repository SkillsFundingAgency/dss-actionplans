using System;
using NCS.DSS.ActionPlan.ReferenceData;
using NSubstitute;
using NUnit.Framework;

namespace NCS.DSS.ActionPlan.Tests.ModelTests
{
    [TestFixture]
    public class ActionPlanModelTests
    {

        [Test]
        public void ActionPlanTests_PopulatesDefaultValues_WhenSetDefaultValuesIsCalled()
        {
            var actionPlan = new Models.ActionPlan();
            actionPlan.SetDefaultValues();

            // Assert
            Assert.IsNotNull(actionPlan.LastModifiedDate);
            Assert.AreEqual(false, actionPlan.CustomerCharterShownToCustomer);
            Assert.AreEqual(PriorityCustomer.NotAPriorityCustomer, actionPlan.PriorityCustomer);
        }

        [Test]
        public void ActionPlanTests_CheckLastModifiedDateDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            var actionPlan = new Models.ActionPlan { LastModifiedDate = DateTime.MaxValue };

            actionPlan.SetDefaultValues();

            // Assert
            Assert.AreEqual(DateTime.MaxValue, actionPlan.LastModifiedDate);
        }

        [Test]
        public void ActionPlanTests_CheckCustomerCharterShownToCustomerDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            var actionPlan = new Models.ActionPlan { CustomerCharterShownToCustomer = true };

            actionPlan.SetDefaultValues();

            // Assert
            Assert.AreEqual(true, actionPlan.CustomerCharterShownToCustomer);
        }

        [Test]
        public void ActionPlanTests_CheckNotAPriorityCustomerToCustomerDoesNotGetPopulated_WhenSetDefaultValuesIsCalled()
        {
            var actionPlan = new Models.ActionPlan { PriorityCustomer = PriorityCustomer.NotAPriorityCustomer };

            actionPlan.SetDefaultValues();

            // Assert
            Assert.AreEqual(PriorityCustomer.NotAPriorityCustomer, actionPlan.PriorityCustomer);
        }

        [Test]
        public void ActionPlanTests_CheckActionPlanIdIsSet_WhenSetIdsIsCalled()
        {
            var actionPlan = new Models.ActionPlan();

            actionPlan.SetIds(actionPlan, Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<string>());

            // Assert
            Assert.AreNotSame(Guid.Empty, actionPlan.ActionPlanId);
        }

        [Test]
        public void ActionPlanTests_CheckCustomerIdIsSet_WhenSetIdsIsCalled()
        {
            var actionPlan = new Models.ActionPlan();

            var customerId = Guid.NewGuid();
            actionPlan.SetIds(actionPlan, customerId, Arg.Any<Guid>(), Arg.Any<string>());

            // Assert
            Assert.AreEqual(customerId, actionPlan.CustomerId);
        }

        [Test]
        public void ActionPlanTests_CheckInteractionIdIsSet_WhenSetIdsIsCalled()
        {
            var actionPlan = new Models.ActionPlan();

            var interactionId = Guid.NewGuid();
            actionPlan.SetIds(actionPlan, Arg.Any<Guid>(), interactionId, Arg.Any<string>());

            // Assert
            Assert.AreEqual(interactionId, actionPlan.InteractionId);
        }

        [Test]
        public void ActionPlanTests_CheckLastModifiedTouchpointIdIsSet_WhenSetIdsIsCalled()
        {
            var actionPlan = new Models.ActionPlan();

            actionPlan.SetIds(actionPlan, Arg.Any<Guid>(), Arg.Any<Guid>(), "0000000000");

            // Assert
            Assert.AreEqual("0000000000", actionPlan.LastModifiedTouchpointId);
        }

        [Test]
        public void ActionPlanTests_CheckDateActionPlanCreatedIsUpdated_WhenPatchIsCalled()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow };
            var actionPlanPatch = new Models.ActionPlanPatch() { DateActionPlanCreated = DateTime.MaxValue };

            actionPlan.Patch(actionPlanPatch);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, actionPlan.DateActionPlanCreated);
        }

        [Test]
        public void ActionPlanTests_CheckCustomerCharterShownToCustomerIsUpdated_WhenPatchIsCalled()
        {
            var actionPlan = new Models.ActionPlan { CustomerCharterShownToCustomer = false };
            var actionPlanPatch = new Models.ActionPlanPatch { CustomerCharterShownToCustomer = true };

            actionPlan.Patch(actionPlanPatch);

            // Assert
            Assert.AreEqual(true, actionPlan.CustomerCharterShownToCustomer);
        }

        [Test]
        public void ActionPlanTests_CheckDateAndTimeCharterShownIsUpdated_WhenPatchIsCalled()
        {
            var actionPlan = new Models.ActionPlan { DateAndTimeCharterShown = DateTime.UtcNow };
            var actionPlanPatch = new Models.ActionPlanPatch { DateAndTimeCharterShown = DateTime.MaxValue };

            actionPlan.Patch(actionPlanPatch);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, actionPlan.DateAndTimeCharterShown);
        }

        [Test]
        public void ActionPlanTests_CheckDateActionPlanSentToCustomerIsUpdated_WhenPatchIsCalled()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanSentToCustomer = DateTime.UtcNow };
            var actionPlanPatch = new Models.ActionPlanPatch { DateActionPlanSentToCustomer = DateTime.MaxValue };

            actionPlan.Patch(actionPlanPatch);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, actionPlan.DateActionPlanSentToCustomer);
        }

        [Test]
        public void ActionPlanTests_CheckActionPlanDeliveryMethodIsUpdated_WhenPatchIsCalled()
        {
            var actionPlan = new Models.ActionPlan { ActionPlanDeliveryMethod = ActionPlanDeliveryMethod.Other };
            var actionPlanPatch = new Models.ActionPlanPatch { ActionPlanDeliveryMethod = ActionPlanDeliveryMethod.Paper };

            actionPlan.Patch(actionPlanPatch);

            // Assert
            Assert.AreEqual(ActionPlanDeliveryMethod.Paper, actionPlan.ActionPlanDeliveryMethod);
        }

        [Test]
        public void ActionPlanTests_CheckDateActionPlanAcknowledgedIsUpdated_WhenPatchIsCalled()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanAcknowledged = DateTime.UtcNow };
            var actionPlanPatch = new Models.ActionPlanPatch { DateActionPlanAcknowledged = DateTime.MaxValue };

            actionPlan.Patch(actionPlanPatch);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, actionPlan.DateActionPlanAcknowledged);
        }

        [Test]
        public void ActionPlanTests_CheckPriorityCustomerIsUpdated_WhenPatchIsCalled()
        {
            var actionPlan = new Models.ActionPlan { PriorityCustomer = PriorityCustomer.LowSkilledAdultsWithoutALevel2Qualification };
            var actionPlanPatch = new Models.ActionPlanPatch { PriorityCustomer = PriorityCustomer.NotAPriorityCustomer };

            actionPlan.Patch(actionPlanPatch);

            // Assert
            Assert.AreEqual(PriorityCustomer.NotAPriorityCustomer, actionPlan.PriorityCustomer);
        }

        [Test]
        public void ActionPlanTests_CheckCurrentSituationIsUpdated_WhenPatchIsCalled()
        {
            var actionPlan = new Models.ActionPlan { CurrentSituation = "Old" };
            var actionPlanPatch = new Models.ActionPlanPatch { CurrentSituation = "Current" };

            actionPlan.Patch(actionPlanPatch);

            // Assert
            Assert.AreEqual("Current", actionPlan.CurrentSituation);
        }

        [Test]
        public void ActionPlanTests_CheckLastModifiedDateIsUpdated_WhenPatchIsCalled()
        {
            var actionPlan = new Models.ActionPlan { LastModifiedDate = DateTime.UtcNow };
            var actionPlanPatch = new Models.ActionPlanPatch { LastModifiedDate = DateTime.MaxValue };

            actionPlan.Patch(actionPlanPatch);

            // Assert
            Assert.AreEqual(DateTime.MaxValue, actionPlan.LastModifiedDate);
        }

        [Test]
        public void ActionPlanTests_CheckLastModifiedTouchpointIdIsUpdated_WhenPatchIsCalled()
        {
            var actionPlan = new Models.ActionPlan { LastModifiedTouchpointId = "0000000000" };
            var actionPlanPatch = new Models.ActionPlanPatch { LastModifiedTouchpointId = "0000000111" };

            actionPlan.Patch(actionPlanPatch);

            // Assert
            Assert.AreEqual("0000000111", actionPlan.LastModifiedTouchpointId);
        }

    }
}