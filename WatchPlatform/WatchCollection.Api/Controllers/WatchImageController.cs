using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Entities.Models;
using WatchCollection.Storage.Exceptions;

namespace WatchCollection.Api.Controllers
{
    [Route("api/Watch/{watchId:Guid}/Images")]
    [Authorize]
    [ApiController]
    //[RoleAuthorize("User", "Admin")] 
    // //only possible for logged in users and admins
    public class WatchImageController(IWatchImageService _service) : ControllerBase
    {
        [HttpPost]
        [Authorize(Policy = "CollectionWritePolicy")]
        //only possible for logged in users and admins
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
        [Authorize(Policy = "CollectionReadPolicy")]
        // only possible for logged in users and admins
        public async Task<ActionResult> GetImages([FromRoute] Guid watchId)
        {
            // this endpoint can remain open for evryone since advertisements will have the ability to show all images
            // even if the viewer is not logged in
            var result = await _service.GetAllImagesByWatchIdAsync(watchId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{imageId:Guid}")]
        [Authorize(Policy = "CollectionWritePolicy")]
        // only possible for logged in users and admins
        public async Task<ActionResult> DeleteImage([FromRoute] Guid watchId, [FromRoute] Guid imageId)
        {
            var ownerIdString = User.FindFirst("sub")?.Value ?? throw new Exception("User ID (sub claim) is missing in the token.");
            await _service.DeleteImageAsync(ownerIdString, watchId, imageId);
            return NoContent();
        }

        [HttpPut]
        [Route("{imageId:Guid}/set-primary")]
        [Authorize(Policy = "CollectionWritePolicy")]
        // only possible for logged in users and admins
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
