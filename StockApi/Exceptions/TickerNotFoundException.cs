namespace StockApi.Exceptions;

public class TickerNotFoundException : Exception
{
    public string Ticker { get; }

    public TickerNotFoundException(string ticker)
        : base($"Stock ticker '{ticker}' was not found.")
    {
        Ticker = ticker;
    }

    public TickerNotFoundException(string ticker, string message)
        : base(message)
    {
        Ticker = ticker;
    }

    public TickerNotFoundException(string ticker, string message, Exception innerException)
        : base(message, innerException)
    {
        Ticker = ticker;
    }
}