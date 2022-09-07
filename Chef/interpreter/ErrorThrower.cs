namespace Chef.interpreter;

public class ErrorThrower
{
    public static void ThrowError(string message)
    {
        Console.Error.WriteLine("Error: " + message);
    }
}