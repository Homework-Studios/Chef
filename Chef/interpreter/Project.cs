namespace Chef.interpreter;

public class Project
{

    public string path;

    public Project(string path)
    {
        this.path = path;

        if (!TryFindProjectFile())
        {
            Debug.ProjectCompileError("Could not find project file at \'" + path + "\'.");
        }
        
        Debug.Log("Running project at \'" + path + "\'.");
    }
    
    public bool TryFindProjectFile()
    {
        string[] files = Directory.GetFiles(path);
        if (files.Contains(path + "/project.chef.json"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}