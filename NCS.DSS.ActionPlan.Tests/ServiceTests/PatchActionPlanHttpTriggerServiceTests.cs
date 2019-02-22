using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NCS.DSS.ActionPlan.Cosmos.Provider;
using NCS.DSS.ActionPlan.Models;
using NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;

namespace NCS.DSS.ActionPlan.Tests.ServiceTests
{
    [TestFixture]
    public class PatchActionPlanHttpTriggerServiceTests
    {
        private IPatchActionPlanHttpTriggerService _actionPlanHttpTriggerService;
        private IActionPlanPatchService _actionPlanPatchService;
        private IDocumentDBProvider _documentDbProvider;
        private string _json;
        private Models.ActionPlan _actionPlan;
        private ActionPlanPatch _actionPlanPatch;
        private readonly Guid _actionPlanId = Guid.Parse("7E467BDB-213F-407A-B86A-1954053D3C24");

        [SetUp]
        public void Setup()
        {
            _actionPlanPatchService = Substitute.For<IActionPlanPatchService>();
            _documentDbProvider = Substitute.For<IDocumentDBProvider>();
            _actionPlanHttpTriggerService = Substitute.For<PatchActionPlanHttpTriggerService>(_actionPlanPatchService, _documentDbProvider);
            _actionPlanPatch = Substitute.For<ActionPlanPatch>();
            _actionPlan = Substitute.For<Models.ActionPlan>();

            _json = JsonConvert.SerializeObject(_actionPlanPatch);
            _actionPlanPatchService.Patch(_json, _actionPlanPatch).Returns(_actionPlan.ToString());
        }

        [Test]
        public void PatchActionPlanHttpTriggerServiceTests_PatchResource_ReturnsNullWhenActionPlanJsonIsNullOrEmpty()
        {
            // Act
            var result = _actionPlanHttpTriggerService.PatchResource(null, Arg.Any<ActionPlanPatch>());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void PatchActionPlanHttpTriggerServiceTests_PatchResource_ReturnsNullWhenActionPlanPatchIsNullOrEmpty()
        {
            // Act
            var result = _actionPlanHttpTriggerService.PatchResource(Arg.Any<string>(), null);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task PatchActionPlanHttpTriggerServiceTests_UpdateAsync_ReturnsNullWhenActionPlanIsNullOrEmpty()
        {
            // Act
            var result = await _actionPlanHttpTriggerService.UpdateCosmosAsync(null, _actionPlanId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task PatchActionPlanHttpTriggerServiceTests_UpdateAsync_ReturnsNullWhenActionPlanPatchServicePatchJsonIsNullOrEmpty()
        {
            _actionPlanPatchService.Patch(Arg.Any<string>(), Arg.Any<ActionPlanPatch>()).ReturnsNull();

            // Act
            var result = await _actionPlanHttpTriggerService.UpdateCosmosAsync(_actionPlan.ToString(), _actionPlanId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task PatchActionPlanHttpTriggerServiceTests_UpdateAsync_ReturnsNullWhenResourceCannotBeUpdated()
        {
            _documentDbProvider.UpdateActionPlanAsync(_json, _actionPlanId).ReturnsNull();

            // Act
            var result = await _actionPlanHttpTriggerService.UpdateCosmosAsync(_actionPlan.ToString(), _actionPlanId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task PatchActionPlanHttpTriggerServiceTests_UpdateAsync_ReturnsNullWhenResourceCannotBeFound()
        {
            _documentDbProvider.CreateActionPlanAsync(Arg.Any<Models.ActionPlan>()).Returns(Task.FromResult(new ResourceResponse<Document>(null)).Result);

            // Act
            var result = await _actionPlanHttpTriggerService.UpdateCosmosAsync(_actionPlan.ToString(), _actionPlanId);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task PatchActionPlanHttpTriggerServiceTests_UpdateAsync_ReturnsResourceWhenUpdated()
        {
            const string documentServiceResponseClass = "Microsoft.Azure.Documents.DocumentServiceResponse, Microsoft.Azure.DocumentDB.Core, Version=2.2.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";
            const string dictionaryNameValueCollectionClass = "Microsoft.Azure.Documents.Collections.DictionaryNameValueCollection, Microsoft.Azure.DocumentDB.Core, Version=2.2.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";

            var resourceResponse = new ResourceResponse<Document>(new Document());
            var documentServiceResponseType = Type.GetType(documentServiceResponseClass);

            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var headers = new NameValueCollection { { "x-ms-request-charge", "0" } };

            var headersDictionaryType = Type.GetType(dictionaryNameValueCollectionClass);

            var headersDictionaryInstance = Activator.CreateInstance(headersDictionaryType, headers);

            var arguments = new[] { Stream.Null, headersDictionaryInstance, HttpStatusCode.OK, null };

            var documentServiceResponse = documentServiceResponseType.GetTypeInfo().GetConstructors(flags)[0].Invoke(arguments);

            var responseField = typeof(ResourceResponse<Document>).GetTypeInfo().GetField("response", flags);

            responseField?.SetValue(resourceResponse, documentServiceResponse);

            _documentDbProvider.UpdateActionPlanAsync(_json,_actionPlanId).Returns(Task.FromResult(resourceResponse).Result);

            // Act
            var result = await _actionPlanHttpTriggerService.UpdateCosmosAsync(_json, _actionPlanId);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<Models.ActionPlan>(result);

        }

        [Test]
        public async Task PatchActionPlanHttpTriggerServiceTests_GetActionPlanForCustomerAsync_ReturnsNullWhenResourceHasNotBeenFound()
        {
            _documentDbProvider.GetActionPlanForCustomerToUpdateAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).ReturnsNull();

            // Act
            var result = await _actionPlanHttpTriggerService.GetActionPlanForCustomerAsync(Arg.Any<Guid>(), Arg.Any<Guid>());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task PatchActionPlanHttpTriggerServiceTests_GetActionPlanForCustomerAsync_ReturnsResourceWhenResourceHasBeenFound()
        {
            _documentDbProvider.GetActionPlanForCustomerToUpdateAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(Task.FromResult(_json).Result);

            // Act
            var result = await _actionPlanHttpTriggerService.GetActionPlanForCustomerAsync(Arg.Any<Guid>(), Arg.Any<Guid>());

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<string>(result);
        }
    }
}