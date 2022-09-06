namespace Chef.interpreter;

public class ChipInterpreter
{
    
    //The File may look like: baseChip(input1,input2){and(input1,input2)}
    //What means: chipName(args){otherChip(args)}

    private string ChipName;
    private bool[] Inputs;
    private string[] InputNames;

    private string[] BasicChips = new[] {"and", "not"};
    private List<string> KnownChips = new List<string>();
    //A dictionary of all known custom chips and their path to the file
    private Dictionary<string, string> CustomChips = new Dictionary<string, string>();
    //Dictory of all known custom chips and their required inputs
    private Dictionary<string, int> CustomChipInputs = new Dictionary<string, int>();

    public bool[] Interpret(string optimizedFileContent, bool[] inputs)
    {
        ChipName = optimizedFileContent.Split('(')[0];
        KnownChips.Add(ChipName);

        optimizedFileContent = ImportAll(optimizedFileContent);
        
        Console.WriteLine(optimizedFileContent);
        
        Inputs = inputs;
        InputNames = optimizedFileContent.Split('(')[1].Split(')')[0].Split(',');
        
        string[] chipsWithArgs = optimizedFileContent.Split('{')[1].Split('}')[0].Split(';');

        foreach (string chipWithArgs in chipsWithArgs)
        {
            string chipName = GetOtherChipName(chipWithArgs);

            if (!IsAChip(chipName))
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

        return new bool[0];
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
                CustomChips.Add(name, path);
                int requiredInputs = Run.RequiredInputCount(path);
                CustomChipInputs.Add(name, requiredInputs);
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
        string reversed = new string(first.ToCharArray().Reverse().ToArray());
        string second = reversed.Substring(1);
        string args = new string(second.ToCharArray().Reverse().ToArray());
        return args.Split(',');
    }
    
    public int GetOtherChipInputCount(string chipName)
    {
        switch (chipName)
        {
            case "and":
                return 2;
            case "not":
                return 1;
            default:
                if (CustomChipInputs.ContainsKey(chipName))
                {
                    return CustomChipInputs[chipName];
                }
                else
                {
                    ErrorThrower.ThrowError("Chip not found: " + chipName);
                    return 0;
                }
            
        }
    }
    
    public bool[] GetOtherChipInputs(string chipWithArgs)
    {
        string[] args = GetChipArgs(chipWithArgs);
        bool[] inputs = new bool[args.Length];
        List<bool> inputsList = new List<bool>();
        foreach (string arg in args)
        {
            if (IsAChip(arg))
            {
                if (IsCustomChip(arg))
                {
                    bool[] outs = Run.RunChip(GetPathForCustomChip(GetOtherChipName(arg)), GetOtherChipInputs(arg));
                    inputsList.AddRange(outs);
                }
                else
                {
                    bool[] outs = RunChip(GetOtherChipName(arg), GetOtherChipInputs(arg));
                    inputsList.AddRange(outs);
                }
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

    public bool[] RunChip(string chipName, bool[] inputs)
    {
        if (GetOtherChipInputCount(chipName) != inputs.Length)
        {
            ErrorThrower.ThrowError("Chip " + chipName + " needs " + GetOtherChipInputCount(chipName) + " inputs, but got " + inputs.Length);
            return new bool[0];
        }
        //Check if its a base chip
        bool[]? baseChipResult = RunBaseChip(chipName, inputs);
        if (baseChipResult != null)
            return baseChipResult;
        
        //Check if it is a custom chip and then run it
        if (IsCustomChip(chipName))
        {
            return Run.RunChip(GetPathForCustomChip(chipName), inputs);
        }
        
        ErrorThrower.ThrowError("Chip not found: " + chipName);
        return new bool[0];
    }

    public bool IsBaseChip(string chipName)
    {
        return BasicChips.Contains(chipName);
    }
    
    public bool IsCustomChip(string chipName)
    {
        return CustomChips.ContainsKey(chipName);
    }
    
    public string GetPathForCustomChip(string chipName)
    {
        return CustomChips[chipName];
    }
    
    public bool IsAChip(string chipName)
    {
        return KnownChips.Contains(chipName) || IsBaseChip(chipName) || IsCustomChip(chipName);
    }
    
    private bool[]? RunBaseChip(string chipName, bool[] inputs)
    {
        switch (chipName)
        {
            case "and":
                return new[] {inputs[0] && inputs[1]};
            case "not":
                return new[] {!inputs[0]};
            default:
                return null;
        }
    }

    public int RequiredInputCount(string optimized)
    {
        string[] chipsWithArgs = optimized.Split('{')[1].Split('}')[0].Split(';');
        foreach (string chipWithArgs in chipsWithArgs)
        {
            string chipName = GetOtherChipName(chipWithArgs);
            if (IsAChip(chipName))
            {
                return GetOtherChipInputCount(chipName);
            }
        }
        return 0;
    }
}