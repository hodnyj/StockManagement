namespace StockApi.Models;

public class StockData
{
    public string Ticker { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public decimal Open { get; set; }
    public decimal Close { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public long Volume { get; set; }
}