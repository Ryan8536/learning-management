namespace Library.LMS.Models;

public class Semester
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string DisplayText
    {
        get
        {
            return
                $"{Name}: " +
                $"{StartDate:MM/dd/yyyy} - " +
                $"{EndDate:MM/dd/yyyy}";
        }
    }
}