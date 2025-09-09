using StockApi.Models;

namespace StockApi.Interfaces;

public interface IStockRepository
{
    Task<IEnumerable<string>> GetAllTickersAsync(CancellationToken cancellationToken = default);

    Task<Stock> GetStockAsync(string ticker, CancellationToken cancellationToken = default);

    Task<IEnumerable<Stock>> GetAllStocksAsync(CancellationToken cancellationToken = default);
}