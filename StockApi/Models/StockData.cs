using System.Text.Json.Serialization;

namespace StockApi.Models;

public class StockData
{
    [JsonRequired]
    public string Ticker { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public decimal Open { get; set; }
    [JsonRequired]
    public decimal Close { get; set; }
    public decimal High { get; set; }
    public decimal Low { get; set; }
    public long Volume { get; set; }
}