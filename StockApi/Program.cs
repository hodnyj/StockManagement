using StockApi.Interfaces;
using StockApi.Middleware;
using StockApi.Options;
using StockApi.Repositories;
using StockApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<FileStockRepositoryOptions>(builder.Configuration.GetSection("Stocks:FileStocRepository"));

builder.Services.AddScoped<IStockRepository, FileStockRepository>();
builder.Services.Decorate<IStockRepository, CachedStockRepository>();
builder.Services.AddScoped<IStockService, StockService>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDistributedMemoryCache(); // in production could be redis
}
else
{
    // for example, if using Redis in production
    // builder.Services.AddRedis(); 
}
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Add global exception handler middleware
app.UseExceptionHandler(options => { });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

// need for integration tests
namespace StockApi
{
    public partial class Program { }
}
