using Chef.chip;

namespace Chef.chip.defaults;

public class And
{
    public static bool[] Run(bool[] inputs)
    {
        return new []{inputs[0] && inputs[1]};
    }
}