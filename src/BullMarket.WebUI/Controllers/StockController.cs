using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BullMarket.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BullMarket.WebUI.Controllers
{
    [Route("api/stocks")]
    [ApiController]
    [Authorize]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            this._stockService = stockService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStocks()
        {
            var result = await _stockService.GetAllStocksAsync();

            return Ok(result);
        }

        [HttpGet("{stockId}")]
        public async Task<IActionResult> GetStockById(Guid stockId)
        {
            var result = await _stockService.GetStockByIdAsync(stockId);

            return Ok(result);
        }
    }
}
