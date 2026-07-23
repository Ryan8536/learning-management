namespace Library.LMS.Models;

public class ModulePage : ModuleItem
{
    public string? Body { get; set; }

    public override string ItemType
    {
        get
        {
            return "Page";
        }
    }

    public override string DisplayText
    {
        get
        {
            return $"Page: {Name} - {Body}";
        }
    }
}