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
            try
            {
                var created = await _service.CreateWatch(contract);
                return CreatedAtAction(nameof(GetWatchById), new { watchId = created.WatchId }, created);
            }
            catch (Exception)
            {
                return Problem("An error occured while creating the watch. Please try again later.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("{watchId:Guid}")]
        // only possible for logged in users and admins
        //[RoleAuthorize("User", "Admin")]
        public async Task<ActionResult<WatchResponseContract>> GetWatchById([FromRoute] Guid watchId)
        {
            try
            {
                var watch = await _service.GetWatchById(watchId);
                if (watch is null)
                    return NotFound(new { Message = $"Watch with id {watchId} not found." }); // No entitynotfoundexception here since it's a get operation where null is acceptable. This improves performance by avoiding exceptions for control flow.
                return Ok(watch);
            }
            catch (Exception)
            {
                return Problem("An error occured while retrieving the watch. Please try again later.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        // only possible for logged in users and admins
        //[RoleAuthorize("User", "Admin")]
        public async Task<IActionResult> GetAllWatches([FromQuery] string? brand = null)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(brand))
                {
                    return Ok(await _service.GetWatchesByBrand(brand));
                }
                return Ok(await _service.GetAll());
            }
            catch (Exception)
            {
                return Problem("An error occured while retrieving the watches. Please try again later.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        [Route("{watchId:Guid}")]
        // only possible for logged in users and admins
        //[RoleAuthorize("User", "Admin")]
        public async Task<ActionResult<WatchResponseContract>> Update([FromRoute] Guid watchId, [FromBody] WatchRequestContract contract)
        {
            try
            {
                var updatedWatch = await _service.UpdateWatch(watchId, contract);
                return Ok(updatedWatch);
            }
            catch (EntityNotFoundException enfe)
            {
                return NotFound(new { enfe.Message }); // entitynotfoundexception here instead of returning null, since we're trying to update a resource that may not exist.
            }
            catch (Exception)
            {
                return Problem("An error occured while updating the watch. Please try again later.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        [Route("{watchId:Guid}")]
        // only possible for logged in users and admins
        //[RoleAuthorize("User", "Admin")]
        public async Task<ActionResult> DeleteWatch([FromRoute] Guid watchId)
        {
            try
            {
                await _service.DeleteWatch(watchId);
                return NoContent();
            }
            catch (EntityNotFoundException enfe)
            {
                return NotFound(new { enfe.Message }); // entitynotfoundexception here instead of returning null, since we're trying to delete a resource that may not exist.
            }
            catch (Exception)
            {
                return Problem("An error occured while deleting the watch. Please try again later.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ActionResult<IEnumerable<string>>> GetWatchBrands()
        {
            try
            {
                var brands = await _service.GetWatchBrands();
                return Ok(brands);
            }
            catch (Exception)
            {
                return Problem("An error occured while retrieving watch brands. Please try again later.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
