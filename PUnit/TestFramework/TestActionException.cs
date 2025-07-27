namespace PUnit.TestFramework;

public class TestActionException : Exception
{
    internal TestActionException(Exception exception) : base($"{exception.GetType().Name}: {exception.Message}", exception.InnerException)
    {
        StackTrace = FilterStackTrace(exception.StackTrace);
    }

    internal TestActionException(string? message, Exception? innerException) : base(message, innerException)
    {
        StackTrace = FilterStackTrace(innerException?.StackTrace);
    }

    public override string StackTrace { get; }

    private static string FilterStackTrace(string? stackTrace)
    {
        if (string.IsNullOrEmpty(stackTrace))
        {
            return string.Empty;
        }

        var lines = stackTrace!.Split([Environment.NewLine], StringSplitOptions.None);

        return string.Join(Environment.NewLine,
            lines.TakeWhile(x => !x.Trim().StartsWith("at PUnit")));
    }
}