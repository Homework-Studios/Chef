using Chef.interpreter;
using Chef.optimizer;

namespace Chef;

public class Run
{
    public static FileOptimizer optimizer = new FileOptimizer();
    public static ChipInterpreter interpreter = new ChipInterpreter();

    public static int RequiredInputCount(string path)
    {
        string[] lines = System.IO.File.ReadAllLines(path);
        string content = String.Join(Environment.NewLine, lines);
        string optimized = optimizer.Optimize(content);
        return interpreter.RequiredInputCount(optimized);
    }
    
    public static bool[]? RunChip(string path, bool[] inputs)
    {
        //Read the file
        string[] lines = System.IO.File.ReadAllLines(path);
        string content = String.Join(Environment.NewLine, lines);
        string optimized = optimizer.Optimize(content);
        return interpreter.Interpret(optimized, inputs, path);
    }
}