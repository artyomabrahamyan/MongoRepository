using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mongo.ManualTests.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Mongo.ManualTests.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetController : ControllerBase
    {
        private readonly TestMongoRepository _repository;

        public GetController(TestMongoRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Tests the MongoQueryable and find all with predicate functionality.
        /// </summary>
        /// <param name="testStringProp">Test string property.</param>
        /// <returns>Test Response.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] string testStringProp = null)
        {
            var testModels = Enumerable.Empty<TestModel>();

            if (!string.IsNullOrEmpty(testStringProp))
            {
                testModels = await _repository
                    .AsMongoQueryable(Constants.PartitionKey)
                    .Where(x => x.TestStringProp == testStringProp)
                    .ToListAsync();

                return Ok(testModels);
            }

            testModels = await _repository.FindAsync(x => true, Constants.PartitionKey);
            return Ok(testModels);
        }

        /// <summary>
        /// Tests get by id with predicate functionality.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>200.</returns>
        [HttpGet("id")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var testModels = await _repository.GetAsync(x => x.TestCompanyId == id);
            return Ok(testModels);
        }
    }
}
