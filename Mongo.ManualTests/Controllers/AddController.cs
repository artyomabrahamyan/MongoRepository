using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Mongo.Helpers;
using Microsoft.AspNetCore.Mvc;
using Mongo.ManualTests.Mongo;

namespace Mongo.ManualTests.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddController : ControllerBase
    {
        private readonly TestMongoRepository _repository;

        public AddController(TestMongoRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Tests Insert many functionality.
        /// </summary>
        /// <param name="testModels">Collection of test models.</param>
        /// <returns>Test Response.</returns>
        [HttpPost]
        public async Task<IActionResult> AddManyAsync([FromBody] IEnumerable<TestModel> testModels)
        {
            await _repository.InsertManyAsync(testModels);
            return NoContent();
        }

        /// <summary>
        /// Tests Insert many functionality.
        /// </summary>
        /// <param name="id">Test model Id.</param>
        /// <param name="testModel">Test model.</param>
        /// <returns>Test Response.</returns>
        [HttpPost("id")]
        public async Task<IActionResult> AddAsync(Guid id, [FromBody] TestModel testModel)
        {
            if (id != Guid.Empty)
            {
                testModel.PartitionKey = Constants.PartitionKey;
                await _repository.InsertAsync(testModel);
                return NoContent();
            }

            return BadRequest();
        }

        /// <summary>
        /// Tests bulk upsert many functionality.
        /// </summary>
        /// <param name="testModels">Test string property.</param>
        /// <returns>Test Response.</returns>
        [HttpPost("bulk")]
        public async Task<IActionResult> BulkAddAsync([FromBody] IEnumerable<TestModel> testModels)
        {
            await _repository.BulkUpsertAsync(
                testModels.Select(
                x => x.ToReplaceOneModel<TestModel, Guid>(y => y.Id == x.Id)),
                Constants.PartitionKey);

            return NoContent();
        }
    }
}
