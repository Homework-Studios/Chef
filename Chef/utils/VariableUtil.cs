namespace Chef.utils;

public class VariableUtil
{
    public static bool[] IntArrayToBoolArray(int[] intArray)
    {
        bool[] boolArray = new bool[intArray.Length];
        for (int i = 0; i < intArray.Length; i++)
        {
            boolArray[i] = intArray[i] == 1;
        }
        return boolArray;
    }
}