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

    public string GetChipName(Chip chip)
    {
        //We need the name of the chip to be the name of the file
        //We search after something that looks like this: "chip:chipname"
        string optimizedSrc = chip.Optimized;
        //Search for the first occurence of "chip:"
        int chipIndex = optimizedSrc.IndexOf("chip:", StringComparison.Ordinal);
        //If we found it
        if (chipIndex != -1)
        {
            //Get the rest of the string until the next (
            string chipName = optimizedSrc.Substring(chipIndex + 5,
                optimizedSrc.IndexOf("(", chipIndex, StringComparison.Ordinal) - chipIndex - 5);
            Debug.Log("Found chip name: " + chipName);
            //check if the name is found and return it if not throw a error
            return chipName;
        }
        else
        {
            Debug.SyntaxError("Chip name not found", "-1");
        }
        Debug.SyntaxError("Chip name not found", "-1");
        return "";
    }
}