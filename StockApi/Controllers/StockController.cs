using Mapster;
using Microsoft.AspNetCore.Mvc;
using StockApi.Dtos;
using StockApi.Interfaces;
using StockApi.Models;

namespace StockApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;
    private readonly ILogger<StockController> _logger;

    // NOTE: For larger APIs, consider using the CQRS pattern (e.g., with MediatR) or implementing dedicated handlers/endpoints for better separation of concerns and scalability.
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
    [ProducesResponseType(typeof(StockDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Stock>> GetTickerDetailsAsync(string ticker, CancellationToken cancellation = default)
    {
        var stock = await _stockService.GetTickerDetailsAsync(ticker, cancellation);
        var result = stock.Adapt<StockDto>();
        return Ok(result);
    }

    [HttpGet("{ticker}/buy")]
    [ProducesResponseType(typeof(BuyingOptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BuyingOption>> GetBuyingOptionAsync(string ticker, [FromQuery] decimal budget, CancellationToken cancellation = default)
    {
        var buyingOption = await _stockService.GetBuyingOptionAsync(ticker, budget, cancellation);
        var result = buyingOption.Adapt<BuyingOptionDto>();
        return Ok(result);
    }
}