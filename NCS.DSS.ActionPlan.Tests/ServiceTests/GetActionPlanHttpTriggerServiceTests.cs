using Moq;
using NCS.DSS.ActionPlan.Cosmos.Provider;
using NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Service;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.Tests.ServiceTests
{
    [TestFixture]
    public class GetActionPlanHttpTriggerServiceTests
    {
        private IGetActionPlanHttpTriggerService _actionPlanHttpTriggerService;
        private Mock<IDocumentDBProvider> _documentDbProvider;
        private readonly Guid _customerId = Guid.Parse("58b43e3f-4a50-4900-9c82-a14682ee90fa");

        [SetUp]
        public void Setup()
        {
            _documentDbProvider = new Mock<IDocumentDBProvider>();
            _actionPlanHttpTriggerService = new GetActionPlanHttpTriggerService(_documentDbProvider.Object);
        }

        [Test]
        public async Task GetActionPlanHttpTriggerServiceTests_GetActionPlansAsync_ReturnsNullWhenResourceCannotBeFound()
        {
            // Arrange
            _documentDbProvider.Setup(x => x.GetActionPlansForCustomerAsync(It.IsAny<Guid>())).Returns(Task.FromResult<List<Models.ActionPlan>>(null));

            // Act
            var result = await _actionPlanHttpTriggerService.GetActionPlansAsync(_customerId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetActionPlanHttpTriggerServiceTests_GetActionPlansAsync_ReturnsResource()
        {
            // Arrange
            _documentDbProvider.Setup(x => x.GetActionPlansForCustomerAsync(It.IsAny<Guid>())).Returns(Task.FromResult(new List<Models.ActionPlan>()));

            // Act
            var result = await _actionPlanHttpTriggerService.GetActionPlansAsync(_customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<Models.ActionPlan>>(result);
        }
    }
}