using Chef.interpreter.types;
using Microsoft.VisualBasic;

namespace Chef.interpreter;

public class ChipInterpreter
{
    public static Chip LoadChip(string path)
    {
        Chip chip = new Chip(path);

        return chip;
    }
}