using Chef.interpreter.types;
using Chef.utils;
using Newtonsoft.Json.Linq;

namespace Chef.interpreter;

public class Project
{

    public readonly string Path;
    
    public string? Name;
    public string? Description;
    public string? Version;
    public string? Author;
    
    public string? StartPath;
    public string GlobalStartPath;
    public int[]? Arguments;

    public readonly string[] Properties = new[] {"name", "description", "version", "author", "start", "arguments"};
    
    public Chip StartChip;
    public List<Chip> ProjectChips = new List<Chip>();

    public Project(string path)
    {
        this.Path = path;

        if (!TryFindProjectFile())
        {
            Debug.ProjectCompileError("Could not find project file at \'" + path + "\'.");
            return;
        }
        
        Debug.Log("Running project at \'" + path + "\'.");
        
        TryReadProjectFile();
        
        Debug.Log("Project name: " + Name);
        Debug.Log("Project description: " + Description);
        Debug.Log("Project version: " + Version);
        Debug.Log("Project author: " + Author);
        Debug.Log("Starting project at \'" + StartPath + "\'.");
        if (Arguments != null) Debug.Log("Project arguments: " + String.Join(" ", Arguments));

        if (!DoesMainFileExist() && StartPath == null)
        {
            Debug.ProjectCompileError("Could not find main file at \'" + StartPath + "\'.");
            return;
        }

        GlobalStartPath = LocalPathToGlobalPath(StartPath);
        
        StartChip = Run.Interpreter.LoadChip(GlobalStartPath, VariableUtil.IntArrayToBoolArray(Arguments));
        ProjectChips.Add(StartChip);

        if (!StartChip.ok) return;

        Debug.Log("Project started.");
        Debug.Log("Running Start Chip...");
        
        StartChip.RunPieces();
    }
    
    public string LocalPathToGlobalPath(string path)
    {
        return Path + "/" + path;
    }
    
    public bool TryFindProjectFile()
    {
        string[] files = Directory.GetFiles(Path);
        if (files.Contains(Path + "/project.chef.json"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public void TryReadProjectFile()
    {
        Debug.Log("Reading project file...");
        string projectFileRaw = File.ReadAllText(Path + "/project.chef.json");
        //Use newtonsoft json to parse the project file
        JObject projectFile = JObject.Parse(projectFileRaw);
        //Loop through all the properties
        foreach (string property in Properties)
        {
            TrySetPropertie(projectFile, property);
        }
    }

    public void TrySetPropertie(JObject projectFile, string property)
    {
        //Try to get the property from the project file and then set it to the according variable
        try
        {
            switch (property)
            {
                case "name":
                    Name = projectFile.GetValue("name")!.ToString();
                    break;
                case "description":
                    Description = projectFile.GetValue("description")!.ToString();
                    break;
                case "version":
                    Version = projectFile.GetValue("version")!.ToString();
                    break;
                case "author":
                    Author = projectFile.GetValue("author")!.ToString();
                    break;
                case "start":
                    StartPath = projectFile.GetValue("start")!.ToString();
                    break;
                case "arguments":
                    Arguments = projectFile.GetValue("arguments")!.ToObject<int[]>();
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.ProjectCompileError("Could not find property \'" + property + "\' in project file.");
        }
    }
    
    public bool DoesMainFileExist()
    {
        if (File.Exists(Path + "/" + StartPath))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}