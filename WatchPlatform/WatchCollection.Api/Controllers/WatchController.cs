using System.Linq.Expressions;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Exceptions;

namespace WatchCollection.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class WatchController(IWatchService _service) : ControllerBase
    {
        
        [HttpPost]
        //only possible for logged in users and admins
        //[RoleAuthorize("User", "Admin")]
        public async Task<ActionResult<WatchResponseContract>> CreateWatch([FromBody] WatchRequestContract contract)
        {
            var created = await _service.CreateWatch(contract);
            return CreatedAtAction(nameof(GetWatchById), new { watchId = created.WatchId }, created);
        }

        [HttpGet]
        [Route("{watchId:Guid}")]
        // only possible for logged in users and admins
        //[RoleAuthorize("User", "Admin")]
        public async Task<ActionResult<WatchResponseContract>> GetWatchById([FromRoute] Guid watchId)
        {
            var watch = await _service.GetWatchById(watchId);
            if (watch is null)
                return NotFound(new { Message = $"Watch with id {watchId} not found." }); // No entitynotfoundexception here since it's a get operation where null is acceptable. This improves performance by avoiding exceptions for control flow.
            return Ok(watch);
        }

        [HttpGet]
        // only possible for logged in users and admins
        //[RoleAuthorize("User", "Admin")]
        public async Task<IActionResult> GetAllWatches([FromQuery] string? brand = null)
        {
            if (!string.IsNullOrWhiteSpace(brand))
                return Ok(await _service.GetWatchesByBrand(brand));
            return Ok(await _service.GetAll());
        }

        [HttpPut]
        [Route("{watchId:Guid}")]
        // only possible for logged in users and admins
        //[RoleAuthorize("User", "Admin")]
        public async Task<ActionResult<WatchResponseContract>> Update([FromRoute] Guid watchId, [FromBody] WatchRequestContract contract)
        {
            var updatedWatch = await _service.UpdateWatch(watchId, contract);
            return Ok(updatedWatch);
        }

        [HttpDelete]
        [Route("{watchId:Guid}")]
        // only possible for logged in users and admins
        //[RoleAuthorize("User", "Admin")]
        public async Task<ActionResult> DeleteWatch([FromRoute] Guid watchId)
        {
            await _service.DeleteWatch(watchId);
            return NoContent();
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IEnumerable<string>>> GetWatchBrands()
        {
            var brands = await _service.GetWatchBrands();
            if (brands is null || !brands.Any())
                return NotFound(new { Message = "No watch brands available." });
            return Ok(brands);
        }
    }
}
