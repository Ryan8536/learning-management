namespace Library.LMS.Models;

public class Assignment : ModuleItem
{
    public string? Description { get; set; }

    public int AvailablePoints { get; set; }

    public DateTime DueDate { get; set; }

    public List<Submission> Submissions { get; set; }

    public override string ItemType
    {
        get
        {
            return "Assignment";
        }
    }

    public override string DisplayText
    {
        get
        {
            return $"Assignment: {Name} - Due {DueDate:MM/dd/yyyy}";
        }
    }

    public Assignment()
    {
        Submissions = new List<Submission>();
    }
}