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

            actionPlan.SetIds(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>());

            // Assert
            Assert.AreNotSame(Guid.Empty, actionPlan.ActionPlanId);
        }

        [Test]
        public void ActionPlanTests_CheckCustomerIdIsSet_WhenSetIdsIsCalled()
        {
            var actionPlan = new Models.ActionPlan();

            var customerId = Guid.NewGuid();
            actionPlan.SetIds(customerId, Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>());

            // Assert
            Assert.AreEqual(customerId, actionPlan.CustomerId);
        }

        [Test]
        public void ActionPlanTests_CheckInteractionIdIsSet_WhenSetIdsIsCalled()
        {
            var actionPlan = new Models.ActionPlan();

            var interactionId = Guid.NewGuid();
            actionPlan.SetIds(Arg.Any<Guid>(), interactionId, Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string>());

            // Assert
            Assert.AreEqual(interactionId, actionPlan.InteractionId);
        }

        [Test]
        public void ActionPlanTests_CheckSessionIdIsSet_WhenSetIdsIsCalled()
        {
            var actionPlan = new Models.ActionPlan();

            var sessionId = Guid.NewGuid();
            actionPlan.SetIds(Arg.Any<Guid>(), Arg.Any<Guid>(), sessionId,  Arg.Any<string>(), Arg.Any<string>());

            // Assert
            Assert.AreEqual(sessionId, actionPlan.SessionId);
        }

        [Test]
        public void ActionPlanTests_CheckTouchpointIdIsSet_WhenSetIdsIsCalled()
        {
            var actionPlan = new Models.ActionPlan();

            actionPlan.SetIds(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<Guid>(), "0000000000", Arg.Any<string>());

            // Assert
            Assert.AreEqual("0000000000", actionPlan.LastModifiedTouchpointId);
        }

        [Test]
        public void ActionPlanTests_CheckSubcontractorIdIsSet_WhenSetIdsIsCalled()
        {
            var actionPlan = new Models.ActionPlan();

            actionPlan.SetIds(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<string>(), "0000000000");

            // Assert
            Assert.AreEqual("0000000000", actionPlan.SubcontractorId);
        }
        
    }
}