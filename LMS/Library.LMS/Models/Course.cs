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
    private string? section;

    private double minimumAPercentage = 90;
    private double minimumBPercentage = 80;
    private double minimumCPercentage = 70;
    private double minimumDPercentage = 60;

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

    public string? Section
    {
        get
        {
            return section;
        }

        set
        {
            section = value;
            NotifyPropertyChanged();
        }
    }

    public double MinimumAPercentage
    {
        get
        {
            return minimumAPercentage;
        }

        set
        {
            minimumAPercentage = value;
            NotifyPropertyChanged();
        }
    }

    public double MinimumBPercentage
    {
        get
        {
            return minimumBPercentage;
        }

        set
        {
            minimumBPercentage = value;
            NotifyPropertyChanged();
        }
    }

    public double MinimumCPercentage
    {
        get
        {
            return minimumCPercentage;
        }

        set
        {
            minimumCPercentage = value;
            NotifyPropertyChanged();
        }
    }

    public double MinimumDPercentage
    {
        get
        {
            return minimumDPercentage;
        }

        set
        {
            minimumDPercentage = value;
            NotifyPropertyChanged();
        }
    }

    public List<Student> Roster { get; set; }

    public List<Module> Modules { get; set; }

    public List<Assignment> Assignments { get; set; }

    public List<AssignmentGroup> AssignmentGroups { get; set; }

    public List<Announcement> Announcements { get; set; }

    public Course()
    {
        Roster = new List<Student>();
        Modules = new List<Module>();
        Assignments = new List<Assignment>();
        AssignmentGroups = new List<AssignmentGroup>();
        Announcements = new List<Announcement>();
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