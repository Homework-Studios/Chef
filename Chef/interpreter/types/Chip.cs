namespace Chef.interpreter.types;

public class Chip
{

    public bool ok;
    public bool finished;
    public bool[] output;
    
    public readonly string Path;
    public string Name;
    
    public readonly string Raw;
    public readonly string Optimized;
    public List<string> Pieces = new List<string>();
    public bool[] Inputs;
    public Dictionary<string, bool> InputsByName = new Dictionary<string, bool>();
    public Dictionary<string, bool[]> CustomVariables = new Dictionary<string, bool[]>();

    public Chip(string path, bool[] inputs)
    {
        this.Path = path;
        this.Raw = File.ReadAllText(path);
        this.Optimized = Run.Optimizer.Optimize(Raw);

        if (!TryGetName())
        {
            ok = false;
            return;
        }
        
        Inputs = inputs;
        
        if (!TryGetInputs())
        {
            ok = false;
            return;
        }
        
        if (!TryInterpretPieces())
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

    public bool TryGetInputs()
    {
        string[] inputs = Run.Interpreter.GetChipInputs(this);
        
        if(inputs.Length != Inputs.Length)
        {
            Debug.SyntaxError("Chip inputs do not match file inputs: " + Path, "chip: " + Name + "(");
            return false;
        }
        
        //We map the inputs to the inputs by name dictionary
        for (int i = 0; i < inputs.Length; i++)
        {
            InputsByName.Add(inputs[i], Inputs[i]);
            //Add them to the custom variables dictionary
            CustomVariables.Add(inputs[i], new bool[1] { Inputs[i] });
        }
        return true;
    }

    public bool TryInterpretPieces()
    {
        Pieces = Run.Interpreter.ChipBodyAsPieces(this);
        //if the pieces are empty, there was an error
        if (Pieces.Count == 0)
        {
            Debug.ProjectCompileError("Chip body is empty: " + Path);
            return false;
        }
        return true;
    }
    
    public void DefineVariable(string name, bool[] value)
    {
        CustomVariables.Add(name, value);
    }
    
    public bool IsRegisteredVariable(string name)
    {
        return CustomVariables.ContainsKey(name);
    }
    
    public bool[] GetVariable(string name)
    {
        return CustomVariables[name];
    }
    
    public void RunPieces()
    {
        foreach (string piece in Pieces)
        {
            Run.Interpreter.InterpertPiece(piece, this);
        }
    }

    public void PrintResultInDebug()
    {
        if (finished)
        {
            Debug.Log("Finished: " + Name);
            Debug.Log(output);
        }
    }
    
    public string GetRaw()
    {
        return File.ReadAllText(Path);
    }
}