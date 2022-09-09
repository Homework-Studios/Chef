using Chef.commands.builtIn;
using Chef.interpreter;

namespace Chef.commands;

public class CommandRegist
{
    public List<Command?> Commands = new List<Command?>();

    public CommandRegist()
    {
        Debug.Log("Registering commands...");
        Command[] init = {
            new BuildProject()
        };
        Commands.AddRange(init);
        Debug.Log("Registered " + Commands.Count + " commands.");
    }

    public void Run(string[] arguments)
    {
        if(arguments.Length == 1)
        {
            Debug.CompileCommandError("No command specified. Try using '--help' for a list of commands.");
            return;
        }
        
        //remove the first argument, which is the command
        arguments = arguments.Skip(1).ToArray();
        
        //the first argument should look like this --commandname
        string commandName = arguments[0];

        if (!commandName.StartsWith("--"))
        {
            Debug.CompileCommandError("Invalid syntax. Try using '--help' for a list of commands.");
            return;
        }
        
        //remove the -- from the command name
        commandName = commandName.Substring(2);
        
        Debug.Log("Running command '" + commandName + "'...");
        //find the command
        Command? command = Commands.Find(x => x.Name == commandName);
        
        if(command == null)
        {
            Debug.CompileCommandError("Command '" + commandName + "' not found. Try using '--help' for a list of commands.");
            return;
        }
        
        //remove the command name from the arguments
        arguments = arguments.Skip(1).ToArray();
        
        Debug.Log("Running command '" + commandName + "' with " + arguments.Length + " arguments.");
        //validate the arguments
        if(!command.Validate(arguments))
        {
            Debug.CompileCommandError("Invalid arguments for command '" + commandName + "'. Try using '--help' for a list of commands.");
            return;
        }
        
        //run the command
        command.Execute(arguments);
    }
    
    public void AddCommand(Command? command)
    {
        Commands.Add(command);
    }
    
    public void RemoveCommand(Command? command)
    {
        Commands.Remove(command);
    }
    
    public void RemoveCommand(string commandName)
    {
        Commands.RemoveAll(x => x?.Name == commandName);
    }
    
    public Command? GetCommand(string commandName)
    {
        return Commands.Find(x => x?.Name == commandName);
    }
    
    public void ExecuteCommand(string commandName, string[] args)
    {
        Command? command = GetCommand(commandName);
        command?.Execute(args);
    }
}