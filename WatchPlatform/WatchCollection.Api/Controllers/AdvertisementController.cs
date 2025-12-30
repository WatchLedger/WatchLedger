using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Exceptions;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Exceptions;

namespace WatchCollection.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class AdvertisementController(IAdvertisementService _advertisementService) : ControllerBase
    {
        [HttpPost]
        
        //only possible for logged in users and admins
        //[RoleAuthorize("User", "Admin")]
        public async Task<ActionResult<AdvertisementResponseContract>> CreateAdvertisement([FromBody] AdvertisementRequestContract request)
        {
            try
            {
                var created = await _advertisementService.CreateAdvertisement(request);
                return CreatedAtAction(nameof(GetById), new { advertisementId = created.AdvertisementId}, created);
            }
            catch (Exception)
            {
                return Problem("An error occurred while creating the advertisement.");
            }
        }

        [HttpGet("{advertisementId:Guid}")]
        //possible for all users
        public async Task<ActionResult<AdvertisementResponseContract>> GetById([FromRoute] Guid advertisementId)
        {
            try
            {
                var advertisement = await _advertisementService.GetAdvertisementById(advertisementId);
                if (advertisement is null)
                    return NotFound(new { Message = $"Advertisement with id {advertisementId} not found." });
                return Ok(advertisement);
            }
            catch (AdvertisementNotFoundExceptions)
            {
                return NotFound(new { Message = $"Advertisement with id {advertisementId} not found." });
            }
            catch (Exception)
            {
                return Problem("An error occurred while retrieving the advertisement.");
            }
        }

        [HttpPut("{advertisementId:Guid}")]
        // only possible for logged in users and admins
        //[RoleAuthorize("User", "Admin")]
        public async Task<ActionResult<AdvertisementResponseContract>> UpdateAdvertisement([FromRoute] Guid advertisementId, [FromBody] AdvertisementRequestContract request)
        {
            try
            {
                var updated = await _advertisementService.UpdateAdvertisement(advertisementId, request);
                return Ok(updated);
            }
            catch (AdvertisementNotFoundExceptions)
            {
                return NotFound(new { Message = $"Advertisement with id {advertisementId} not found." });
            }
            catch (Exception)
            {
                return Problem("An error occurred while updating the advertisement.");
            }
        }

        [HttpGet]
        //possible for all users
        public async Task<ActionResult<IEnumerable<AdvertisementResponseContract>>> GetAllAdvertisements()
        {
            try
            {
                var advertisements = await _advertisementService.GetAllAdvertisements();
                return Ok(advertisements);
            }
            catch (Exception)
            {
                return Problem("An error occurred while retrieving advertisements.");
            }
        }

        [HttpDelete("{advertisementId:Guid}")]
        // only possible for logged in users and admins
        //[RoleAuthorize("User", "Admin")]
        public async Task<ActionResult> DeleteAdvertisement([FromRoute] Guid advertisementId)
        {
            try
            {
                await _advertisementService.DeleteAdvertisement(advertisementId);
                return NoContent();
            }
            catch (Exception)
            {
                return Problem("An error occurred while deleting the advertisement.");
            }
        }

        [HttpGet("valuation")]
        public async Task<ActionResult<decimal>> GetWatchValuation([FromQuery] string referenceNumber)
        {
            try 
            {
                var valuation = await _advertisementService.GetWatchValuation(referenceNumber);
                if (valuation == default)
                {
                    return NotFound(new { Message = "No valuation available for the provided reference number." });
                }
                return Ok(valuation);
            }
            catch (ValuationUnavailableException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Problem("An error occurred while retrieving the watch valuation.");
            }
        }
    }
}
