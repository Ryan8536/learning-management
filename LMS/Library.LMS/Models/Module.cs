namespace Library.LMS.Models;

public class Module
{
    public int Id { get; set; }

    public List<string> Content { get; set; }

    public Module()
    {
        Content = new List<string>();
    }
}