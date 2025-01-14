using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Moq;
using NCS.DSS.ActionPlan.Cosmos.Provider;
using NCS.DSS.ActionPlan.Models;
using NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.ServiceBus;
using Newtonsoft.Json;

using NUnit.Framework;

namespace NCS.DSS.ActionPlan.Tests.ServiceTests
{
    [TestFixture]
    public class PatchActionPlanHttpTriggerServiceTests
    {
        private IPatchActionPlanHttpTriggerService _actionPlanHttpTriggerService;        
        private Mock<IActionPlanPatchService> _mockActionPlanPatchService;
        private Mock<ICosmosDbProvider> _mockCosmosDbProvider;
        private Mock<IActionPlanServiceBusClient> _mockActionPlanServiceBusClient;
        private Mock<ILogger<PatchActionPlanHttpTriggerService>> _mockLogger;
        private string _json;
        private Models.ActionPlan _actionPlan;
        private readonly Guid _actionPlanId = Guid.Parse("7E467BDB-213F-407A-B86A-1954053D3C24");        
        
        [SetUp]
        public void Setup()
        {
            _mockActionPlanPatchService = new Mock<IActionPlanPatchService>();
            _mockCosmosDbProvider = new Mock<ICosmosDbProvider>();
            _mockActionPlanServiceBusClient = new Mock<IActionPlanServiceBusClient>();
            _mockLogger = new Mock<ILogger<PatchActionPlanHttpTriggerService>>();

            _actionPlanHttpTriggerService = new PatchActionPlanHttpTriggerService(
                _mockActionPlanPatchService.Object,
                _mockCosmosDbProvider.Object, 
                _mockActionPlanServiceBusClient.Object, 
                _mockLogger.Object);

            _actionPlan = new Models.ActionPlan();
            _json = JsonConvert.SerializeObject(_actionPlan);
        }

        [Test]
        public void PatchActionPlanHttpTriggerServiceTests_PatchResource_ReturnsNullWhenActionPlanJsonIsNullOrEmpty()
        {
            // Act
            var result = _actionPlanHttpTriggerService.PatchResource(null, new ActionPlanPatch());

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void PatchActionPlanHttpTriggerServiceTests_PatchResource_ReturnsNullWhenActionPlanPatchIsNullOrEmpty()
        {
            // Act
            var result = _actionPlanHttpTriggerService.PatchResource("{'test':'test-data'}", null);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void PatchActionPlanHttpTriggerServiceTests_PatchResource_ReturnsUpdateActionPlanWhenInputDataIsValid()
        {
            // Arrange             
            _mockActionPlanPatchService.Setup(m => m.Patch(It.IsAny<string>(), It.IsAny<ActionPlanPatch>()))
                .Returns("{'test':'test-data-updated'}");

            // Act
            var result = _actionPlanHttpTriggerService.PatchResource("{'test':'test-data'}", new ActionPlanPatch());

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task PatchActionPlanHttpTriggerServiceTests_UpdateAsync_ReturnsNullWhenActionPlanIsNullOrEmpty()
        {
            // Act
            var result = await _actionPlanHttpTriggerService.UpdateCosmosAsync(null, _actionPlanId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task PatchActionPlanHttpTriggerServiceTests_UpdateAsync_ReturnsNullWhenActionPlanPatchServicePatchJsonIsNullOrEmpty()
        {
            // Arrange
            string json = null;

            _mockActionPlanPatchService.Setup(m => m.Patch(It.IsAny<string>(), It.IsAny<ActionPlanPatch>()))
                .Returns(json);

            // Act
            var result = await _actionPlanHttpTriggerService.UpdateCosmosAsync(_actionPlan.ToString(), _actionPlanId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task PatchActionPlanHttpTriggerServiceTests_UpdateAsync_ReturnsNullWhenResourceCannotBeUpdated()
        {
            ItemResponse<Models.ActionPlan> response = null;

            _mockCosmosDbProvider.Setup(m => m.UpdateActionPlanAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(response);

            // Act
            var result = await _actionPlanHttpTriggerService.UpdateCosmosAsync(_actionPlan.ToString(), _actionPlanId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task PatchActionPlanHttpTriggerServiceTests_UpdateAsync_ReturnsResourceWhenUpdated()
        {
            // Arrange            
            var mockActionPlan = new Models.ActionPlan
            {
                ActionPlanId = new Guid("9c0d182f-5d62-4b64-921e-ab80d6352c57"),
                CustomerId = new Guid("8840cb20-2436-431b-9e93-5899bb6ea966"),
                InteractionId = new Guid("22b49d9f-f6eb-4aff-919e-e1dc7f413db7"),
                SessionId = new Guid("cce61da8-b7a8-4843-b308-39c8c380210e"),
                SubcontractorId = "12345678",
                LastModifiedTouchpointId = "9999999999"
            };

            var mockItemResponse = new Mock<ItemResponse<Models.ActionPlan>>();

            mockItemResponse
            .Setup(response => response.Resource)
            .Returns(mockActionPlan);
            mockItemResponse
            .Setup(response => response.StatusCode)
            .Returns(HttpStatusCode.OK);

            var resourceResponse = mockItemResponse.Object;

            _mockCosmosDbProvider.Setup(x => x.UpdateActionPlanAsync(It.IsAny<string>(), It.IsAny<Guid>()))
                .ReturnsAsync(resourceResponse);
            // Act
            var result = await _actionPlanHttpTriggerService.UpdateCosmosAsync(_json, _actionPlanId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<Models.ActionPlan>());
        }
    }
}