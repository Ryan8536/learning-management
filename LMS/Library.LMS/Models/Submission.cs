namespace Library.LMS.Models;

public class Submission
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int AssignmentId { get; set; }

    public string? Content { get; set; }

    public DateTime SubmissionDate { get; set; }

    public double? Grade { get; set; }

    public string? Feedback { get; set; }

    public string? AttachedFileName { get; set; }

    public string? AttachedFilePath { get; set; }

    public List<SubmissionComment> Comments
    {
        get;
        set;
    }

    public bool HasAttachedFile
    {
        get
        {
            return
                !string.IsNullOrWhiteSpace(
                    AttachedFileName
                )
                &&
                !string.IsNullOrWhiteSpace(
                    AttachedFilePath
                );
        }
    }

    public Submission()
    {
        Comments =
            new List<SubmissionComment>();
    }
}