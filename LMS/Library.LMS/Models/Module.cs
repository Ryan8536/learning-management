namespace Library.LMS.Models;

public class Module
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public List<ModuleItem> Content { get; set; }

    public string DisplayText
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return $"Module {Id}";
            }

            return $"Module {Id}: {Name}";
        }
    }

    public Module()
    {
        Content = new List<ModuleItem>();
    }
}