using Microsoft.AspNetCore.Mvc;
using StockApi.Interfaces;
using StockApi.Models;

namespace StockApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;
    private readonly ILogger<StockController> _logger;

    public StockController(IStockService stockService, ILogger<StockController> logger)
    {
        _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("tickers")]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string[]>> GetAllTickersAsync(CancellationToken cancellation = default)
    {
        var tickers = await _stockService.GetAllTickersAsync(cancellation);
        return Ok(tickers);
    }

    [HttpGet("{ticker}")]
    [ProducesResponseType(typeof(Stock), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Stock>> GetTickerDetailsAsync(string ticker, CancellationToken cancellation = default)
    {
        var stock = await _stockService.GetTickerDetailsAsync(ticker, cancellation);
        return Ok(stock);
    }

    [HttpGet("{ticker}/buy")]
    [ProducesResponseType(typeof(BuyingOption), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BuyingOption>> GetBuyingOptionAsync(string ticker, [FromQuery] decimal budget, CancellationToken cancellation = default)
    {
        var buyingOption = await _stockService.GetBuyingOptionAsync(ticker, budget, cancellation);
        return Ok(buyingOption);
    }
}