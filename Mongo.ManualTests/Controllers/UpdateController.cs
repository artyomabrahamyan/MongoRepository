using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mongo.ManualTests;
using Mongo.ManualTests.Mongo;
using MongoDB.Driver;

namespace Mongo.ManualTests.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {
        private readonly TestMongoRepository _repository;

        public UpdateController(TestMongoRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Tests Insert many functionality.
        /// </summary>
        /// <param name="testModel">Test string property.</param>
        /// <returns>Test Response.</returns>
        [HttpPatch]
        public async Task<IActionResult> UpdateManyAsync([FromBody] TestModel testModel)
        {
            await _repository.UpdateManyAsync(
                new ExpressionFilterDefinition<TestModel>(x => true),
                new UpdateDefinitionBuilder<TestModel>()
                .Set(x => x.TestStringProp, testModel.TestStringProp)
                .Set(x => x.TestCompanyId, testModel.TestCompanyId),
                partitionKey: Constants.PartitionKey);

            return Ok();
        }

        /// <summary>
        /// Tests Insert many functionality.
        /// </summary>
        /// <param name="id">Id for the test model.</param>
        /// <param name="testModel">Collection of test models.</param>
        /// <returns>Test Response.</returns>
        [HttpPost("id")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] TestModel testModel)
        {
            await _repository.UpdateAsync(x => x.Id == id, testModel);
            return Ok();
        }
    }
}
