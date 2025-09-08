using StockApi.Models;

namespace StockApi.Interfaces;

public interface IStockRepository
{
    Task<IEnumerable<string>> GetAllTickersAsync(CancellationToken cancellation = default);

    Task<Stock> GetStockAsync(string ticker, CancellationToken cancellation = default);

    Task<IEnumerable<Stock>> GetAllStocksAsync(CancellationToken cancellation = default);
}