using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Library.LMS.Models;

public class Course : INotifyPropertyChanged
{
    private int id;
    private string? name;
    private string? code;
    private string? description;

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