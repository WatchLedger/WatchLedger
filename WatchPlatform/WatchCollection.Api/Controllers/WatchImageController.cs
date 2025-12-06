using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Entities.Models;
using WatchCollection.Storage.Exceptions;

namespace WatchCollection.Api.Controllers
{
    [Route("api/Watch/{watchId:Guid}/Images")]
    [ApiController]
    //[Authorize]
    //[RoleAuthorize("User", "Admin")] 
    // //only possible for logged in users and admins
    public class WatchImageController(IWatchImageService _service) : ControllerBase
    {
        [HttpPost]
        //only possible for logged in users and admins
        public async Task<ActionResult> UploadImage(
            [FromRoute] Guid watchId,
            [FromForm] WatchImageRequestContract contract, 
            [FromForm] IFormFile file)
        {
            try
            {
                var fileName = file.FileName;
                var contentType = file.ContentType;
                var fileSize = file.Length;

                using var stream = file.OpenReadStream();
                var result =  await _service.UploadImageAsync(watchId, fileName, contentType, fileSize,contract, stream);
                return CreatedAtAction(nameof(GetImages), new { watchId = watchId, imageId = result.ImageId }, result);
            }
            catch(EntityNotFoundException enfe)
            {
                return NotFound( new {message = enfe.Message });
            }
            catch (Exception)
            {
                return Problem("An error occurred while uploading the image.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        // only possible for logged in users and admins
        public async Task<ActionResult> GetImages([FromRoute] Guid watchId)
        {
            try
            {
                var result = await _service.GetAllImagesByWatchIdAsync(watchId);
                return Ok(result);
            }
            catch(EntityNotFoundException enfe)
            {
                return NotFound( new {message = enfe.Message });
            }
            catch (Exception)
            {
                return Problem("An error occurred while retrieving images.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        [Route("{imageId:Guid}")]
        // only possible for logged in users and admins
        public async Task<ActionResult> DeleteImage([FromRoute] Guid watchId, [FromRoute] Guid imageId)
        {
            try
            {
                await _service.DeleteImageAsync(watchId, imageId);
                return NoContent();
            }
            catch(EntityNotFoundException enfe)
            {
                return NotFound( new {message = enfe.Message });
            }
            catch (Exception)
            {
                return Problem("An error occurred while deleting the image.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        [Route("{imageId:Guid}/set-primary")]
        // only possible for logged in users and admins
        public async Task<ActionResult> SetMainImage([FromRoute] Guid watchId, [FromRoute] Guid imageId)
        {
            try
            {
                if (imageId == Guid.Empty || watchId == Guid.Empty)
                    return BadRequest(new { message = "ImageId and WatchId cannot be empty." });
                
                var updated = await _service.SetMainImageAsync(watchId, imageId);
                return Ok(updated);
            }
            catch(EntityNotFoundException enfe)
            {
                return NotFound( new {message = enfe.Message });
            }
            catch (Exception)
            {
                return Problem("An error occurred while setting the main image.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
