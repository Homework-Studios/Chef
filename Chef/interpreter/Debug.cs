namespace Chef.interpreter;

public class Debug
{
    public static void Log(string message)
    {
        Console.WriteLine("Debug: " + message);
    }
    
    public static void Log(object[] message)
    {
        Console.WriteLine("Debug: " + String.Join(" ", message));
    }

    public static void CompileCommandError(string message)
    {
        Console.Error.WriteLine("CompileCommandError: " + message);
    }
    
    public static void ProjectCompileError(string message)
    {
        Console.Error.WriteLine("ProjectCompileError: " + message);
    }
    
    public static void SyntaxError(string message, string at)
    {
        Console.Error.WriteLine("SyntaxError: " + message + " at " + at);
    }
}