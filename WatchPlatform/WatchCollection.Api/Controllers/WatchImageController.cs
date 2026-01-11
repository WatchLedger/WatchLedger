using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Entities.Models;
using WatchCollection.Storage.Exceptions;

namespace WatchCollection.Api.Controllers
{
    [Route("api/Watch/{watchId:Guid}/Images")]
    [Authorize]
    [ApiController]
    public class WatchImageController(IWatchImageService _service) : ControllerBase
    {
        [HttpPost]
        [EnableRateLimiting("imageUpload")]
        [Authorize(Policy = "CollectionWritePolicy")]
        public async Task<ActionResult> UploadImage(
            [FromRoute] Guid watchId,
            [FromForm] bool isPrimary,
            [FromForm] IFormFile file)
        {
            var ownerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");

            var fileName = file.FileName;
            var contentType = file.ContentType;
            var fileSize = file.Length;

            using var stream = file.OpenReadStream();
            var result =  await _service.UploadImageAsync(ownerIdString, watchId, fileName, contentType, fileSize, isPrimary, stream);
            return CreatedAtAction(nameof(GetImages), new { watchId = watchId, imageId = result.ImageId }, result);
        }

        [HttpGet]
        [Authorize(Policy = "PublicReadPolicy")]
        public async Task<ActionResult> GetImages([FromRoute] Guid watchId)
        {
            var result = await _service.GetAllImagesByWatchIdAsync(watchId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{imageId:Guid}")]
        [Authorize(Policy = "CollectionWritePolicy")]
        public async Task<ActionResult> DeleteImage([FromRoute] Guid watchId, [FromRoute] Guid imageId)
        {
            var ownerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            await _service.DeleteImageAsync(ownerIdString, watchId, imageId);
            return NoContent();
        }

        [HttpPatch]
        [Route("{imageId:Guid}/set-primary")]
        [Authorize(Policy = "CollectionWritePolicy")]
        public async Task<ActionResult> SetMainImage([FromRoute] Guid watchId, [FromRoute] Guid imageId)
        {
            var ownerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            if (imageId == Guid.Empty || watchId == Guid.Empty)
                return BadRequest(new { message = "ImageId and WatchId are required." });
                
            var updated = await _service.SetMainImageAsync(ownerIdString, watchId, imageId);
            return Ok(updated);
        }
    }
}
