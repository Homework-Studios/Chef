namespace Chef.interpreter.types;

public class Chip
{
    
    public readonly string Path;
    public readonly string Name;
    
    public readonly string Raw;
    public readonly string Optimized;
    public List<string> Pieces = new List<string>();

    public Chip(string path)
    {
        this.Path = path;
        this.Raw = File.ReadAllText(path);
        this.Optimized = Run.Optimizer.Optimize(Raw);
    }

    public string GetRaw()
    {
        return File.ReadAllText(Path);
    }
}