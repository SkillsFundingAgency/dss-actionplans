using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NCS.DSS.ActionPlan.ReferenceData;
using NCS.DSS.ActionPlan.Validation;
using NUnit.Framework;

namespace NCS.DSS.ActionPlan.Tests.ValidationTests
{
    [TestFixture]
    public class ValidateTests
    {

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanCreatedIsNotSupplied()
        {
            var actionPlan = new Models.ActionPlan { PriorityCustomer = PriorityCustomer.NotAPriorityCustomer, CustomerCharterShownToCustomer = false };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenPriorityCustomerIsNotSupplied()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow, CustomerCharterShownToCustomer = false };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }


        [Test]
        public void ValidateTests_ReturnValidationResult_WhenCustomerCharterShownToCustomerIsNotSupplied()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow, PriorityCustomer = PriorityCustomer.NotAPriorityCustomer };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }


        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanCreatedIsInTheFuture()
        {
            var actionPlan = new Models.ActionPlan { PriorityCustomer = PriorityCustomer.NotAPriorityCustomer, CustomerCharterShownToCustomer = false, DateActionPlanCreated = DateTime.MaxValue };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateAndTimeCharterShownIsInTheFuture()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow, PriorityCustomer = PriorityCustomer.NotAPriorityCustomer, CustomerCharterShownToCustomer = false, DateAndTimeCharterShown = DateTime.MaxValue };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanSentToCustomerIsInTheFuture()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow, PriorityCustomer = PriorityCustomer.NotAPriorityCustomer, CustomerCharterShownToCustomer = false, DateActionPlanSentToCustomer = DateTime.MaxValue };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanAcknowledgedIsInTheFuture()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow, PriorityCustomer = PriorityCustomer.NotAPriorityCustomer, CustomerCharterShownToCustomer = false, DateActionPlanAcknowledged = DateTime.MaxValue };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenLastModifiedDateIsInTheFuture()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow, PriorityCustomer = PriorityCustomer.NotAPriorityCustomer, CustomerCharterShownToCustomer = false, LastModifiedDate = DateTime.MaxValue };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenPriorityCustomerIsNotValid()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow, CustomerCharterShownToCustomer = false, PriorityCustomer = (PriorityCustomer)100 };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenActionPlanDeliveryMethodIsNotValid()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow, PriorityCustomer = PriorityCustomer.NotAPriorityCustomer, CustomerCharterShownToCustomer = false, ActionPlanDeliveryMethod = (ActionPlanDeliveryMethod)100 };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

    }
}