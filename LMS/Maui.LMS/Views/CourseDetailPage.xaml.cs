using Library.LMS.Models;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class CourseDetailPage : ContentPage
{
    public int CourseId { get; set; }

    public CourseDetailPage()
    {
        InitializeComponent();
    }

    private void CourseDetailPageNavigatedTo(
        object? sender,
        NavigatedToEventArgs e)
    {
        if (CourseId == 0)
        {
            NameEntry.Text = "";
            CodeEntry.Text = "";
            DescriptionEditor.Text = "";
        }
        else
        {
            Course? course =
                CourseServiceProxy.Current.GetById(CourseId);

            if (course != null)
            {
                NameEntry.Text = course.Name;
                CodeEntry.Text = course.Code;
                DescriptionEditor.Text =
                    course.Description;
            }
        }
    }

    private async void SaveCourseClicked(
        object? sender,
        EventArgs e)
    {
        if (CourseId == 0)
        {
            Course newCourse = new Course
            {
                Name = NameEntry.Text,
                Code = CodeEntry.Text,
                Description = DescriptionEditor.Text
            };

            CourseServiceProxy.Current.Add(newCourse);
        }
        else
        {
            CourseServiceProxy.Current.UpdateDescription(
                CourseId,
                DescriptionEditor.Text
            );
        }

        CourseId = 0;

        await Shell.Current.GoToAsync(
            "//TeacherMenuPage"
        );
    }

    private async void CancelClicked(
        object? sender,
        EventArgs e)
    {
        CourseId = 0;

        await Shell.Current.GoToAsync(
            "//TeacherMenuPage"
        );
    }
}