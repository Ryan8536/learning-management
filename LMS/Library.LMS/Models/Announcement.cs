namespace Library.LMS.Models;

public class Announcement
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Message { get; set; }

    public DateTime PostedDate { get; set; }

    public string DisplayText
    {
        get
        {
            return $"{Title} - {PostedDate:MM/dd/yyyy}";
        }
    }
}