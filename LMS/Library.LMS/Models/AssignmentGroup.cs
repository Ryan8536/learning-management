namespace Library.LMS.Models;

public class AssignmentGroup
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public double Weight { get; set; }

    public List<Assignment> Assignments { get; set; }

    public AssignmentGroup()
    {
        Weight = 0;

        Assignments =
            new List<Assignment>();
    }
}