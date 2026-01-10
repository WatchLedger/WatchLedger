using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
        [Authorize(Policy = "PublicReadPolicy")]
        public async Task<ActionResult<AdvertisementResponseContract>> GetById([FromRoute] Guid advertisementId)
        {
            var userId = User.FindFirst("sub")?.Value;
            var advertisement = await _advertisementService.GetAdvertisementById(userId, advertisementId);
            if (advertisement is null)
                return NotFound(new { Message = $"Advertisement with id {advertisementId} not found." });
            return Ok(advertisement);
        }



        [HttpPut("{advertisementId:Guid}")]
        [Authorize(Policy = "AdminOrUserWritePolicy")]
        public async Task<ActionResult<AdvertisementResponseContract>> UpdateAdvertisement([FromRoute] Guid advertisementId, [FromBody] AdvertisementUpdateRequestContract request)
        {
            var sellerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            var isAdmin = await _userRoleService.UserHasRoleAsync(User, "Admin");
            var updated = await _advertisementService.UpdateAdvertisement(advertisementId, sellerIdString, isAdmin, request);
            return Ok(updated);
        }

        // this endpoint allows the public to see all published advertisements by a specific seller
        // TODO : ensure only published advertisements are returned and also implement pagination and filtering
        [HttpGet("seller/{sellerId:Guid}")]
        [Authorize(Policy = "PublicReadPolicy")]
        public async Task<ActionResult<IEnumerable<AdvertisementResponseContract>>> GetAdvertisementsBySellerId([FromRoute] Guid sellerId, [FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? watchBrand)
        {
            var advertisements = await _advertisementService.GetAdvertisementsBySellerId(sellerId, pageNumber, pageSize, watchBrand);
            return Ok(advertisements);
        }


        // this endpoint allows the public to see all published advertisements
        // TODO : ensure only published advertisements are returned
        // TODO: implement pagination and filtering
        [HttpGet]
        [Authorize(Policy = "PublicReadPolicy")]
        public async Task<ActionResult<IEnumerable<AdvertisementResponseContract>>> GetAllAdvertisements([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? watchBrand)
        {
            var advertisements = await _advertisementService.GetAllAdvertisements(pageNumber, pageSize, watchBrand);
            return Ok(advertisements);
        }

        // this enpoint allows the user te see all of their own advertisements including unpublished ones
        // TODO: implement pagination and filtering
        [HttpGet]
        [Authorize(Policy = "CollectionReadPolicy")]
        public async Task<ActionResult<IEnumerable<AdvertisementResponseContract>>> GetAdvertisementsByOwnerId([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? watchBrand)
        {
            var sellerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            var advertisements = await _advertisementService.GetAdvertisementsByOwnerId(sellerIdString, pageNumber, pageSize, watchBrand);
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
        [EnableRateLimiting("watchValuation")]
        [Authorize(Policy = "PublicReadPolicy")]
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
