using Chef.chip;

namespace Chef.interpreter;

public class ChipInterpreter
{
    
    //The File may look like: baseChip(input1,input2){and(input1,input2)}
    //What means: chipName(args){otherChip(args)}

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
        
        optimizedFileContent = ImportAll(optimizedFileContent);

        Inputs = inputs;
        InputNames = optimizedFileContent.Split('(')[1].Split(')')[0].Split(',');
        
        string[] chipsWithArgs = optimizedFileContent.Split('{')[1].Split('}')[0].Split(';');

        foreach (string chipWithArgs in chipsWithArgs)
        {
            string chipName = GetOtherChipName(chipWithArgs);

            if (!IsChip(chipName))
            {
                bool? input = GetInput(chipName);
                if (input != null)
                {
                    return new []{(bool) input};
                }
            }
            
            bool[] chipInputs = GetOtherChipInputs(chipWithArgs);
            
            return RunChip(chipName, chipInputs);
        }
        
        return null;
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

    public bool? GetInput(string inputName)
    {
        bool reverse = inputName.StartsWith("!");
        inputName = inputName.Replace("!", "");
        
        if (!InputNames.Contains(inputName))
        {
            ErrorThrower.ThrowError("Input not found: " + inputName);
            return null;
        }
        
        for (int i = 0; i < InputNames.Length; i++)
        {
            if (InputNames[i] == inputName)
            {
                if(reverse) return !Inputs[i];
                
                return Inputs[i];
            }
        }
        
        return null;
    }

    public string GetOtherChipName(string chipWithArgs)
    {
        return chipWithArgs.Split('(')[0];
    }
    
    public string[] GetChipArgs(string chipWithArgs)
    {
        string chipName = GetOtherChipName(chipWithArgs);
        string first = chipWithArgs.Replace(chipName + "(", "");
        string second = first.Substring(0, first.Length - 1);
        return second.Split(',');
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
    
    public bool[] GetOtherChipInputs(string chipWithArgs)
    {
        string[] args = GetChipArgs(chipWithArgs);
        bool[] inputs = new bool[args.Length];
        List<bool> inputsList = new List<bool>();
        foreach (string arg in args)
        {
            if (IsChip(arg))
            {
                RunChip(arg, inputs);
            }
            else
            {
                bool? input = GetInput(arg);
                if(input != null) inputsList.Add((bool) input);
            }
        }
        if(inputsList.Count > 1) return inputsList.ToArray();
        return new[] {inputsList[0]};
    }

    public bool[]? RunChip(string chipName, bool[] inputs)
    {
        if (GetOtherChipInputCount(chipName) != inputs.Length)
        {
            ErrorThrower.ThrowError("Chip " + chipName + " needs " + GetOtherChipInputCount(chipName) + " inputs, but got " + inputs.Length);
            return null;
        }

        //Check if it is a custom chip and then run it
        if (IsChip(chipName))
        {
            Chip? chip = GetChip(chipName);
            if (chip != null)
            {
                return chip.Run(inputs);
            }
        }
        
        ErrorThrower.ThrowError("Chip not found: " + chipName);
        return null;
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
        string[] chipsWithArgs = optimized.Split('{')[1].Split('}')[0].Split(';');
        foreach (string chipWithArgs in chipsWithArgs)
        {
            string chipName = GetOtherChipName(chipWithArgs);
            if (IsChip(chipName))
            {
                return GetOtherChipInputCount(chipName);
            }
        }
        return 0;
    }
}