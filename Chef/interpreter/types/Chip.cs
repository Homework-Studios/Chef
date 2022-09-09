namespace Chef.interpreter.types;

public class Chip
{
    
    public readonly string Path;
    public readonly string Name;
    public List<string> Pieces = new List<string>();

    public Chip(string path)
    {
        this.Path = path;
    }

    public string GetRaw()
    {
        return File.ReadAllText(Path);
    }
}