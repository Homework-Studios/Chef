using Chef.commands;
using Chef.interpreter;
using Chef.optimizer;

namespace Chef;

public class Run
{
    
    public static CommandRegist CommandRegist;
    
    public static FileOptimizer Optimizer = new FileOptimizer();
    public static ChipInterpreter Interpreter = new ChipInterpreter();

    public static void Startup(string[] arguments)
    {
        Debug.Log("Chef is starting up...");
        CommandRegist = new CommandRegist();
        Debug.Log("Commands Registered, accepting the command...");
        CommandRegist.Run(arguments);
    }
    
    public static void CompileProject(string pathToProject)
    {
        Debug.Log("Compiling project: " + pathToProject);
    }
}