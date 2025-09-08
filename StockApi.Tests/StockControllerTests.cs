using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using StockApi.Controllers;
using StockApi.Models;
using StockApi.Options;
using StockApi.Repositories;
using StockApi.Services;

namespace StockApi.Tests
{
    public class StockControllerTests
    {
        private StockController _controller;
        public StockControllerTests()
        {
            _controller = new StockController(
                new StockService(
                    new FileStockRepository(
                        NullLogger<FileStockRepository>.Instance,
                        Microsoft.Extensions.Options.Options.Create(new FileStockRepositoryOptions
                        {
                            FilePath = "stocks.json"
                        })
                    ),
                    NullLogger<StockService>.Instance),
                NullLogger<StockController>.Instance);
        }

        [Fact]
        public async Task GetAllTickers_ShouldReturnListOfTickers()
        {
            // Arrange
            var controller = _controller;

            // Act

            var response = await controller.GetAllTickersAsync(); // changed ObjectResult -> see integration tests
            var result = response.Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var tickers = Assert.IsType<List<string>>(result.Value);
            Assert.Contains("AAPL", tickers);
            Assert.Contains("MSFT", tickers);
            Assert.Contains("GOOGL", tickers);
            Assert.Equal(3, tickers.Count);
        }

        [Fact]
        public async Task GetTickerDetails_ShouldReturnCorrectDetailsForAAPL()
        {
            // Arrange
            var controller = _controller;

            // Act

            // Direct dynamic property assertions are not possible with typed ActionResult<T>.
            // Use integration tests to inspect JSON property names and casing.
            var response = await controller.GetTickerDetailsAsync("AAPL");
            var result = response.Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var stock = Assert.IsType<Stock>(result?.Value);
            Assert.NotNull(stock);
            Assert.Equal("AAPL", stock.Ticker);
            Assert.Equal(198.15m, stock.Open);
            Assert.Equal(202.30m, stock.Close);
        }

        [Fact]
        public async Task GetBuyingOption_ShouldReturnNumberOfSharesForBudget()
        {
            // Arrange
            var controller = _controller;

            // Act

            // Direct dynamic property assertions are not possible with typed ActionResult<T>.
            // Use integration tests to inspect JSON property names and casing.
            var response = await controller.GetBuyingOptionAsync("AAPL", 1000);
            var result = response.Result as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var buyingOptions = Assert.IsType<BuyingOption>(result?.Value);
            Assert.NotNull(buyingOptions);
            Assert.Equal("AAPL", buyingOptions.Ticker);
            Assert.Equal(1000, buyingOptions.Budget);
            Assert.Equal(4, buyingOptions.Shares); // 1000 / 202.30 ≈ 4
        }
    }
}