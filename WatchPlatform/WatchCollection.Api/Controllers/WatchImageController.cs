using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchCollection.Domain.Services.Interfaces;

namespace WatchCollection.Api.Controllers
{
    [Route("api/Watch/{watchId:Guid}/Images")]
    [ApiController]
    public class WatchImageController(IWatchImageService _service) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> UploadImage([FromRoute] Guid watchId, IFormFile image)
        {
            try
            {
                return StatusCode((int)HttpStatusCode.NotImplemented, "Image upload functionality is not implemented yet.");
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
                return StatusCode((int)HttpStatusCode.NotImplemented, "Get images functionality is not implemented yet.");
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
                return StatusCode((int)HttpStatusCode.NotImplemented, "Delete image functionality is not implemented yet.");
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
