using Chef.interpreter.types;
using Microsoft.VisualBasic;

namespace Chef.interpreter;

public class ChipInterpreter
{
    public Chip LoadChip(string path)
    {
        Debug.Log("Loading chip from " + path);
        Chip chip = new Chip(path);

        //Log Raw and Optimized
        Debug.Log("Raw Chip:" + chip.Raw);
        Debug.Log("Optimized Chip: " + chip.Optimized);
        
        
        return chip;
    }
}