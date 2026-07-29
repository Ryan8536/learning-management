namespace Library.LMS.Models;

public class SubmissionComment
{
    public int Id { get; set; }

    public int SubmissionId { get; set; }

    public int AuthorId { get; set; }

    public string? AuthorName { get; set; }

    public string? AuthorRole { get; set; }

    public string? Message { get; set; }

    public DateTime PostedDate { get; set; }

    public string DisplayText
    {
        get
        {
            return
                $"{AuthorName} ({AuthorRole}) - " +
                $"{PostedDate:g}\n" +
                $"{Message}";
        }
    }
}