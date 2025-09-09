using StockApi.Exceptions;
using StockApi.Models;

namespace StockApi.Repositories;

public static class StockRepositoryHelper
{
    public static Stock GetStock(string ticker, IEnumerable<Stock> stocks)
    {
        if (string.IsNullOrEmpty(ticker))
        {
            throw new ArgumentException("Ticker cannot be null or empty", nameof(ticker));
        }

        var stock = stocks.FirstOrDefault(s =>
            string.Equals(s.Ticker, ticker, StringComparison.OrdinalIgnoreCase));

        if (stock is null)
        {
            throw new TickerNotFoundException(ticker);
        }

        return stock;
    }

}
