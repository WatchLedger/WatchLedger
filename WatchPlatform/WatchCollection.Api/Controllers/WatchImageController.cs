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
            return StatusCode((int)HttpStatusCode.NotImplemented, "Image upload functionality is not implemented yet.");
        }

        [HttpGet]
        public async Task<ActionResult> GetImages([FromRoute] Guid watchId)
        {
            return StatusCode((int)HttpStatusCode.NotImplemented, "Image retrieval functionality is not implemented yet.");
        }

        [HttpDelete]
        [Route("{watchId:Guid}/images/{imageId:Guid}")]
        public async Task<ActionResult> DeleteImage([FromRoute] Guid watchId, [FromRoute] Guid imageId)
        {
            return StatusCode((int)HttpStatusCode.NotImplemented, "Image deletion functionality is not implemented yet.");
        }

        [HttpPut]
        [Route("{watchId:Guid}/images/{imageId:Guid}/set-primary")]
        public async Task<ActionResult> SetMainImage([FromRoute] Guid watchId, [FromRoute] Guid imageId)
        {
            return StatusCode((int)HttpStatusCode.NotImplemented, "Set main image functionality is not implemented yet.");
        }
    }
}
