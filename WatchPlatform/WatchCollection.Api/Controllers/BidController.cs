using System.Net;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Exceptions;

namespace WatchCollection.Api.Controllers
{
    [Route("api/Advertisement/{advertisementId:Guid}/[controller]")]
    [Authorize]
    [ApiController]
    public class BidController(IBidService _service) : ControllerBase
    {
        [HttpPost]
        [Authorize(Policy = "CollectionWritePolicy")]
        public async Task<IActionResult> AddBid([FromBody] BidRequestContract contract, [FromRoute] Guid advertisementId)
        {
            var bidderIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            Console.WriteLine($"BidderId: {bidderIdString}");
            var created = await _service.AddBidAsync(advertisementId, bidderIdString, contract);
            return CreatedAtAction(nameof(AddBid), new { bidId = created.BidId }, created);
        }

        [HttpGet]
        [Authorize(Policy = "PublicReadPolicy")]
        public async Task<IActionResult> GetBids([FromRoute] Guid advertisementId)
        {
            var bids = await _service.GetBidsByAdvertisementIdAsync(advertisementId);
            return Ok(bids);
        }

        [HttpDelete("{bidId:Guid}")]
        [Authorize(Policy = "AdminWritePolicy")]
        // for fairness, only admins can delete bids when necessary
        public async Task<IActionResult> DeleteBid([FromRoute] Guid bidId)
        {
            await _service.DeleteBidAsync(bidId);
            return NoContent();
        }
    }
}
