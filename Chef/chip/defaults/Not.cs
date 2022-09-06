namespace Chef.chip.defaults;

public class Not
{ 
    public static bool[] Run(bool[] input)
    {
        return new bool[] { !input[0] };
    }
}