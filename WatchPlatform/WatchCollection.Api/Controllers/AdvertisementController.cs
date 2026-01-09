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
        [Authorize(Policy = "CollectionWritePolicy")]
        public async Task<ActionResult<AdvertisementResponseContract>> CreateAdvertisement([FromBody] AdvertisementRequestContract request)
        {
            var sellerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            var created = await _advertisementService.CreateAdvertisement(sellerIdString, request);
            return CreatedAtAction(nameof(GetById), new { advertisementId = created.AdvertisementId}, created);  
        }

        [HttpGet("{advertisementId:Guid}")]
        [Authorize(Policy = "CollectionReadPolicy")]
        public async Task<ActionResult<AdvertisementResponseContract>> GetById([FromRoute] Guid advertisementId)
        {
            var userIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            var advertisement = await _advertisementService.GetAdvertisementById(userIdString, advertisementId);
            if (advertisement is null)
                return NotFound(new { Message = $"Advertisement with id {advertisementId} not found." });
            return Ok(advertisement);
        }

        [HttpPut("{advertisementId:Guid}")]
        [Authorize(Policy = "CollectionWritePolicy")]
        public async Task<ActionResult<AdvertisementResponseContract>> UpdateAdvertisement([FromRoute] Guid advertisementId, [FromBody] AdvertisementUpdateRequestContract request)
        {
            // TODO: pass along the role claims to service layer to handle admin overrides
            var sellerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            var updated = await _advertisementService.UpdateAdvertisement(advertisementId, sellerIdString, request);
            return Ok(updated);
        }

        [HttpGet]
        [Authorize(Policy = "CollectionReadPolicy")]
        public async Task<ActionResult<IEnumerable<AdvertisementResponseContract>>> GetAllAdvertisements()
        {
            var advertisements = await _advertisementService.GetAllAdvertisements();
            return Ok(advertisements);
        }

        [HttpDelete("{advertisementId:Guid}")]
        [Authorize(Policy = "CollectionWritePolicy")]
        public async Task<ActionResult> DeleteAdvertisement([FromRoute] Guid advertisementId)
        {
            // TODO: pass along the role claims to service layer to handle admin overrides
            var sellerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            await _advertisementService.DeleteAdvertisement(sellerIdString, advertisementId);
            return NoContent();
        }

        [HttpGet("valuation")]
        [Authorize(Policy = "CollectionReadPolicy")]
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
