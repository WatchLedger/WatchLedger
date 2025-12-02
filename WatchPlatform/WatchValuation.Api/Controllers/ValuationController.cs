using Microsoft.AspNetCore.Mvc;
using WatchValuation.Api.Contracts;
using WatchValuation.Domain.Services.Interfaces;

namespace WatchValuation.Api.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuationController(IValuationService _service) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ValuationResponseContract>> GetValuation([FromBody] ValuationRequestContract request)
        {
            try
            {
                var result = await _service.GetValuation(request);
                return Ok(result);
            }
            catch (Exception)
            {
                return Problem("An error occurred while processing the valuation request."); 
            }
        }
    }
}
