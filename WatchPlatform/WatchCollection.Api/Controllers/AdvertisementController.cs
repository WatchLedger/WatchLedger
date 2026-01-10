using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchCollection.Api.Contracts;
using WatchCollection.Api.Services;
using WatchCollection.Domain.Services.Exceptions;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Exceptions;

namespace WatchCollection.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AdvertisementController(IAdvertisementService _advertisementService, IUserRoleService _userRoleService) : ControllerBase
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
            var userId = User.FindFirst("sub")?.Value;
            var advertisement = await _advertisementService.GetAdvertisementById(userId, advertisementId);
            if (advertisement is null)
                return NotFound(new { Message = $"Advertisement with id {advertisementId} not found." });
            return Ok(advertisement);
        }

        // TODO: maybe add a getbysellerid endpoint?

        [HttpPut("{advertisementId:Guid}")]
        [Authorize(Policy = "AdminOrUserWritePolicy")]
        public async Task<ActionResult<AdvertisementResponseContract>> UpdateAdvertisement([FromRoute] Guid advertisementId, [FromBody] AdvertisementUpdateRequestContract request)
        {
            var sellerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            var isAdmin = await _userRoleService.UserHasRoleAsync(User, "Admin");
            var updated = await _advertisementService.UpdateAdvertisement(advertisementId, sellerIdString, isAdmin, request);
            return Ok(updated);
        }

        // TODO: maybe getall for specific users? dont know yet
        [HttpGet]
        [Authorize(Policy = "CollectionReadPolicy")]
        public async Task<ActionResult<IEnumerable<AdvertisementResponseContract>>> GetAllAdvertisements()
        {
            var advertisements = await _advertisementService.GetAllAdvertisements();
            return Ok(advertisements);
        }

        [HttpDelete("{advertisementId:Guid}")]
        [Authorize(Policy = "AdminOrUserWritePolicy")]
        public async Task<ActionResult> DeleteAdvertisement([FromRoute] Guid advertisementId)
        {
            var sellerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            var isAdmin = await _userRoleService.UserHasRoleAsync(User, "Admin");
            await _advertisementService.DeleteAdvertisement(sellerIdString, isAdmin, advertisementId);
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
