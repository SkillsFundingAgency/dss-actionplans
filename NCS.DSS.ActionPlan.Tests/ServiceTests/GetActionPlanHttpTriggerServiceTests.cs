using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NCS.DSS.ActionPlan.Cosmos.Provider;
using NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Service;
using NSubstitute;
using NUnit.Framework;

namespace NCS.DSS.ActionPlan.Tests.ServiceTests
{
    [TestFixture]
    public class GetActionPlanHttpTriggerServiceTests
    {
        private IGetActionPlanHttpTriggerService _actionPlanHttpTriggerService;
        private IDocumentDBProvider _documentDbProvider;
        private readonly Guid _customerId = Guid.Parse("58b43e3f-4a50-4900-9c82-a14682ee90fa");
        
        [SetUp]
        public void Setup()
        {
            _documentDbProvider = Substitute.For<IDocumentDBProvider>();
            _actionPlanHttpTriggerService = Substitute.For<GetActionPlanHttpTriggerService>(_documentDbProvider);
        }

        [Test]
        public async Task GetActionPlanHttpTriggerServiceTests_GetActionPlansAsync_ReturnsNullWhenResourceCannotBeFound()
        {
            _documentDbProvider.GetActionPlansForCustomerAsync(Arg.Any<Guid>()).Returns(Task.FromResult<List<Models.ActionPlan>>(null).Result);

            // Act
            var result = await _actionPlanHttpTriggerService.GetActionPlansAsync(_customerId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetActionPlanHttpTriggerServiceTests_GetActionPlansAsync_ReturnsResource()
        {
            _documentDbProvider.GetActionPlansForCustomerAsync(Arg.Any<Guid>()).Returns(Task.FromResult(new List<Models.ActionPlan>()).Result);

            // Act
            var result = await _actionPlanHttpTriggerService.GetActionPlansAsync(_customerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<List<Models.ActionPlan>>(result);
        }
    }
}