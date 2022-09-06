using System.Text.RegularExpressions;

namespace Chef.optimizer;

public class FileOptimizer
{
    public string Optimize(string fileContent)
    {
        //Remove all comments -- Hehe Rebixxuslostus
        //All possible comment syntaxes: /* */ or //
        fileContent = Regex.Replace(fileContent, @"/\*.*?\*/", "", RegexOptions.Singleline);
        fileContent = Regex.Replace(fileContent, @"//.*", "");
        //Remove all spaces and linebreaks
        fileContent = fileContent.Replace(Environment.NewLine, "");
        fileContent = fileContent.Replace(" ", "");
        return fileContent;
    }
}