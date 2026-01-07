using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Exceptions;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Exceptions;

namespace WatchCollection.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AdvertisementController(IAdvertisementService _advertisementService) : ControllerBase
    {
        [HttpPost]
        //only possible for logged in users and admins
        //[RoleAuthorize("User", "Admin")]
        public async Task<ActionResult<AdvertisementResponseContract>> CreateAdvertisement([FromBody] AdvertisementRequestContract request)
        {
            var created = await _advertisementService.CreateAdvertisement(request);
            return CreatedAtAction(nameof(GetById), new { advertisementId = created.AdvertisementId}, created);  
        }

        [HttpGet("{advertisementId:Guid}")]
        //possible for all users
        public async Task<ActionResult<AdvertisementResponseContract>> GetById([FromRoute] Guid advertisementId)
        {
            var advertisement = await _advertisementService.GetAdvertisementById(advertisementId);
            if (advertisement is null)
                return NotFound(new { Message = $"Advertisement with id {advertisementId} not found." });
            return Ok(advertisement);
        }

        [HttpPut("{advertisementId:Guid}")]
        // only possible for logged in users and admins
        //[RoleAuthorize("User", "Admin")]
        public async Task<ActionResult<AdvertisementResponseContract>> UpdateAdvertisement([FromRoute] Guid advertisementId, [FromBody] AdvertisementUpdateRequestContract request)
        {
            var updated = await _advertisementService.UpdateAdvertisement(advertisementId, request);
            return Ok(updated);
        }

        [HttpGet]
        //possible for all users
        public async Task<ActionResult<IEnumerable<AdvertisementResponseContract>>> GetAllAdvertisements()
        {
            var advertisements = await _advertisementService.GetAllAdvertisements();
            return Ok(advertisements);
        }

        [HttpDelete("{advertisementId:Guid}")]
        // only possible for logged in users and admins
        //[RoleAuthorize("User", "Admin")]
        public async Task<ActionResult> DeleteAdvertisement([FromRoute] Guid advertisementId)
        {
            await _advertisementService.DeleteAdvertisement(advertisementId);
            return NoContent();
        }

        [HttpGet("valuation")]
        public async Task<ActionResult<decimal>> GetWatchValuation([FromQuery] string referenceNumber)
        {
            var valuation = await _advertisementService.GetWatchValuation(referenceNumber);
            if (valuation == default)
            {
                return NotFound(new { Message = "No valuation available for the provided reference number." });
            }
            return Ok(valuation);
        }
    }
}
