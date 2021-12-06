using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mongo.ManualTests.Mongo;

namespace Mongo.ManualTests.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeleteController : ControllerBase
    {
        private readonly TestMongoRepository _repository;

        public DeleteController(TestMongoRepository repository)
        {
            _repository = repository;
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id, Constants.PartitionKey);
            return NoContent();
        }
    }
}
