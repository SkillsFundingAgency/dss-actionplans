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
            List<PriorityCustomer> pList = new List<PriorityCustomer> { PriorityCustomer.NotAPriorityCustomer };
            var actionPlan = new Models.ActionPlan
            {
                PriorityCustomer = pList,
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
            List<PriorityCustomer> pList = new List<PriorityCustomer> { PriorityCustomer.NotAPriorityCustomer };
            var actionPlan = new Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = pList,
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
            List<PriorityCustomer> pList = new List<PriorityCustomer> { PriorityCustomer.NotAPriorityCustomer };
            var actionPlan = new Models.ActionPlan {
                DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = pList,
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
            List<PriorityCustomer> pList = new List<PriorityCustomer> { PriorityCustomer.NotAPriorityCustomer };
            var actionPlan = new Models.ActionPlan
            {
                PriorityCustomer = pList,
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
            List<PriorityCustomer> pList = new List<PriorityCustomer> { PriorityCustomer.NotAPriorityCustomer };
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = pList,
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
        public void ValidateTests_ReturnValidationResult_WhenDateAndTimeCharterShownIsInTheFuture()
        {
            List<PriorityCustomer> pList = new List<PriorityCustomer> { PriorityCustomer.NotAPriorityCustomer };
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = pList,
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
            List<PriorityCustomer> pList = new List<PriorityCustomer> { PriorityCustomer.NotAPriorityCustomer };
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = pList,
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
            List<PriorityCustomer> pList = new List<PriorityCustomer> { PriorityCustomer.NotAPriorityCustomer };
            var actionPlan = new Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.UtcNow.AddDays(2),
                PriorityCustomer = pList,
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
            List<PriorityCustomer> pList = new List<PriorityCustomer> { PriorityCustomer.NotAPriorityCustomer };
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = pList,
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
            List<PriorityCustomer> pList = new List<PriorityCustomer> { PriorityCustomer.NotAPriorityCustomer };
            var actionPlan = new Models.ActionPlan
            {
                DateActionPlanCreated = DateTime.MaxValue,
                PriorityCustomer = pList,
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
            List<PriorityCustomer> pList = new List<PriorityCustomer> { PriorityCustomer.NotAPriorityCustomer };
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = pList,
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
            List<PriorityCustomer> pList = new List<PriorityCustomer> { (PriorityCustomer)100 };
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow,
                CustomerCharterShownToCustomer = false,
                SessionId = Guid.Empty,
                PriorityCustomer = pList
            };

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
            List<PriorityCustomer> pList = new List<PriorityCustomer> { PriorityCustomer.NotAPriorityCustomer };
            var actionPlan = new Models.ActionPlan { DateActionPlanCreated = DateTime.UtcNow,
                PriorityCustomer = pList,
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
            List<PriorityCustomer> pList = new List<PriorityCustomer> { PriorityCustomer.NotAPriorityCustomer };
            var actionPlan = new Models.ActionPlan
            {
                PriorityCustomer = pList,
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