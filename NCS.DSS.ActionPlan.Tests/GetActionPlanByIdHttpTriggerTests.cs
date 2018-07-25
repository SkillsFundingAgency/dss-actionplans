using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Service;
using NCS.DSS.ActionPlan.Helpers;
using NSubstitute;
using NUnit.Framework;

namespace NCS.DSS.ActionPlan.Tests
{
    [TestFixture]
    public class GetActionPlanByIdHttpTriggerTests
    {
        private const string ValidCustomerId = "7E467BDB-213F-407A-B86A-1954053D3C24";
        private const string ValidInteractionId = "1e1a555c-9633-4e12-ab28-09ed60d51cb3";
        private const string ValidActionPlanId = "d5369b9a-6959-4bd3-92fc-1583e72b7e51";
        private const string InValidId = "1111111-2222-3333-4444-555555555555";

        private ILogger _log;
        private HttpRequestMessage _request;
        private IResourceHelper _resourceHelper;
        private IGetActionPlanByIdHttpTriggerService _getActionPlanByIdHttpTriggerService;
        private IHttpRequestMessageHelper _httpRequestMessageHelper;
        private Models.ActionPlan _actionPlan;

        [SetUp]
        public void Setup()
        {
            _actionPlan = Substitute.For<Models.ActionPlan>();

            _request = new HttpRequestMessage()
            {
                Content = new StringContent(string.Empty),
                RequestUri = 
                    new Uri($"http://localhost:7071/api/Customers/7E467BDB-213F-407A-B86A-1954053D3C24/" +
                            $"Interactions/aa57e39e-4469-4c79-a9e9-9cb4ef410382/" +
                            $"ActionPlans/d5369b9a-6959-4bd3-92fc-1583e72b7e51")
            };

            _log = Substitute.For<ILogger>();
            _resourceHelper = Substitute.For<IResourceHelper>();
            _getActionPlanByIdHttpTriggerService = Substitute.For<IGetActionPlanByIdHttpTriggerService>();
            _httpRequestMessageHelper = Substitute.For<IHttpRequestMessageHelper>();
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            _httpRequestMessageHelper.GetTouchpointId(_request).Returns((Guid?) null);

            // Act
            var result = await RunFunction(InValidId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            _httpRequestMessageHelper.GetTouchpointId(_request).Returns(new Guid());

            // Act
            var result = await RunFunction(InValidId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeBadRequest_WhenInteractionIdIsInvalid()
        {
            _httpRequestMessageHelper.GetTouchpointId(_request).Returns(new Guid());

            // Act
            var result = await RunFunction(ValidCustomerId, InValidId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeBadRequest_WhenActionPlanIdIsInvalid()
        {
            _httpRequestMessageHelper.GetTouchpointId(_request).Returns(new Guid());

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId, InValidId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            _httpRequestMessageHelper.GetTouchpointId(_request).Returns(new Guid());
            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).ReturnsForAnyArgs(false);

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeNoContent_WhenInteractionDoesNotExist()
        {
            _httpRequestMessageHelper.GetTouchpointId(_request).Returns(new Guid());
            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).Returns(true);

            _resourceHelper.DoesInteractionExist(Arg.Any<Guid>()).Returns(false);

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeOk_WhenActionPlanDoesNotExist()
        {
            _httpRequestMessageHelper.GetTouchpointId(_request).Returns(new Guid());
            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).Returns(true);
            _resourceHelper.DoesInteractionExist(Arg.Any<Guid>()).Returns(true);

            _getActionPlanByIdHttpTriggerService.GetActionPlanForCustomerAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(Task.FromResult<Models.ActionPlan>(null).Result);

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task GetActionPlanByIdHttpTrigger_ReturnsStatusCodeOk_WhenActionPlanExists()
        {
            _httpRequestMessageHelper.GetTouchpointId(_request).Returns(new Guid());
            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).Returns(true);
            _resourceHelper.DoesInteractionExist(Arg.Any<Guid>()).Returns(true);

            _getActionPlanByIdHttpTriggerService.GetActionPlanForCustomerAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(Task.FromResult(_actionPlan).Result);

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        private async Task<HttpResponseMessage> RunFunction(string customerId, string interactionId, string actionPlanId)
        {
            return await GetActionPlanByIdHttpTrigger.Function.GetActionPlanByIdHttpTrigger.Run(
                _request, _log, customerId, interactionId, actionPlanId, _resourceHelper, _httpRequestMessageHelper, _getActionPlanByIdHttpTriggerService).ConfigureAwait(false);
        }

    }
}