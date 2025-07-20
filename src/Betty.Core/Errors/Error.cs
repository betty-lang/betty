namespace Betty.Core.Errors;

public class Error(string message, int line, int column)
{
    public string Message { get; } = message;
    public int Line { get; } = line;
    public int Column { get; } = column;

    public override string ToString()
    {
        return $"Error at {Line}:{Column}: {Message}";
    }
}
