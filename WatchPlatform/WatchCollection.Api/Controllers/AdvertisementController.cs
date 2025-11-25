using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;

namespace WatchCollection.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementController(IAdvertisementService _advertisementService) : ControllerBase
    {
        [HttpPost]
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
        public async Task<ActionResult<AdvertisementResponseContract>> GetById([FromRoute] Guid advertisementId)
        {
            try
            {
                var advertisement = await _advertisementService.GetAdvertisementById(advertisementId);
                if (advertisement is null)
                    return NotFound(new { Message = $"Advertisement with id {advertisementId} not found." });
                return Ok(advertisement);
            }
            catch (Exception)
            {
                return Problem("An error occurred while retrieving the advertisement.");
            }
        }
    }
}
