using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Validation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.Tests.FunctionTests
{
    [TestFixture]
    public class PostActionPlanHttpTriggerTests
    {
        private const string ValidCustomerId = "f72c07d6-e3a6-4dc2-9e62-2e91f09e484e";//"7E467BDB-213F-407A-B86A-1954053D3C24";
        private const string ValidInteractionId = "00f9f801-5c58-495d-a7c4-491057919455"; //"1e1a555c-9633-4e12-ab28-09ed60d51cb3";
        private const string ValidSessionId = "1e17d2dd-f48c-4488-a3e8-1c4889763604";//"58b43e3f-4a50-4900-9c82-a14682ee90fa";
        private const string ValidDssCorrelationId = "452d8e8c-2516-4a6b-9fc1-c85e578ac066";
        private const string InValidId = "1111111-2222-3333-4444-555555555555";

        private Mock<ILogger> _log;
        private HttpRequest _request;
        private Mock<IResourceHelper> _resourceHelper;
        private IValidate _validate;
        private Mock<IPostActionPlanHttpTriggerService> _postActionPlanHttpTriggerService;
        private Mock<ILoggerHelper> _loggerHelper;
        private Mock<IHttpRequestHelper> _httpRequestHelper;
        private IJsonHelper _jsonHelper;
        private Models.ActionPlan _actionPlan;
        private PostActionPlanHttpTrigger.Function.PostActionPlanHttpTrigger _function;

        [SetUp]
        public void Setup()
        {
            _actionPlan =new Models.ActionPlan();
            _request = null;
            _resourceHelper = new Mock<IResourceHelper>();
            _loggerHelper = new Mock<ILoggerHelper>();
            _httpRequestHelper = new Mock<IHttpRequestHelper>();
            _jsonHelper = new JsonHelper();
            _log = new Mock<ILogger>(); 
            _validate = new Validate();
            _postActionPlanHttpTriggerService = new Mock<IPostActionPlanHttpTriggerService>();
            _function = new PostActionPlanHttpTrigger.Function.PostActionPlanHttpTrigger(
                _resourceHelper.Object, 
                _validate,
                _postActionPlanHttpTriggerService.Object, 
                _loggerHelper.Object, 
                _httpRequestHelper.Object, 
                _jsonHelper);
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenTouchpointIdIsNotProvided()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns((string)null);

            // Act
            var result = await RunFunction(InValidId, ValidInteractionId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenApiurlIsNotProvided()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");

            // Act
            var result = await RunFunction(InValidId, ValidInteractionId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:");

            // Act
            var result = await RunFunction(InValidId, ValidInteractionId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenInteractionIdIsInvalid()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:");

            // Act
            var result = await RunFunction(ValidCustomerId, InValidId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenActionPlanHasFailedValidation()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:");
            var validateMock = new Mock<IValidate>();
            var validationResults = new List<ValidationResult> { new ValidationResult("interaction Id is Required") };
            validateMock.Setup(x => x.ValidateResource(It.IsAny<Models.ActionPlan>(), It.IsAny<DateTime>())).Returns(validationResults);
            _function = new PostActionPlanHttpTrigger.Function.PostActionPlanHttpTrigger(
                _resourceHelper.Object,
                validateMock.Object,
                _postActionPlanHttpTriggerService.Object,
                _loggerHelper.Object,
                _httpRequestHelper.Object,
                _jsonHelper);

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId);

            // Assert
            Assert.That(result, Is.InstanceOf<UnprocessableEntityObjectResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenActionPlanRequestIsInvalid()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<Models.ActionPlan>(_request)).Returns(Task.FromResult<Models.ActionPlan>(null));

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId);

            // Assert
            Assert.That(result, Is.InstanceOf<UnprocessableEntityObjectResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<Models.ActionPlan>(_request)).Returns(Task.FromResult<Models.ActionPlan>(_actionPlan));
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(false));

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenInteractionDoesNotExist()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<Models.ActionPlan>(_request)).Returns(Task.FromResult<Models.ActionPlan>(_actionPlan));
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _resourceHelper.Setup(x => x.DoesInteractionExistAndBelongToCustomer(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(false);

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenUnableToCreateActionPlanRecord()
        {
            // Arrange
            _actionPlan = new Models.ActionPlan() { CustomerCharterShownToCustomer = true, DateActionPlanCreated = DateTime.Now.AddDays(-1), SessionId = new Guid(ValidSessionId) };
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<Models.ActionPlan>(_request)).Returns(Task.FromResult<Models.ActionPlan>(_actionPlan));
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _resourceHelper.Setup(x => x.DoesInteractionExistAndBelongToCustomer(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
            _resourceHelper.Setup(x => x.DoesSessionExistAndBelongToCustomer(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
            _postActionPlanHttpTriggerService.Setup(x => x.CreateAsync(It.IsAny<Models.ActionPlan>())).Returns(Task.FromResult<Models.ActionPlan>(null));

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeCreated_WhenRequestIsValid()
        {
            // Arrange
            _actionPlan = new Models.ActionPlan() { CustomerCharterShownToCustomer = true, DateActionPlanCreated = DateTime.Now.AddDays(-1), SessionId = new Guid(ValidSessionId) };
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _httpRequestHelper.Setup(x => x.GetDssApimUrl(_request)).Returns("http://localhost:");
            _httpRequestHelper.Setup(x => x.GetResourceFromRequest<Models.ActionPlan>(_request)).Returns(Task.FromResult<Models.ActionPlan>(_actionPlan));
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _resourceHelper.Setup(x => x.DoesInteractionExistAndBelongToCustomer(It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
            _resourceHelper.Setup(x => x.DoesSessionExistAndBelongToCustomer(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>())).Returns(true);
            _postActionPlanHttpTriggerService.Setup(x => x.CreateAsync(It.IsAny<Models.ActionPlan>())).Returns(Task.FromResult<Models.ActionPlan>(_actionPlan));

            // Act
            var result = await RunFunction(ValidCustomerId, ValidInteractionId);

            // Assert
            Assert.That(result,Is.InstanceOf<ObjectResult>());
            Assert.That((int)HttpStatusCode.Created == ((ObjectResult)result).StatusCode);
        }
        private async Task<IActionResult> RunFunction(string customerId, string interactionId)
        {
            return await _function.Run(
                _request,
                _log.Object,
                customerId,
                interactionId).ConfigureAwait(false);
        }
    }
}