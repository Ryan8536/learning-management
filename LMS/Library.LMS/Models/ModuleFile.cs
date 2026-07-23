namespace Library.LMS.Models;

public class ModuleFile : ModuleItem
{
    public string? FilePath { get; set; }

    public override string ItemType
    {
        get
        {
            return "File";
        }
    }

    public override string DisplayText
    {
        get
        {
            return $"File: {Name} - {FilePath}";
        }
    }
}