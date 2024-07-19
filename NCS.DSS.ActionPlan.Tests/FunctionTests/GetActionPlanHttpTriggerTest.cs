using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Service;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.Tests.FunctionTests
{
    [TestFixture]
    public class GetActionPlanHttpTriggerTest
    {
        private const string ValidCustomerId = "7E467BDB-213F-407A-B86A-1954053D3C24";
        private const string ValidDssCorrelationId = "452d8e8c-2516-4a6b-9fc1-c85e578ac066";
        private const string InValidId = "1111111-2222-3333-4444-555555555555";

        private Mock<ILogger> _log;
        private HttpRequest _request;
        private Mock<IResourceHelper> _resourceHelper;
        private Mock<IGetActionPlanHttpTriggerService> _getActionPlanHttpTriggerService;
        private Mock<ILoggerHelper> _loggerHelper;
        private Mock<IHttpRequestHelper> _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IJsonHelper _jsonHelper;
        private GetActionPlanHttpTrigger.Function.GetActionPlanHttpTrigger _function;

        [SetUp]
        public void Setup()
        {
            _request = (new DefaultHttpContext()).Request;

            _resourceHelper = new Mock<IResourceHelper>();
            _loggerHelper = new Mock<ILoggerHelper>();
            _httpRequestHelper = new Mock<IHttpRequestHelper>();
            _httpResponseMessageHelper = new HttpResponseMessageHelper();
            _jsonHelper = new JsonHelper();
            _log = new Mock<ILogger>();
            _resourceHelper = new Mock<IResourceHelper>();
            _getActionPlanHttpTriggerService = new Mock<IGetActionPlanHttpTriggerService>();
            _function = new GetActionPlanHttpTrigger.Function.GetActionPlanHttpTrigger(_resourceHelper.Object, _getActionPlanHttpTriggerService.Object, _loggerHelper.Object, _httpRequestHelper.Object, _httpResponseMessageHelper, _jsonHelper);
        }

        [Test]
        public async Task GetActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssCorrelationId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");

            // Act and Assert
            await ActAndAssert(HttpStatusCode.NoContent);
        }

        [Test]
        public async Task GetActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssCorrelationId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _resourceHelper.Setup(x=>x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(false));

            /// Act and Assert
            await ActAndAssert(HttpStatusCode.NoContent);
        }

        [Test]
        public async Task GetActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenActionPlanDoesNotExist()
        {
            // Arrange
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetDssCorrelationId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _getActionPlanHttpTriggerService.Setup(x=>x.GetActionPlansAsync(It.IsAny<Guid>())).Returns(Task.FromResult<List<Models.ActionPlan>>(null));

            // Act and Assert
            await ActAndAssert(HttpStatusCode.NoContent);
        }

        [Test]
        public async Task GetActionPlanHttpTrigger_ReturnsStatusCodeOk_WhenActionPlanExists()
        {
            // Arrange
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetDssCorrelationId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            var listOfActionPlanes = new List<Models.ActionPlan>();
            _getActionPlanHttpTriggerService.Setup(x=>x.GetActionPlansAsync(It.IsAny<Guid>())).Returns(Task.FromResult(listOfActionPlanes));

            // Act and Assert
            await ActAndAssert(HttpStatusCode.OK);
        }

        private async Task<bool> ActAndAssert(HttpStatusCode statusCode)
        {
            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.That(typeof(HttpResponseMessage) == result.GetType());
            Assert.That(statusCode == result.StatusCode);
            return true;
        }

        private async Task<HttpResponseMessage> RunFunction(string customerId)
        {
            return await _function.Run(
                _request,
                _log.Object,
                customerId).ConfigureAwait(false);
        }
    }
}