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

    public Course? SelectedCourse { get; set; }

    public void RefreshCourses()
    {
        Courses = new ObservableCollection<Course>(
            CourseServiceProxy.Current.Courses
        );
    }

    public void DeleteSelectedCourse()
    {
        if (SelectedCourse == null)
        {
            return;
        }

        CourseServiceProxy.Current.Delete(SelectedCourse.Id);
        SelectedCourse = null;
        RefreshCourses();
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