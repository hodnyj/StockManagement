using StockApi.Models;

namespace StockApi.Interfaces;

public interface IStockService
{
    Task<IEnumerable<string>> GetAllTickersAsync(CancellationToken cancellation = default);

    Task<Stock> GetTickerDetailsAsync(string ticker, CancellationToken cancellation = default);

    Task<BuyingOption> GetBuyingOptionAsync(string ticker, decimal budget, CancellationToken cancellation = default);
}
