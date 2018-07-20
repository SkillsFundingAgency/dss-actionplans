using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.Helpers;
using NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Validation;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace NCS.DSS.ActionPlan.Tests
{
    [TestFixture]
    public class PostActionPlanHttpTriggerTests
    {
        private const string ValidCustomerId = "7E467BDB-213F-407A-B86A-1954053D3C24";
        private const string ValidInteractionId = "1e1a555c-9633-4e12-ab28-09ed60d51cb3";
        private const string InValidId = "1111111-2222-3333-4444-555555555555";
        private ILogger _log;
        private HttpRequestMessage _request;
        private IResourceHelper _resourceHelper;
        private IValidate _validate;
        private IHttpRequestMessageHelper _httpRequestMessageHelper;
        private IPostActionPlanHttpTriggerService _postActionPlanHttpTriggerService;
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
                            $"ActionPlans")
            };

            _log = Substitute.For<ILogger>();
            _resourceHelper = Substitute.For<IResourceHelper>();
            _httpRequestMessageHelper = Substitute.For<IHttpRequestMessageHelper>();
            _validate = Substitute.For<IValidate>();
            _postActionPlanHttpTriggerService = Substitute.For<IPostActionPlanHttpTriggerService>();
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            // Act
            var result = await RunFunction(InValidId, ValidInteractionId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenInteractionIdIsInvalid()
        {
            // Act
            var result = await RunFunction(ValidCustomerId, InValidId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenActionPlanHasFailedValidation()
        {
            _httpRequestMessageHelper.GetActionPlanFromRequest<Models.ActionPlan>(_request).Returns(Task.FromResult(_actionPlan).Result);

            var validationResults = new List<ValidationResult> { new ValidationResult("interaction Id is Required") };
            _validate.ValidateResource(Arg.Any<Models.ActionPlan>()).Returns(validationResults);

            var result = await RunFunction(ValidCustomerId, ValidInteractionId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual((HttpStatusCode)422, result.StatusCode);
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeUnprocessableEntity_WhenActionPlanRequestIsInvalid()
        {
            _httpRequestMessageHelper.GetActionPlanFromRequest<Models.ActionPlan>(_request).Throws(new JsonException());

            var result = await RunFunction(ValidCustomerId, ValidInteractionId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual((HttpStatusCode)422, result.StatusCode);
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            _httpRequestMessageHelper.GetActionPlanFromRequest<Models.ActionPlan>(_request).Returns(Task.FromResult(_actionPlan).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).Returns(false);

            var result = await RunFunction(ValidCustomerId, ValidInteractionId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenInteractionDoesNotExist()
        {
            _httpRequestMessageHelper.GetActionPlanFromRequest<Models.ActionPlan>(_request).Returns(Task.FromResult(_actionPlan).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).Returns(true);
            _resourceHelper.DoesInteractionExist(Arg.Any<Guid>()).Returns(false);

            var result = await RunFunction(ValidCustomerId, ValidInteractionId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenUnableToCreateActionPlanRecord()
        {
            _httpRequestMessageHelper.GetActionPlanFromRequest<Models.ActionPlan>(_request).Returns(Task.FromResult(_actionPlan).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).ReturnsForAnyArgs(true);
            _resourceHelper.DoesInteractionExist(Arg.Any<Guid>()).Returns(true);

            _postActionPlanHttpTriggerService.CreateAsync(Arg.Any<Models.ActionPlan>()).Returns(Task.FromResult<Models.ActionPlan>(null).Result);

            var result = await RunFunction(ValidCustomerId, ValidInteractionId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeCreated_WhenRequestIsNotValid()
        {
            _httpRequestMessageHelper.GetActionPlanFromRequest<Models.ActionPlan>(_request).Returns(Task.FromResult(_actionPlan).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).ReturnsForAnyArgs(true);
            _resourceHelper.DoesInteractionExist(Arg.Any<Guid>()).Returns(true);

            _postActionPlanHttpTriggerService.CreateAsync(Arg.Any<Models.ActionPlan>()).Returns(Task.FromResult<Models.ActionPlan>(null).Result);

            var result = await RunFunction(ValidCustomerId, ValidInteractionId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [Test]
        public async Task PostActionPlanHttpTrigger_ReturnsStatusCodeCreated_WhenRequestIsValid()
        {
            _httpRequestMessageHelper.GetActionPlanFromRequest<Models.ActionPlan>(_request).Returns(Task.FromResult(_actionPlan).Result);

            _resourceHelper.DoesCustomerExist(Arg.Any<Guid>()).ReturnsForAnyArgs(true);
            _resourceHelper.DoesInteractionExist(Arg.Any<Guid>()).Returns(true);

            _postActionPlanHttpTriggerService.CreateAsync(Arg.Any<Models.ActionPlan>()).Returns(Task.FromResult(_actionPlan).Result);

            var result = await RunFunction(ValidCustomerId, ValidInteractionId);

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
        }

        private async Task<HttpResponseMessage> RunFunction(string customerId, string interactionId)
        {
            return await PostActionPlanHttpTrigger.Function.PostActionPlanHttpTrigger.Run(
                _request, _log, customerId, interactionId, _resourceHelper, _httpRequestMessageHelper, _validate, _postActionPlanHttpTriggerService).ConfigureAwait(false);
        }

    }
}