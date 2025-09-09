using Microsoft.Extensions.Logging;
using StockApi.Repositories;

namespace StockApi.Tests.Repositories;
public class FileStockRepositoryTests
{
    [Theory]
    [InlineData("", typeof(InvalidOperationException))]
    [InlineData(null, typeof(InvalidOperationException))]
    [InlineData("invalid.json", typeof(FileNotFoundException))]
    [InlineData("stocks-invalid.json", typeof(InvalidOperationException))]
    public async Task GetAllStocks_InvalidFile_ShouldThrowFileException(string fileName, Type expectedExceptionType)
    {
        // Arrange
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger<FileStockRepository>();
        var options = Microsoft.Extensions.Options.Options.Create(new StockApi.Options.FileStockRepositoryOptions
        {
            FilePath = fileName
        });
        var repository = new FileStockRepository(logger, options);

        // Act & Assert
        await Assert.ThrowsAsync(expectedExceptionType, async () => await repository.GetAllStocksAsync());

    }
}
