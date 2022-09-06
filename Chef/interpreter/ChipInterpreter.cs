using Chef.chip;
using Microsoft.VisualBasic;

namespace Chef.interpreter;

public class ChipInterpreter
{
    
    private string ChipName;
    private bool[] Inputs;
    private string[] InputNames;

    private string[] BasicChips = new[] {"and", "not"};
    private List<Chip> Chips = new List<Chip>();

    public bool[]? Interpret(string optimizedFileContent, bool[] inputs, string path)
    {
        ChipName = optimizedFileContent.Split('(')[0];
        Chips.Add(new (ChipName, path, RequiredInputCount(optimizedFileContent)));
        Chips.Add(new ("and", "none", 2));
        Chips.Add(new ("not", "none", 1));
        Chips.Add(new ("out", "none", -1));
        
        optimizedFileContent = ImportAll(optimizedFileContent);
        
        InputNames = optimizedFileContent.Split('(')[1].Split(')')[0].Split(',');
        //replace all the input names with the actual inputs (0 or 1)
        for (int i = 0; i < InputNames.Length; i++)
        {
            optimizedFileContent = optimizedFileContent.Replace(InputNames[i], inputs[i].ToString());
        }
        
        string[] chipsWithArgs = optimizedFileContent.Split('{')[1].Split('}')[0].Split(';');

        foreach (string chipWithArgs in chipsWithArgs)
        {
            string chipName = GetOtherChipName(chipWithArgs);

            if (!IsChip(chipName) && chipName.Length == 1)
            {
                bool? input = StrToBool(chipName);
                return new []{(bool) input};
            }
            
            bool[]? chipInputs = GetOtherChipInputs(chipWithArgs, InputNames);
            
            if(chipInputs == null) return null;

            return RunChip(chipName, chipInputs);
        }
        
        return null;
    }

    public bool StrToBool(string i)
    {
        return i == "1";
    }
    
    public string ImportAll(string content)
    {
        //the imports are at the start of the file
        //import:"path/to/file.chef";import:"path/to/file2.chef";...
        //the name would be file
        string[] imports = content.Split('{')[0].Split(';');
        foreach (string import in imports)
        {
            if (import.StartsWith("import:"))
            {
                string path = import.Split('"')[1];
                string name = path.Split('/')[path.Split('/').Length - 1].Split('.')[0];
                int requiredInputs = Run.RequiredInputCount(path);
                Chips.Add(new (name, path, requiredInputs));
            }
        }
        //Remove the imports from the content
        //split the string at the last ; and take the second part
        content = content.Split(';')[content.Split(';').Length - 1];
        return content;
    }

    public string GetOtherChipName(string chipWithArgs)
    {
        return chipWithArgs.Split('(')[0];
    }

    public string RemoveUntilBracket(string chipWithArgs)
    {
        foreach (char c in chipWithArgs)
        {
            if (c == '(' || c == ')')
            {
                chipWithArgs = chipWithArgs.Substring(1);
                break;
            }
            chipWithArgs = chipWithArgs.Substring(1);
        }
        return chipWithArgs;
    }
    
    //Worst thing that happend in my live so far
    public string[] GetChipArgs(string chipWithArgs)
    {
        string args = RemoveUntilBracket(chipWithArgs);
        args = new string(args.Reverse().ToArray());
        args = RemoveUntilBracket(args);
        args = new string(args.Reverse().ToArray());
        int count = 0;
        List<string> argsList = new List<string>();
        string currentArg = "";
        foreach (char c in args)
        {
            if (c == '(')
            {
                count++;
            }
            else if (c == ')')
            {
                count--;
            }
            else if (c == ',' && count == 0)
            {
                argsList.Add(currentArg);
                currentArg = "";
                continue;
            }

            currentArg += c;
        }
        argsList.Add(currentArg);
        return argsList.ToArray();
    }

    public int GetOtherChipInputCount(string chipName)
    {
        Chip? chip = GetChip(chipName);
        if (chip != null)
        {
            return chip.InputCount;
        }
        ErrorThrower.ThrowError("Chip not found: " + chipName);
        return 0;
    }
    
    public bool[]? GetOtherChipInputs(string chipWithArgs, string[] inputNames)
    {
        //chipWithArgs could look like: and(not(a),b)
        //Which Means: chip(chip(a),b)
        //So we need to get the args of the chip and then get the args of the args
        string[] chipArgs = GetChipArgs(chipWithArgs);
        bool[] chipInputs = new bool[chipArgs.Length];
        for (int i = 0; i < chipArgs.Length; i++)
        {
            string chipArg = chipArgs[i];
            string name = GetOtherChipName(chipArgs[i]);
            if (IsChip(name))
            {
                //chipArg is a chip
                //so we need to get the inputs of the chip
                bool[]? otherChipInputs = GetOtherChipInputs(chipArg, inputNames);
                if (otherChipInputs == null) return null;
                //run the chip but it can return more than one output
                bool[]? otherChipOutputs = RunChip(name, otherChipInputs);
                if (otherChipOutputs == null) return null;
                //we only need to add all the outputs to the inputs
                foreach (bool otherChipOutput in otherChipOutputs)
                {
                    chipInputs[i] = otherChipOutput;
                }
            }
            else
            {
                //chipArg is an input
                //so we need to get the value of the input
                bool input = StrToBool(name);
                chipInputs[i] = input;
            }
        }
        return chipInputs;
    }

    public bool[]? RunChip(string chipName, bool[] inputs)
    {
        Chip? chip = GetChip(chipName);
        if (chip == null)
        {
            ErrorThrower.ThrowError("Chip not found: " + chipName);
            return null;
        }

        if (chip.InputCount != inputs.Length && chip.InputCount != -1)
        {
            ErrorThrower.ThrowError("Chip " + chipName + " needs " + GetOtherChipInputCount(chipName) + " inputs, but got " + inputs.Length);
            return null;
        }

        return chip.Run(inputs);
    }
    
    public Chip? GetChip(string chipName)
    {
        foreach (Chip chip in Chips)
        {
            if (chip.Name == chipName)
            {
                return chip;
            }
        }
        return null;
    }

    public string? GetPathForCustomChip(string chipName)
    {
        Chip? chip = GetChip(chipName);
        if (chip == null) return null;
        return chip.Path;
    }
    
    public bool IsChip(string chipName)
    {
        return GetChip(chipName) != null;
    }

    public int RequiredInputCount(string optimized)
    {
        string[] split = optimized.Split('(');
        string[] split2 = split[1].Split(')');
        string[] split3 = split2[0].Split(',');
        return split3.Length;
    }
}