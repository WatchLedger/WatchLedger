using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;

namespace WatchCollection.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchController(IWatchService _service) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<WatchResponseContract>> Create([FromBody] WatchRequestContract contract)
        {
            try
            {
                var created = await _service.CreateWatch(contract);
                return CreatedAtAction(nameof(Get), new { watchId = created.WatchId }, created);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{watchId:Guid}")]
        public async Task<ActionResult<WatchResponseContract>> Get([FromRoute] Guid watchId)
        {
            try
            {
                var watch = await _service.GetWatchById(watchId);
                if (watch is null)
                    return NotFound();
                return Ok(watch);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpPut]
        [Route("{watchId:Guid}")]
        public async Task<ActionResult<WatchResponseContract>> Update([FromRoute] Guid watchId, [FromBody] WatchRequestContract contract)
        {
            try
            {
                var updatedWatch = await _service.UpdateWatch(watchId, contract);
                if (updatedWatch is null)
                    return BadRequest();
                return Ok(updatedWatch);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
