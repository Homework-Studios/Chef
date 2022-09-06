//Chef-legacy Interpreter 

//Get the filepath from the start arguments

using Chef;
using Chef.interpreter;
using Chef.optimizer;

class Program
{
    static void Main(string[] args)
    {
        //The File to Run
        string path = args[0];

        //the next arguments are only binary arguments (1 0)
        string[] binaryArgs = new string[args.Length - 1];
        Array.Copy(args, 1, binaryArgs, 0, args.Length - 1);
        //convert the binary arguments to bool
        bool[] inputs = new bool[binaryArgs.Length];
        for (int i = 0; i < binaryArgs.Length; i++)
        {
            inputs[i] = binaryArgs[i] == "1";
        }

        bool[]? output = Run.RunChip(path, inputs);
        
        if(output == null)
        {
            ErrorThrower.ThrowError("The Chip did not return a value");
            return;
        }

        Console.WriteLine(String.Join(" ", output));
    }
}