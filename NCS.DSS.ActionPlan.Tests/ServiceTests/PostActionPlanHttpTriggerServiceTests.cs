using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Moq;
using NCS.DSS.ActionPlan.Cosmos.Provider;
using NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.ServiceBus;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.Tests.ServiceTests
{
    [TestFixture]
    public class PostActionPlanHttpTriggerServiceTests
    {
        private IPostActionPlanHttpTriggerService _actionPlanHttpTriggerService;
        private Mock<ICosmosDbProvider> _mockCosmosDbProvider;
        private Mock<IActionPlanServiceBusClient> _mockActionPlanServiceBusClient;            
        private Mock<ILogger<PostActionPlanHttpTriggerService>> _mockLogger;
        private string _json;
        private Models.ActionPlan _actionPlan;
        private readonly Guid _actionPlanId = Guid.Parse("7E467BDB-213F-407A-B86A-1954053D3C24");

        [SetUp]
        public void Setup()
        {
            _mockCosmosDbProvider = new Mock<ICosmosDbProvider>();
            _mockActionPlanServiceBusClient = new Mock<IActionPlanServiceBusClient>();
            _mockLogger = new Mock<ILogger<PostActionPlanHttpTriggerService>>();
            _actionPlanHttpTriggerService = new PostActionPlanHttpTriggerService(_mockCosmosDbProvider.Object, _mockActionPlanServiceBusClient.Object, _mockLogger.Object);
            _actionPlan = new Models.ActionPlan();
            _json = JsonConvert.SerializeObject(_actionPlan);
        }

        [Test]
        public async Task PostActionPlanHttpTriggerServiceTests_CreateAsync_ReturnsNullWhenActionPlanJsonIsNull()
        {
            // Act
            var result = await _actionPlanHttpTriggerService.CreateAsync(It.IsAny<Models.ActionPlan>());

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task PostActionPlanHttpTriggerServiceTests_CreateAsync_ReturnsResource()
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
            .Returns(HttpStatusCode.Created);

            var resourceResponse = mockItemResponse.Object;

            _mockCosmosDbProvider.Setup(x => x.CreateActionPlanAsync(It.IsAny<Models.ActionPlan>())).Returns(Task.FromResult(resourceResponse));

            // Act
            var result = await _actionPlanHttpTriggerService.CreateAsync(_actionPlan);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(typeof(Models.ActionPlan) == result.GetType());
        }
    }
}