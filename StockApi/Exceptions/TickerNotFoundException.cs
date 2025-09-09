namespace StockApi.Exceptions;

public class TickerNotFoundException : NotFoundException
{
    public TickerNotFoundException(string ticker) : base("Ticker", ticker)
    {
    }
}