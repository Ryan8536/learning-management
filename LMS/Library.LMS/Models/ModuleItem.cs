namespace Library.LMS.Models;

public abstract class ModuleItem
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public abstract string ItemType { get; }

    public virtual string DisplayText
    {
        get
        {
            return $"{ItemType}: {Name}";
        }
    }
}