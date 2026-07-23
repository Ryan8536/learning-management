namespace Library.LMS.Models;

public class Module
{
    public int Id { get; set; }

    public List<ModuleItem> Content { get; set; }

    public Module()
    {
        Content = new List<ModuleItem>();
    }
}