using StockApi.Interfaces;
using StockApi.Models;

namespace StockApi.Services;

public class StockService : IStockService
{
    private readonly IStockRepository _stockRepository;
    private readonly ILogger<StockService> _logger;

    public StockService(IStockRepository stockRepository, ILogger<StockService> logger)
    {
        _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<string>> GetAllTickersAsync(CancellationToken cancellation = default)
    {
        var tickers = await _stockRepository.GetAllTickersAsync(cancellation);
        _logger.LogInformation("Retrieved {Count} tickers", tickers.Count());
        return tickers;
    }

    public async Task<Stock> GetTickerDetailsAsync(string ticker, CancellationToken cancellation = default)
    {
        if (string.IsNullOrWhiteSpace(ticker))
        {
            throw new ArgumentException("Ticker cannot be null or empty", nameof(ticker));
        }

        var stock = await _stockRepository.GetStockAsync(ticker, cancellation);
        return stock;
    }

    public async Task<BuyingOption> GetBuyingOptionAsync(string ticker, decimal budget, CancellationToken cancellation = default)
    {
        if (string.IsNullOrWhiteSpace(ticker))
        {
            throw new ArgumentException("Ticker cannot be null or empty", nameof(ticker));
        }

        if (budget <= 0)
        {
            throw new ArgumentException("Budget must be greater than zero", nameof(budget));
        }

        _logger.LogInformation("Calculating buying option for ticker: {Ticker} with budget: {Budget:C}", ticker, budget);

        var stock = await _stockRepository.GetStockAsync(ticker, cancellation);

        var sharePrice = stock.Close;
        var shares = (int)Math.Floor(budget / sharePrice);
        var totalCost = shares * sharePrice;
        var remainingBudget = budget - totalCost;

        var buyingOption = new BuyingOption
        {
            Ticker = ticker,
            Budget = budget,
            Shares = shares,
            SharePrice = sharePrice,
            TotalCost = totalCost,
            RemainingBudget = remainingBudget
        };

        _logger.LogInformation("Calculated buying option for {Ticker}: {Shares} shares at {SharePrice:C} each",
            ticker, shares, sharePrice);

        return buyingOption;
    }
}