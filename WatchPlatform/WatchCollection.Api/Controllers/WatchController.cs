using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchCollection.Api.Contracts;
using WatchCollection.Domain.Services.Interfaces;

namespace WatchCollection.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchController(IWatchService service) : ControllerBase
    {
        [HttpPost]
        public ActionResult<WatchResponseContract> Create([FromBody] WatchRequestContract contract)
        {
            try
            {
                var created = service.CreateWatch(contract);
                return CreatedAtAction(nameof(Get), new { watchId = created.WatchId }, created);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("{watchId:Guid}")]
        public ActionResult<WatchResponseContract> Get([FromRoute] Guid watchId)
        {
            try
            {
                var watch = service.GetWatchById(watchId);
                if (watch is null)
                    return NotFound();
                return Ok(watch);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(service.GetAll());
        }
    }
}
