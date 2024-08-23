using Moq;
using NCS.DSS.ActionPlan.Cosmos.Provider;
using NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Service;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.Tests.ServiceTests
{
    [TestFixture]
    public class GetActionPlanByIdHttpTriggerServiceTests
    {
        private IGetActionPlanByIdHttpTriggerService _actionPlanHttpTriggerService;
        private Mock<IDocumentDBProvider> _documentDbProvider;
        private Models.ActionPlan _actionPlan;
        private readonly Guid _actionPlanId = Guid.Parse("7E467BDB-213F-407A-B86A-1954053D3C24");
        private readonly Guid _customerId = Guid.Parse("58b43e3f-4a50-4900-9c82-a14682ee90fa");


        [SetUp]
        public void Setup()
        {
            _documentDbProvider = new Mock<IDocumentDBProvider>();
            _actionPlanHttpTriggerService = new GetActionPlanByIdHttpTriggerService(_documentDbProvider.Object);
            _actionPlan = new Models.ActionPlan();
        }

        [Test]
        public async Task GetActionPlanHttpTriggerServiceTests_GetActionPlanForCustomerAsync_ReturnsNullWhenResourceCannotBeFound()
        {
            // Arrange
            _documentDbProvider.Setup(x => x.GetActionPlanForCustomerAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.FromResult<Models.ActionPlan>(null));

            // Act
            var result = await _actionPlanHttpTriggerService.GetActionPlanForCustomerAsync(_customerId, _actionPlanId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetActionPlanHttpTriggerServiceTests_GetActionPlanForCustomerAsync_ReturnsResource()
        {
            // Arrange
            _documentDbProvider.Setup(x => x.GetActionPlanForCustomerAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.FromResult(_actionPlan));

            // Act
            var result = await _actionPlanHttpTriggerService.GetActionPlanForCustomerAsync(_customerId, _actionPlanId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(typeof(Models.ActionPlan) == result.GetType());
        }
    }
}