using Application;
using Microsoft.AspNetCore.Mvc;

namespace Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataLoaderController :ControllerBase
    {
        private readonly HistoricalTradeService _historicalTradeService;

        public DataLoaderController(HistoricalTradeService historicalTradeService ) {

            _historicalTradeService = historicalTradeService;

        }

        [HttpGet("historical-trade")]
        public IActionResult HistoricalTrade()
        {
            _historicalTradeService.GetHistoricalTrades();
            return Ok();
        }
    }
}
