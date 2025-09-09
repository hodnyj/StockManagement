using System.Text.Json;
using Xunit.Abstractions;

namespace StockApi.Tests.IntegrationTests;

public class StockControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly HttpClient _client;

    public StockControllerIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        factory.TestOutputHelper = _testOutputHelper;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllTickers_ShouldReturnListOfTickers()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/stock/tickers");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Assert
        Assert.Equal(JsonValueKind.Array, root.ValueKind);
        var tickers = root.EnumerateArray().Select(e => e.GetString()).ToList();

        Assert.Contains("AAPL", tickers);
        Assert.Contains("MSFT", tickers);
        Assert.Contains("GOOGL", tickers);
        Assert.Equal(3, tickers.Count);
    }

    [Fact]
    public async Task GetTickerDetails_ShouldReturnCorrectDetailsForAAPL()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/stock/AAPL");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Assert
        Assert.Equal("AAPL", root.GetProperty("ticker").GetString());
        Assert.Equal(198.15m, root.GetProperty("open").GetDecimal());
        Assert.Equal(202.30m, root.GetProperty("close").GetDecimal());
    }

    [Fact]
    public async Task GetBuyingOption_ShouldReturnNumberOfSharesForBudget()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/stock/AAPL/buy?budget=1000");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Assert
        Assert.Equal("AAPL", root.GetProperty("ticker").GetString());
        Assert.Equal(1000, root.GetProperty("budget").GetDecimal());
        Assert.Equal(4, root.GetProperty("shares").GetDecimal());
    }

    [Fact]
    public async Task GetTickerDetails_InvalidTicker_ShouldReturnNotFound()
    {
        // Arrange & Act
        var response = await _client.GetAsync("/api/stock/INVALID");
        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
