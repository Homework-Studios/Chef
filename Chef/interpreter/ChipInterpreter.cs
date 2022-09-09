using Chef.interpreter.types;
using Microsoft.VisualBasic;

namespace Chef.interpreter;

public class ChipInterpreter
{
    public Chip LoadChip(string path)
    {
        Debug.Log("Loading chip from " + path);
        Chip chip = new Chip(path);
        
        
        
        return chip;
    }
}