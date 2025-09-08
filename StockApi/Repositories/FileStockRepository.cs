namespace StockApi.Repositories;

using Microsoft.Extensions.Options;
using StockApi.Exceptions;
using StockApi.Interfaces;
using StockApi.Models;
using StockApi.Options;
using System.Text.Json;

/// <summary>
/// Repository implementation for stock data using JSON file storage
/// </summary>
public class FileStockRepository : IStockRepository
{
    private readonly ILogger<FileStockRepository> _logger;
    private readonly FileStockRepositoryOptions _options;
    private readonly Lazy<List<Stock>> _stocks;

    public FileStockRepository(ILogger<FileStockRepository> logger, IOptions<FileStockRepositoryOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _stocks = new Lazy<List<Stock>>(LoadStocksData);
    }

    /// <summary>
    /// Gets all available stock tickers
    /// </summary>
    public async Task<IEnumerable<string>> GetAllTickersAsync(CancellationToken cancellation = default)
    {
        _logger.LogInformation("Retrieving all stock tickers");

        return await Task.FromResult(_stocks.Value.Select(s => s.Ticker).Distinct().ToList());
    }

    /// <summary>
    /// Gets stock by ticker symbol
    /// </summary>
    public async Task<Stock> GetStockAsync(string ticker, CancellationToken cancellation = default)
    {
        if (string.IsNullOrWhiteSpace(ticker))
        {
            throw new ArgumentException("Ticker cannot be null or empty", nameof(ticker));
        }

        _logger.LogInformation("Retrieving stock data for ticker: {Ticker}", ticker);

        var stock = _stocks.Value.FirstOrDefault(s =>
            string.Equals(s.Ticker, ticker, StringComparison.OrdinalIgnoreCase));

        if (stock == null)
        {
            _logger.LogWarning("Stock ticker '{Ticker}' not found", ticker);
            throw new TickerNotFoundException(ticker);
        }

        return await Task.FromResult(stock);
    }

    /// <summary>
    /// Gets all stocks data
    /// </summary>
    public async Task<IEnumerable<Stock>> GetAllStocksAsync(CancellationToken cancellation = default)
    {
        _logger.LogInformation("Retrieving all stocks data");

        return await Task.FromResult(_stocks.Value.AsEnumerable());
    }

    /// <summary>
    /// Loads stock data from JSON file
    /// </summary>
    private List<Stock> LoadStocksData()
    {
        var filePath = _options.FilePath;
        if (string.IsNullOrEmpty(filePath))
        {
            throw new InvalidOperationException("File path for stock data is not configured");
        }
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Stock data file not found at: {filePath}");
        }

        try
        {
            _logger.LogInformation("Loading stock data from file: {FilePath}", _options.FilePath);

            var stockFileDataList = JsonSerializer.Deserialize<List<StockData>>(
                File.ReadAllText(filePath),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (stockFileDataList == null)
            {
                _logger.LogWarning("No stock data found in file");
                return new List<Stock>();
            }

            var stocks = stockFileDataList.Select(data => new Stock
            {
                Ticker = data.Ticker,
                Date = DateTime.Parse(data.Date),
                Open = data.Open,
                Close = data.Close,
                High = data.High,
                Low = data.Low,
                Volume = data.Volume
            }).ToList();

            _logger.LogInformation("Successfully loaded {Count} stock records", stocks.Count);
            return stocks;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException("Failed to parse stock data from JSON file", ex);
        }
        catch (Exception)
        {
            throw;
        }
    }
}