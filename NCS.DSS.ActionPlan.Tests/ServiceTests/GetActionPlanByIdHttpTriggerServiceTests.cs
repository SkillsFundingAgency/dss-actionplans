using System;
using System.Threading.Tasks;
using NCS.DSS.ActionPlan.Cosmos.Provider;
using NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Service;
using NSubstitute;
using NUnit.Framework;

namespace NCS.DSS.ActionPlan.Tests.ServiceTests
{
    [TestFixture]
    public class GetActionPlanByIdHttpTriggerServiceTests
    {
        private IGetActionPlanByIdHttpTriggerService _actionPlanHttpTriggerService;
        private IDocumentDBProvider _documentDbProvider;
        private Models.ActionPlan _actionPlan;
        private readonly Guid _actionPlanId = Guid.Parse("7E467BDB-213F-407A-B86A-1954053D3C24");
        private readonly Guid _customerId = Guid.Parse("58b43e3f-4a50-4900-9c82-a14682ee90fa");


        [SetUp]
        public void Setup()
        {
            _documentDbProvider = Substitute.For<IDocumentDBProvider>();
            _actionPlanHttpTriggerService = Substitute.For<GetActionPlanByIdHttpTriggerService>(_documentDbProvider);
            _actionPlan = Substitute.For<Models.ActionPlan>();
        }

        [Test]
        public async Task GetActionPlanHttpTriggerServiceTests_GetActionPlanForCustomerAsync_ReturnsNullWhenResourceCannotBeFound()
        {
            _documentDbProvider.GetActionPlanForCustomerAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(Task.FromResult<Models.ActionPlan>(null).Result);

            // Act
            var result = await _actionPlanHttpTriggerService.GetActionPlanForCustomerAsync(_customerId, _actionPlanId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetActionPlanHttpTriggerServiceTests_GetActionPlanForCustomerAsync_ReturnsResource()
        {

            _documentDbProvider.GetActionPlanForCustomerAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(Task.FromResult(_actionPlan).Result);

            // Act
            var result = await _actionPlanHttpTriggerService.GetActionPlanForCustomerAsync(_customerId, _actionPlanId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Models.ActionPlan>(result);
        }
    }
}