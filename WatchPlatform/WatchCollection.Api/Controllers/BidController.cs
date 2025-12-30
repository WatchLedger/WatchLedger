using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Exceptions;

namespace WatchCollection.Api.Controllers
{
    [Route("api/Advertisement/{advertisementId:Guid}/[controller]")]
    [ApiController]
    public class BidController(IBidService _service) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> AddBid([FromBody] BidRequestContract contract, [FromRoute] Guid advertisementId)
        {
            try
            {
                var created = await _service.AddBidAsync(advertisementId, contract);
                return CreatedAtAction(nameof(AddBid), new { bidId = created.BidId }, created);
            } catch (Exception)
            {
                return Problem("An error occured while placing the bid. Please try again later.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBids([FromRoute] Guid advertisementId)
        {
            try
            {
                var bids = await _service.GetBidsByAdvertisementIdAsync(advertisementId);
                return Ok(bids);
            } catch (Exception)
            {
                return Problem("An error occured while retrieving the bids. Please try again later.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{bidId:Guid}")]
        public async Task<IActionResult> DeleteBid([FromRoute] Guid bidId)
        {
            try
            {
                await _service.DeleteBidAsync(bidId);
                return NoContent();
            } catch (EntityNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            } catch (Exception)
            {
                return Problem("An error occured while deleting the bid. Please try again later.", statusCode: (int)HttpStatusCode.InternalServerError);
            }   
        }
    }
}
