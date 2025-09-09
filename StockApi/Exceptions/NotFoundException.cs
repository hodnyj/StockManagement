namespace StockApi.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string name, object key) : base($"Entry \"{name}\" ({key}) was not found.")
    {
    }
}
