using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;
using WatchCollection.Storage.Entities.Models;

namespace WatchCollection.Api.Controllers
{
    [Route("api/Watch/{watchId:Guid}/Images")]
    [ApiController]
    public class WatchImageController(IWatchImageService _service) : ControllerBase
    {
        [HttpPost]
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
            catch (Exception)
            {
                return Problem("An error occurred while uploading the image.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetImages([FromRoute] Guid watchId)
        {
            try
            {
                var result = await _service.GetAllImagesByWatchIdAsync(watchId);
                return Ok(result);
            }
            catch (Exception)
            {
                return Problem("An error occurred while retrieving images.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        [Route("{imageId:Guid}")]
        public async Task<ActionResult> DeleteImage([FromRoute] Guid watchId, [FromRoute] Guid imageId)
        {
            try
            {
                await _service.DeleteImageAsync(watchId, imageId);
                return NoContent();
            }
            catch (Exception)
            {
                return Problem("An error occurred while deleting the image.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut]
        [Route("{imageId:Guid}/set-primary")]
        public async Task<ActionResult> SetMainImage([FromRoute] Guid watchId, [FromRoute] Guid imageId)
        {
            try
            {
                return StatusCode((int)HttpStatusCode.NotImplemented, "Set main image functionality is not implemented yet.");
            }
            catch (Exception)
            {
                return Problem("An error occurred while setting the main image.", statusCode: (int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
