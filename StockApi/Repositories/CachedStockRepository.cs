using Microsoft.Extensions.Caching.Distributed;
using StockApi.Interfaces;
using StockApi.Models;
using System.Text.Json;

namespace StockApi.Repositories;

public class CachedStockRepository(IStockRepository stockRepository, IDistributedCache cache) : IStockRepository
{
    private const string AllStocksCacheKey = "AllStocks";
    private const string AllTickersCacheKey = "AllTickers";
    public async Task<IEnumerable<Stock>> GetAllStocksAsync(CancellationToken cancellationToken = default)
    {
        var cachedStocks = await cache.GetStringAsync(AllStocksCacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedStocks))
        {
            return JsonSerializer.Deserialize<IEnumerable<Stock>>(cachedStocks)!;
        }
        var stocks = await stockRepository.GetAllStocksAsync(cancellationToken);
        await cache.SetStringAsync(AllStocksCacheKey, JsonSerializer.Serialize(stocks), token: cancellationToken);
        await cache.RemoveAsync(AllTickersCacheKey, cancellationToken); // remove tickers cache as data might have changed
        return stocks;
    }

    public async Task<IEnumerable<string>> GetAllTickersAsync(CancellationToken cancellationToken = default)
    {
        var cachedTickers = await cache.GetStringAsync(AllTickersCacheKey, cancellationToken);
        if (!string.IsNullOrEmpty(cachedTickers))
        {
            return JsonSerializer.Deserialize<IEnumerable<string>>(cachedTickers)!;
        }
        var tickers = await stockRepository.GetAllTickersAsync(cancellationToken);
        await cache.SetStringAsync(AllTickersCacheKey, JsonSerializer.Serialize(tickers), token: cancellationToken);
        return tickers;
    }

    public async Task<Stock> GetStockAsync(string ticker, CancellationToken cancellationToken = default)
    => StockRepositoryHelper.GetStock(ticker, await GetAllStocksAsync(cancellationToken)); // Reuse helper method for consistency
}
