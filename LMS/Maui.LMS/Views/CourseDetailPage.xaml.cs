using Library.LMS.Models;
using Library.LMS.Services;

namespace Maui.LMS.Views;

public partial class CourseDetailPage : ContentPage
{
    public CourseDetailPage()
    {
        InitializeComponent();
    }

    private void CourseDetailPageNavigatedTo(
        object? sender,
        NavigatedToEventArgs e)
    {
        BindingContext = new Course();
    }

    private async void SaveCourseClicked(
        object? sender,
        EventArgs e)
    {
        Course? course = BindingContext as Course;

        if (course != null)
        {
            CourseServiceProxy.Current.Add(course);
            await Shell.Current.GoToAsync("//TeacherMenuPage");
        }
    }

    private async void CancelClicked(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("//TeacherMenuPage");
    }
}