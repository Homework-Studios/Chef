using Chef.interpreter.types;

namespace Chef.interpreter.libarys;

public class BaseLibary
{
    public static bool[] RunFunction(string name, bool[] inputs, Chip chip)
    {
        Debug.Log("Running function " + name);
        try
        {
            switch (name)
            {
                case "and":
                    return new []{ inputs[0] && inputs[1] };
            }
        }catch(Exception e)
        {
            Debug.SyntaxError("Error: " + e.Message, "BaseLibary");
        }
        return new bool[0];
    }
}