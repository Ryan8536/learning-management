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
            SemesterEntry.Text = "";
            DescriptionEditor.Text = "";

            return;
        }

        Course? course =
            CourseServiceProxy.Current.GetById(CourseId);

        if (course == null)
        {
            return;
        }

        NameEntry.Text =
            course.Name;

        CodeEntry.Text =
            course.Code;

        SemesterEntry.Text =
            course.Semester;

        DescriptionEditor.Text =
            course.Description;
    }

    private async void SaveCourseClicked(
        object? sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(
            SemesterEntry.Text))
        {
            await DisplayAlertAsync(
                "Missing Semester",
                "Every course must be assigned to a semester.",
                "OK"
            );

            return;
        }

        if (CourseId == 0)
        {
            Course newCourse = new Course
            {
                Name = NameEntry.Text,
                Code = CodeEntry.Text,
                Semester = SemesterEntry.Text.Trim(),
                Description = DescriptionEditor.Text
            };

            CourseServiceProxy.Current.Add(newCourse);
        }
        else
        {
            Course? course =
                CourseServiceProxy.Current.GetById(CourseId);

            if (course != null)
            {
                course.Name =
                    NameEntry.Text;

                course.Code =
                    CodeEntry.Text;

                course.Semester =
                    SemesterEntry.Text.Trim();

                course.Description =
                    DescriptionEditor.Text;
            }
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