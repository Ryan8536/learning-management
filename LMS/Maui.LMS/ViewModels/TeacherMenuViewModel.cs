using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Library.LMS.Models;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels;

public class TeacherMenuViewModel : INotifyPropertyChanged
{
    private ObservableCollection<Course> courses = new();

    public ObservableCollection<Course> Courses
    {
        get
        {
            return courses;
        }

        set
        {
            courses = value;
            NotifyPropertyChanged();
        }
    }

    public void RefreshCourses()
    {
        Courses = new ObservableCollection<Course>(
            CourseServiceProxy.Current.Courses
        );
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