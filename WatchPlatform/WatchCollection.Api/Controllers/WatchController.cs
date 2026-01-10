using System.Linq.Expressions;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Exceptions;

namespace WatchCollection.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class WatchController(IWatchService _service) : ControllerBase
    {
        
        [HttpPost]
        [Authorize(Policy = "CollectionWritePolicy")]
        public async Task<ActionResult<WatchResponseContract>> CreateWatch([FromBody] WatchRequestContract contract)
        {
            var ownerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            var created = await _service.CreateWatch(ownerIdString, contract);
            return CreatedAtAction(nameof(GetWatchById), new { watchId = created.WatchId }, created);
        }

        [HttpGet]
        [Route("{watchId:Guid}")]
        [Authorize(Policy = "CollectionReadPolicy")]
        public async Task<ActionResult<WatchResponseContract>> GetWatchById([FromRoute] Guid watchId)
        {
            var ownerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            var watch = await _service.GetWatchById(ownerIdString, watchId);
            if (watch is null)
                return NotFound(new { Message = $"Watch with id {watchId} not found." }); // No entitynotfoundexception here since it's a get operation where null is acceptable. This improves performance by avoiding exceptions for control flow.
            return Ok(watch);
        }

        [HttpGet]
        [Authorize(Policy = "CollectionReadPolicy")]
        public async Task<IActionResult> GetAllWatches([FromQuery] string? brand = null)
        {
            var ownerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            if (!string.IsNullOrWhiteSpace(brand))
                return Ok(await _service.GetWatchesByBrand(ownerIdString, brand));
            return Ok(await _service.GetAll(ownerIdString));
        }

        [HttpPut]
        [Route("{watchId:Guid}")]
        [Authorize(Policy = "CollectionWritePolicy")]
        public async Task<ActionResult<WatchResponseContract>> Update([FromRoute] Guid watchId, [FromBody] WatchRequestContract contract)
        {
            var ownerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            var updatedWatch = await _service.UpdateWatch(ownerIdString, watchId, contract);
            return Ok(updatedWatch);
        }

        [HttpDelete]
        [Route("{watchId:Guid}")]
        [Authorize(Policy = "CollectionWritePolicy")]
        public async Task<ActionResult> DeleteWatch([FromRoute] Guid watchId)
        {
            var ownerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            await _service.DeleteWatch(ownerIdString, watchId);
            return NoContent();
        }

        [HttpGet("brands")]
        [EnableRateLimiting("brandList")]
        [Authorize(Policy = "PublicReadPolicy")]
        public async Task<ActionResult<IEnumerable<string>>> GetWatchBrands()
        {
            var brands = await _service.GetWatchBrands();
            if (brands is null || !brands.Any())
                return NotFound(new { Message = "No watch brands available." });
            return Ok(brands);
        }
    }
}
