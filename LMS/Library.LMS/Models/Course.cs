using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Library.LMS.Models;

public class Course : INotifyPropertyChanged
{
    private int id;
    private string? name;
    private string? code;
    private string? description;
    private string? semester;

    public int Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
            NotifyPropertyChanged();
        }
    }

    public string? Name
    {
        get
        {
            return name;
        }

        set
        {
            name = value;
            NotifyPropertyChanged();
        }
    }

    public string? Code
    {
        get
        {
            return code;
        }

        set
        {
            code = value;
            NotifyPropertyChanged();
        }
    }

    public string? Description
    {
        get
        {
            return description;
        }

        set
        {
            description = value;
            NotifyPropertyChanged();
        }
    }

    public string? Semester
    {
        get
        {
            return semester;
        }

        set
        {
            semester = value;
            NotifyPropertyChanged();
        }
    }

    public List<Student> Roster { get; set; }

    public List<Module> Modules { get; set; }

    public List<Assignment> Assignments { get; set; }

    public List<AssignmentGroup> AssignmentGroups { get; set; }

    public Course()
    {
        Roster = new List<Student>();
        Modules = new List<Module>();
        Assignments = new List<Assignment>();
        AssignmentGroups = new List<AssignmentGroup>();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged(
        [CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName)
        );
    }
}