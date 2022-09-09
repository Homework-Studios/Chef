using Chef.interpreter.libarys;
using Chef.interpreter.types;
using Microsoft.VisualBasic;

namespace Chef.interpreter;

public class ChipInterpreter
{
    public Chip LoadChip(string path, bool[] inputs)
    {
        Debug.Log("Loading chip from " + path);
        Chip chip = new Chip(path, inputs);
        
        
        
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

    public string[] GetChipInputs(Chip chip)
    {
        //the inputnames are directly after the chipname inside the ()
        //we search for the first ( and the next )
        string optimizedSrc = chip.Optimized;
        int chipIndex = optimizedSrc.IndexOf("chip:", StringComparison.Ordinal);
        int firstBracket = optimizedSrc.IndexOf("(", chipIndex, StringComparison.Ordinal);
        int secondBracket = optimizedSrc.IndexOf(")", chipIndex, StringComparison.Ordinal);
        //if we found them
        if (chipIndex != -1 && firstBracket != -1 && secondBracket != -1)
        {
            //get the string between the brackets
            string inputString = optimizedSrc.Substring(firstBracket + 1, secondBracket - firstBracket - 1);
            //split the string at the commas
            string[] inputs = inputString.Split(',');
            //check if we found any inputs and return them if not throw a error
            if (inputs.Length > 0)
            {
                return inputs;
            }
            else
            {
                Debug.SyntaxError("No inputs found", "-1");
            }
        }
        else
        {
            Debug.SyntaxError("No inputs found", "-1");
        }
        return new string[0];
    }

    public List<string> ChipBodyAsPieces(Chip chip)
    {
        //the body of the chip is everything after the first { and before the last } 
        //Every piece ends with a ;
        string optimizedSrc = chip.Optimized;
        int firstBracket = optimizedSrc.IndexOf("{", StringComparison.Ordinal);
        int lastBracket = optimizedSrc.LastIndexOf("}", StringComparison.Ordinal);
        //if we found them
        if (firstBracket != -1 && lastBracket != -1)
        {
            //get the string between the brackets
            string bodyString = optimizedSrc.Substring(firstBracket + 1, lastBracket - firstBracket - 1);
            //split the string at the ;
            List<string> body = bodyString.Split(';').ToList();
            //check if we found any inputs and return them if not throw a error
            if (body.Count > 0)
            {
                //Remove all empty strings
                body.RemoveAll(string.IsNullOrEmpty);
                return body;
            }
            else
            {
                Debug.SyntaxError("No body found", "-1");
            }
        }
        else
        {
            Debug.SyntaxError("No body found", "-1");
        }
        return new List<string>();
    }

    public bool IsLibAccess(string misterx)
    {
        switch (misterx)
        {
            case "base":
                return true;
            default:
                return false;
        }
    }

    public string GetFunctionName(string functionWithArgs)
    {
        //The functionname is everything before the first (
        int firstBracket = functionWithArgs.IndexOf("(", StringComparison.Ordinal);
        //if we found it
        if (firstBracket != -1)
        {
            //get the string before the bracket
            string functionName = functionWithArgs.Substring(0, firstBracket);
            //check if we found any inputs and return them if not throw a error
            if (functionName.Length > 0)
            {
                return functionName;
            }
            else
            {
                Debug.SyntaxError("No function name found", "-1");
            }
        }
        else
        {
            Debug.SyntaxError("No function name found", "-1");
        }
        return "";
    }

    public string[] InputSplit(string inputString)
    {
        //could look like this: base:and(input1,input2),input2
        //should be split into: base:and(input1,input2) input2
        //split the string at the commas but count the brackets to not split inside a function
        List<string> inputs = new List<string>();
        string currentInput = "";
        int bracketCount = 0;
        for (int i = 0; i < inputString.Length; i++)
        {
            if (inputString[i] == '(')
            {
                bracketCount++;
            }
            else if (inputString[i] == ')')
            {
                bracketCount--;
            }
            else if (inputString[i] == ',' && bracketCount == 0)
            {
                inputs.Add(currentInput);
                currentInput = "";
                continue;
            }
            currentInput += inputString[i];
        }
        inputs.Add(currentInput);
        return inputs.ToArray();
    }
    
    public bool[] FunctionToInputArray(string function, Chip chip)
    {
        //function: functionName(input1,input2,input3)
        //or function: functionName(libAccess:functionName(input1,input2),input2,input3)
        //we need to get the inputs and put them in an array 
        //and check if the input is a varible, if not check if its eather 1 or 0 / true or false / on or off
        string functionName = function.Substring(0, function.IndexOf("(", StringComparison.Ordinal));
        //Remove the functionname and the (
        string inputString = function.Substring(functionName.Length + 1);
        //Remove the ) from the end
        inputString = inputString.Substring(0, inputString.Length - 1);
        string[] inputs = InputSplit(inputString);
        bool[] inputArray = new bool[inputs.Length];
        foreach (string inputName in inputs)
        {
            Debug.Log("Input: " + inputName);
            //check if the input is a libAccess
            if (IsFunction(inputName))
            {
                Debug.Log("IsFunction");
                string[] bodySplit = inputName.Split(':');
                string libAccess = bodySplit[0];
                //merge the rest togerther
                string functionBody = string.Join(":", bodySplit.Skip(1).ToArray());
                bool[] input = RunLibaryFunction(libAccess, GetFunctionName(functionBody), FunctionToInputArray(functionBody, chip), chip);
                //add all the inputs to the inputArray
                inputArray = inputArray.Concat(input).ToArray();
            }
            //check if the input is a variable
            if (chip.IsRegisteredVariable(inputName))
            {
                Debug.Log("IsVariable");
                bool[] input = chip.GetVariable(inputName);
                //add all the inputs to the inputArray
                inputArray = inputArray.Concat(input).ToArray();
            }
        }
        return inputArray;
    }
    
    public bool[] RunLibaryFunction(string lib, string function, bool[] inputs, Chip chip)
    {
        switch (lib)
        {
            case "base":
                return BaseLibary.RunFunction(function, inputs, chip);
            default:
                return new bool[0];
        }
    }
    

    public bool IsFunction(string misterx)
    {
        string[] bodySplit = misterx.Split(':');
        string libAccess = bodySplit[0];
        if (IsLibAccess(libAccess))
        {
            return true;
        }
        return false;
    }
    
    public bool[] RunActionBody(string body, Chip chip)
    {
        //firstly check if the body is a registered variable
        if (chip.IsRegisteredVariable(body))
        {
            return chip.GetVariable(body);
        }
        //check if the pass contains a : if it does it calls a chip
        //it can be a base chip or a custom chip 
        //Example: base:and(input1,input2)
        //Example: custom:and(input1,input2)
        //But customs arent implemented yet
        string[] bodySplit = body.Split(':');
        string libAccess = bodySplit[0];
        //merge the rest of the string back together
        string function = string.Join(":", bodySplit.Skip(1).ToArray());
        if (IsLibAccess(libAccess))
        {
            return RunLibaryFunction(libAccess, GetFunctionName(function), FunctionToInputArray(function, chip), chip);
        }
        else
        {
            Debug.SyntaxError("Libary not found", "-1");
        }
        return new bool[0];
    }

    public void CReturn(bool[] output, Chip chip)
    {
        chip.output = output;
        chip.finished = true;
    }
    
    public bool InterpertPiece(string piece, Chip chip)
    {
        //split the string at the first :
        string[] pieceSplit = piece.Split(':');
        string actionType = pieceSplit[0];
        //Merge the rest of the string back together
        string actionBody = string.Join(":", pieceSplit.Skip(1).ToArray());
        Debug.Log("Action type: " + actionType);

        switch (actionType)
        {
            case "var":
                //split the string at the first =
                string[] actionSplit = actionBody.Split('=');
                string varName = actionSplit[0];
                string varValue = actionSplit[1];
                Debug.Log("Var name: " + varName);
                Debug.Log("Var value: " + varValue);
                chip.DefineVariable(varName, RunActionBody(varValue, chip));
                break;
            case "return":
                CReturn(RunActionBody(actionBody, chip), chip);
                chip.PrintResultInDebug();
                break;
            default:
                Debug.SyntaxError("Unknown action type", piece);
                break;
        }
        
        return true;
    }
}