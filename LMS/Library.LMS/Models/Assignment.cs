namespace Library.LMS.Models;

public class Assignment : ModuleItem
{
    public string? Description { get; set; }

    public int AvailablePoints { get; set; }

    public DateTime DueDate { get; set; }

    public bool IsQuiz { get; set; }

    public string? QuizQuestion { get; set; }

    public List<Submission> Submissions { get; set; }

    public override string ItemType
    {
        get
        {
            return IsQuiz
                ? "Quiz"
                : "Assignment";
        }
    }

    public override string DisplayText
    {
        get
        {
            string itemName =
                IsQuiz
                    ? "Quiz"
                    : "Assignment";

            return
                $"{itemName}: {Name} - " +
                $"Due {DueDate:MM/dd/yyyy}";
        }
    }

    public Assignment()
    {
        Submissions =
            new List<Submission>();

        IsQuiz = false;
    }
}