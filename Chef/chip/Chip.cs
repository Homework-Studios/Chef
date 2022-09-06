using Chef.chip.defaults;

namespace Chef.chip;

public class Chip
{

    public string Name;
    public string Path;
    public int InputCount;
    
    public Chip(string name, string path, int inputCount)
    {
        Name = name;
        Path = path;
        InputCount = inputCount;
    }

    public bool[]? Run(bool[] inputs)
    {
        switch (Name)
        {
            case "and":
                return And.Run(inputs);
            case "not":
                return Not.Run(inputs);
            case "out":
                return Out.Run(inputs);
            default:
                return Chef.Run.RunChip(Path, inputs);
        }
    }
    
    public override string ToString()
    {
        return Name;
    }
}