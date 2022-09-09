using Chef.interpreter;

namespace Chef.commands.builtIn;

public class BuildProject : Command
{
    public BuildProject() : base("bp", "Build a project at the given path or the current directory if no path is given.") {}

    public override void Execute(string[] args)
    {
        Debug.Log("Compiling project...");
        string path = args[0];
        Run.CompileProject(path);
    }

    public override bool Validate(string[] args)
    {
        //if lenght is 1 and if its a path its valid
        if (args.Length == 1 && IsPath(args[0]))
        {
            return true;
        }

        return false;
    }

    public bool IsPath(string path)
    {
        return File.Exists(path) || Directory.Exists(path);
    }
}