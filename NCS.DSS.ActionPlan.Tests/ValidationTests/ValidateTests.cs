using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NCS.DSS.ActionPlan.ReferenceData;
using NCS.DSS.ActionPlan.Validation;
using NSubstitute;
using NUnit.Framework;

namespace NCS.DSS.ActionPlan.Tests.ValidationTests
{
    [TestFixture]
    public class ValidateTests
    {

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanCreatedIsNotSupplied()
        {
            var actionPlan = new Models.ActionPlan
            {
                PriorityCustomer = PriorityCustomer.NotAPriorityCustomer,
                CustomerCharterShownToCustomer = false,
                SessionId = Guid.Empty
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, Arg.Any<DateTime>());

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenSessionIdIsNotSupplied()
        {
            var actionPlan = new Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = PriorityCustomer.NotAPriorityCustomer,
                CustomerCharterShownToCustomer = false,
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, Arg.Any<DateTime>());

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanCreatedIsNotGreaterThanDateAndTimeOfSession()
        {
            var actionPlan = new Models.ActionPlan {
                DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = PriorityCustomer.NotAPriorityCustomer,
                CustomerCharterShownToCustomer = false,
                SessionId = Guid.Empty
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, DateTime.MaxValue);

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanCreatedIsInTheFuture()
        {
            var actionPlan = new Models.ActionPlan
            {
                PriorityCustomer = PriorityCustomer.NotAPriorityCustomer,
                CustomerCharterShownToCustomer = false,
                DateActionPlanCreated = DateTime.MaxValue,
                SessionId = Guid.Empty
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, Arg.Any<DateTime>());

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenPriorityCustomerIsNotSupplied()
        {
            var actionPlan = new Models.ActionPlan {
                DateActionPlanCreated = DateTime.UtcNow,
                CustomerCharterShownToCustomer = false,
                SessionId = Guid.Empty
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, Arg.Any<DateTime>());

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenCustomerCharterShownToCustomerIsNotSupplied()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = PriorityCustomer.NotAPriorityCustomer,
                SessionId = Guid.Empty
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, Arg.Any<DateTime>());

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateAndTimeCharterShownIsNotGreaterThanDateActionPlanCreated()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow.AddDays(2),
                PriorityCustomer = PriorityCustomer.NotAPriorityCustomer,
                SessionId = Guid.Empty,
                CustomerCharterShownToCustomer = true,
                DateAndTimeCharterShown = DateTime.UtcNow
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, Arg.Any<DateTime>());

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateAndTimeCharterShownIsInTheFuture()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = PriorityCustomer.NotAPriorityCustomer,
                SessionId = Guid.Empty,
                CustomerCharterShownToCustomer = false,
                DateAndTimeCharterShown = DateTime.MaxValue };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, Arg.Any<DateTime>());

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanSentToCustomerIsInTheFuture()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = PriorityCustomer.NotAPriorityCustomer,
                CustomerCharterShownToCustomer = false,
                SessionId = Guid.Empty,
                DateActionPlanSentToCustomer = DateTime.MaxValue };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, Arg.Any<DateTime>());

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanSentToCustomerIsNotGreaterThanDateActionPlanCreated()
        {
            var actionPlan = new Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.UtcNow.AddDays(2),
                PriorityCustomer = PriorityCustomer.NotAPriorityCustomer,
                CustomerCharterShownToCustomer = true,
                SessionId = Guid.Empty,
                DateActionPlanSentToCustomer = DateTime.UtcNow
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, Arg.Any<DateTime>());

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanAcknowledgedIsInTheFuture()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = PriorityCustomer.NotAPriorityCustomer,
                SessionId = Guid.Empty,
                CustomerCharterShownToCustomer = false, DateActionPlanAcknowledged = DateTime.MaxValue };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, Arg.Any<DateTime>());

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanAcknowledgedIsNotGreaterThanDateActionPlanCreated()
        {
            var actionPlan = new Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.MaxValue,
                PriorityCustomer = PriorityCustomer.NotAPriorityCustomer,
                SessionId = Guid.Empty,
                DateActionPlanAcknowledged = DateTime.UtcNow
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, Arg.Any<DateTime>());

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenLastModifiedDateIsInTheFuture()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = PriorityCustomer.NotAPriorityCustomer,
                CustomerCharterShownToCustomer = false,
                SessionId = Guid.Empty,
                LastModifiedDate = DateTime.MaxValue };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, Arg.Any<DateTime>());

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenPriorityCustomerIsNotValid()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow,
                CustomerCharterShownToCustomer = false,
                SessionId = Guid.Empty,
                PriorityCustomer = (PriorityCustomer)100 };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, Arg.Any<DateTime>());

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenActionPlanDeliveryMethodIsNotValid()
        {
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = PriorityCustomer.NotAPriorityCustomer,
                CustomerCharterShownToCustomer = false,
                SessionId = Guid.Empty,
                ActionPlanDeliveryMethod = (ActionPlanDeliveryMethod)100 };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, Arg.Any<DateTime>());

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ValidateTests_ReturnValidationResult_WhenDateActionPlanCreatedIsLessThanDateTimeSessionCreated()
        {
            var actionPlan = new Models.ActionPlan
            {
                PriorityCustomer = PriorityCustomer.NotAPriorityCustomer,
                CustomerCharterShownToCustomer = false,
                DateActionPlanCreated = DateTime.UtcNow,
                SessionId = Guid.Empty
            };

            var validation = new Validate();

            var result = validation.ValidateResource(actionPlan, DateTime.UtcNow.AddDays(2));

            // Assert
            Assert.IsInstanceOf<List<ValidationResult>>(result);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
        }

    }
}