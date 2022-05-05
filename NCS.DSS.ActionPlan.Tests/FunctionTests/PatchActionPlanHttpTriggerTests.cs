using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.Models;
using NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Validation;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.Tests.FunctionTests
{
    [TestFixture]
    public class PatchActionPlanHttpTriggerTests
    {
        private const string ValidCustomerId = "7E467BDB-213F-407A-B86A-1954053D3C24";
        private const string ValidInteractionId = "1e1a555c-9633-4e12-ab28-09ed60d51cb3";
        private const string ValidActionPlanId = "d5369b9a-6959-4bd3-92fc-1583e72b7e51";
        private const string ValidSessionId = "58b43e3f-4a50-4900-9c82-a14682ee90fa";
        private const string ValidDssCorrelationId = "452d8e8c-2516-4a6b-9fc1-c85e578ac066";
        private const string InValidId = "1111111-2222-3333-4444-555555555555";

        private Mock<ILogger> _log;
        private HttpRequest _request;
        private Mock<IResourceHelper> _resourceHelper;
        private IValidate _validate;
        private Mock<IPatchActionPlanHttpTriggerService> _patchActionPlanHttpTriggerService;
        private Mock<ILoggerHelper> _loggerHelper;
        private Mock<IHttpRequestHelper> _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IJsonHelper _jsonHelper;
        private Models.ActionPlan _actionPlan;
        private ActionPlanPatch _actionPlanPatch;
        private string _actionPlanString = string.Empty;
        private PatchActionPlanHttpTrigger.Function.PatchActionPlanHttpTrigger _function;

        [SetUp]
        public void Setup()
        {
            _actionPlan = new Models.ActionPlan();
            _actionPlanPatch = new ActionPlanPatch();
            _request = new DefaultHttpRequest(new DefaultHttpContext());
            _resourceHelper = new Mock<IResourceHelper>();
            _loggerHelper = new Mock<ILoggerHelper>();
            _httpRequestHelper = new Mock<IHttpRequestHelper>();
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _jsonHelper = new JsonHelper();
            _log = new Mock<ILogger>();
            _resourceHelper = new Mock<IResourceHelper>();
            _validate = new Validate();
            _patchActionPlanHttpTriggerService = new Mock<IPatchActionPlanHttpTriggerService>();
            _actionPlanString = JsonConvert.SerializeObject(_actionPlan);
            _function = new PatchActionPlanHttpTrigger.Function.PatchActionPlanHttpTrigger(
                _resourceHelper.Object, 
                _validate, 
                _patchActionPlanHttpTriggerService.Object, 
                _loggerHelper.Object, 
                _httpRequestHelper.Object, 
                _httpResponseMessageHelper, 
                _jsonHelper);
        }

        [Test]
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            // arrange
            _httpRequestHelper.Setup(x=>x.GetDssTouchpointId(_request)).Returns((string)null);
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");

            // Act
            var result = await RunFunction(InValidId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenSubcontractorIdIsNotProvided()
        {
            // arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns((string)null);

            // Act
            var result = await RunFunction(InValidId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenApiurlIsNotProvided()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x=>x.GetDssApimUrl(_request)).Returns((string)null);

            // Act
            var result = await RunFunction(InValidId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://someurl.com");

            // Act
            var result = await RunFunction(InValidId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenInteractionIdIsInvalid()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://someurl.com");

            // Act
            var result = await RunFunction(ValidCustomerId, InValidId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenActionPlanIdIsInvalid()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://someurl.com");

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId, InValidId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenActionPlanRequestIsInvalid()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://someurl.com");
            _httpRequestHelper.Setup(x=>x.GetResourceFromRequest<ActionPlanPatch>(_request)).Throws(new JsonException());

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual((HttpStatusCode)422, result.StatusCode);
        }

        [Test]
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://someurl.com");
            _resourceHelper.Setup(x=>x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(false));
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(_actionPlanPatch));

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenInteractionDoesNotExist()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://someurl.com");
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(_actionPlanPatch));
            _resourceHelper.Setup(x=>x.DoesInteractionExistAndBelongToCustomer(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(false);

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenSessionDoesNotExist()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://someurl.com");
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(_actionPlanPatch));
            _patchActionPlanHttpTriggerService.Setup(x=>x.GetActionPlanForCustomerAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.FromResult("actionplan"));
            _patchActionPlanHttpTriggerService.Setup(x=>x.PatchResource(It.IsAny<string>(), It.IsAny<Models.ActionPlanPatch>())).Returns((string)null);
            _resourceHelper.Setup(x=>x.DoesSessionExistAndBelongToCustomer(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(false);

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenActionPlanDoesNotExist()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://someurl.com");
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(_actionPlanPatch));
            _patchActionPlanHttpTriggerService.Setup(x => x.GetActionPlanForCustomerAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.FromResult((string)null));

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenActionPlaCantBePatched()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://someurl.com");
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _resourceHelper.Setup(x => x.DoesInteractionExistAndBelongToCustomer(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(_actionPlanPatch));
            _patchActionPlanHttpTriggerService.Setup(x => x.GetActionPlanForCustomerAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.FromResult("actionplan"));
            _patchActionPlanHttpTriggerService.Setup(x=>x.PatchResource(It.IsAny<string>(), It.IsAny<Models.ActionPlanPatch>())).Returns((string)null);

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenActionPlanHasFailedValidation()
        {
            // Arrange
            var actionPlanRequest = new Models.ActionPlanPatch() { DateActionPlanCreated = DateTime.Now.AddDays(-1), SessionId = new Guid(ValidSessionId) };
            var actionPlan = new Models.ActionPlan() { DateActionPlanCreated = DateTime.Now.AddDays(-1), SessionId = new Guid(ValidSessionId) };
            var actionPlanStr = JsonConvert.SerializeObject(actionPlan);
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://someurl.com");
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _resourceHelper.Setup(x => x.DoesInteractionExistAndBelongToCustomer(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(actionPlanRequest));
            _patchActionPlanHttpTriggerService.Setup(x => x.GetActionPlanForCustomerAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.FromResult(actionPlanStr));
            _patchActionPlanHttpTriggerService.Setup(x => x.UpdateCosmosAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<Models.ActionPlan>(null));
            _patchActionPlanHttpTriggerService.Setup(x => x.PatchResource(It.IsAny<string>(), It.IsAny<Models.ActionPlanPatch>())).Returns(actionPlanStr);
            _resourceHelper.Setup(x => x.GetDateAndTimeOfSession(It.IsAny<Guid>())).Returns(Task.FromResult<DateTime?>(DateTime.Now.AddDays(-2)));

            // Action
            var result = await RunFunction(ValidCustomerId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual((HttpStatusCode)422, result.StatusCode);
        }

        [Test]
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenUnableToUpdateActionPlanRecord()
        {
            // Arrange
            var actionPlanRequest = new Models.ActionPlanPatch() { DateActionPlanCreated = DateTime.Now.AddDays(-1), SessionId = new Guid(ValidSessionId) };
            var actionPlan = new Models.ActionPlan() { DateActionPlanCreated = DateTime.Now.AddDays(-1), SessionId = new Guid(ValidSessionId), CustomerCharterShownToCustomer=true };
            var actionPlanStr = JsonConvert.SerializeObject(actionPlan);
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://someurl.com");
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _resourceHelper.Setup(x => x.DoesInteractionExistAndBelongToCustomer(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(actionPlanRequest));
            _patchActionPlanHttpTriggerService.Setup(x => x.GetActionPlanForCustomerAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.FromResult(actionPlanStr));
            _patchActionPlanHttpTriggerService.Setup(x => x.UpdateCosmosAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<Models.ActionPlan>(null));
            _patchActionPlanHttpTriggerService.Setup(x => x.PatchResource(It.IsAny<string>(), It.IsAny<Models.ActionPlanPatch>())).Returns(actionPlanStr);
            _resourceHelper.Setup(x => x.GetDateAndTimeOfSession(It.IsAny<Guid>())).Returns(Task.FromResult<DateTime?>(DateTime.Now.AddDays(-2)));

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }


        [Test]
        public async Task PatchActionPlanHttpTrigger_ReturnsStatusCodeOK_WhenRequestIsValid()
        {
            // Arrange
            var actionPlanRequest = new Models.ActionPlanPatch() { DateActionPlanCreated = DateTime.Now.AddDays(-1), SessionId = new Guid(ValidSessionId) };
            var actionPlan = new Models.ActionPlan() { DateActionPlanCreated = DateTime.Now.AddDays(-1), SessionId = new Guid(ValidSessionId), CustomerCharterShownToCustomer=true };
            var actionPlanStr = JsonConvert.SerializeObject(actionPlan);
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssSubcontractorId(_request)).Returns("9999999999");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://someurl.com");
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _resourceHelper.Setup(x => x.DoesInteractionExistAndBelongToCustomer(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<ActionPlanPatch>(_request)).Returns(Task.FromResult(actionPlanRequest));
            _patchActionPlanHttpTriggerService.Setup(x => x.GetActionPlanForCustomerAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(Task.FromResult(actionPlanStr));
            _patchActionPlanHttpTriggerService.Setup(x => x.UpdateCosmosAsync(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult<Models.ActionPlan>(actionPlan));
            _patchActionPlanHttpTriggerService.Setup(x => x.PatchResource(It.IsAny<string>(), It.IsAny<Models.ActionPlanPatch>())).Returns(actionPlanStr);
            _resourceHelper.Setup(x => x.GetDateAndTimeOfSession(It.IsAny<Guid>())).Returns(Task.FromResult<DateTime?>(DateTime.Now.AddDays(-2)));

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId, ValidActionPlanId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
        }

        private async Task<HttpResponseMessage> RunFunction(string customerId, string interactionId, string actionPlanId)
        {
            return await _function.Run(
                _request,
                _log.Object,
                customerId,
                interactionId,
                actionPlanId).ConfigureAwait(false);
        }
    }
}