namespace Chef.optimizer;

public class FileOptimizer
{
    public string Optimize(string fileContent)
    {
        //Remove all spaces and linebreaks
        fileContent = fileContent.Replace(Environment.NewLine, "");
        fileContent = fileContent.Replace(" ", "");
        return fileContent;
    }
}