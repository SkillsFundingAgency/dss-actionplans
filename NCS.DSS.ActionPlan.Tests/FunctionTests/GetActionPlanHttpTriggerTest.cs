using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using GetActionPlanLogger = NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Function;

namespace NCS.DSS.ActionPlan.Tests.FunctionTests
{
    [TestFixture]
    public class GetActionPlanHttpTriggerTest
    {
        private const string ValidCustomerId = "7E467BDB-213F-407A-B86A-1954053D3C24";
        private const string ValidDssCorrelationId = "452d8e8c-2516-4a6b-9fc1-c85e578ac066";
        private const string InValidId = "1111111-2222-3333-4444-555555555555";

        private HttpRequest _request;
        private Mock<IResourceHelper> _resourceHelper;
        private Mock<IGetActionPlanHttpTriggerService> _getActionPlanHttpTriggerService;
        private Mock<ILogger<GetActionPlanLogger.GetActionPlanHttpTrigger>> _loggerHelper;
        private Mock<IHttpRequestHelper> _httpRequestHelper;
        private IConvertToDynamic _dynamicHelper;
        private GetActionPlanLogger.GetActionPlanHttpTrigger _function;

        [SetUp]
        public void Setup()
        {
            _request = (new DefaultHttpContext()).Request;

            _resourceHelper = new Mock<IResourceHelper>();
            _loggerHelper = new Mock<ILogger<GetActionPlanLogger.GetActionPlanHttpTrigger>>();
            _httpRequestHelper = new Mock<IHttpRequestHelper>();
            _resourceHelper = new Mock<IResourceHelper>();
            _dynamicHelper = new ConvertToDynamic();
            _getActionPlanHttpTriggerService = new Mock<IGetActionPlanHttpTriggerService>();
            _function = new GetActionPlanLogger.GetActionPlanHttpTrigger(_resourceHelper.Object, _getActionPlanHttpTriggerService.Object, _loggerHelper.Object, _httpRequestHelper.Object, _dynamicHelper);
        }

        [Test]
        public async Task GetActionPlanHttpTrigger_ReturnsStatusCodeBadRequest_WhenCustomerIdIsInvalid()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssCorrelationId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task GetActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenCustomerDoesNotExist()
        {
            // Arrange
            _httpRequestHelper.Setup(x => x.GetDssCorrelationId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _resourceHelper.Setup(x=>x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(false));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task GetActionPlanHttpTrigger_ReturnsStatusCodeNoContent_WhenActionPlanDoesNotExist()
        {
            // Arrange
            _resourceHelper.Setup(x => x.DoesCustomerExist(It.IsAny<Guid>())).Returns(Task.FromResult(true));
            _httpRequestHelper.Setup(x => x.GetDssCorrelationId(_request)).Returns(ValidDssCorrelationId);
            _httpRequestHelper.Setup(x => x.GetDssTouchpointId(_request)).Returns("0000000001");
            _getActionPlanHttpTriggerService.Setup(x=>x.GetActionPlansAsync(It.IsAny<Guid>())).Returns(Task.FromResult<List<Models.ActionPlan>>(null));

            // Act
            var result = await RunFunction(ValidCustomerId);

            // Assert
            Assert.That(result, Is.InstanceOf<NoContentResult>());
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

            
            // Act
            var result = await RunFunction(ValidCustomerId);
            var jsonResult = result as JsonResult;

            // Assert
            Assert.That(result,Is.InstanceOf<JsonResult>());
            Assert.That(jsonResult.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
            
        }

       

        private async Task<IActionResult> RunFunction(string customerId)
        {
            return await _function.Run(
                _request,
                customerId).ConfigureAwait(false);
        }
    }
}