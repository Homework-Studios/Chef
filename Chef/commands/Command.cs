namespace Chef.commands;

public abstract class Command
{
    
    public string Name;
    public string Help;
    
    public Command(string name, string help)
    {
        this.Name = name;
        this.Help = help;
    }
    
    public abstract void Execute(string[] args);
    
    public abstract bool Validate(string[] args);
    
    public void PrintHelp()
    {
        Console.WriteLine(this.Help);
    }
    
    public string GetHelp()
    {
        return this.Help;
    }
    
    public string GetName()
    {
        return this.Name;
    }
}