namespace Chef.interpreter.types;

public class Chip
{

    public bool ok;
    
    public readonly string Path;
    public string Name;
    
    public readonly string Raw;
    public readonly string Optimized;
    public List<string> Pieces = new List<string>();

    public Chip(string path)
    {
        this.Path = path;
        this.Raw = File.ReadAllText(path);
        this.Optimized = Run.Optimizer.Optimize(Raw);

        if (!TryGetName())
        {
            ok = false;
            return;
        }
        
        Debug.Log("Loaded chip: " + Name);
        
        ok = true;
    }

    public bool TryGetName()
    {
        string name = Run.Interpreter.GetChipName(this);
        //Check if the name matches the file base
        if (name == Path.Split('/').Last().Split('.').First())
        {
            this.Name = name;
            return true;
        }
        else
        {
            Debug.ProjectCompileError("Chip name does not match file name: " + Path);
            return false;
        }
    }
    
    public string GetRaw()
    {
        return File.ReadAllText(Path);
    }
}