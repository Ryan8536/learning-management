using Library.LMS.Models;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
public partial class StudentCoursePage : ContentPage
{
    private string? studentId;

    public string? StudentId
    {
        get
        {
            return studentId;
        }

        set
        {
            studentId = value;
        }
    }

    public StudentCoursePage()
    {
        InitializeComponent();
    }

    private void StudentCoursePageNavigatedTo(
        object? sender,
        NavigatedToEventArgs e)
    {
        if (
            !int.TryParse(
                StudentId,
                out int selectedStudentId
            )
        )
        {
            StudentNameLabel.Text =
                "Student not found";

            return;
        }

        Student? student =
            StudentServiceProxy.Current.GetById(
                selectedStudentId
            );

        if (student == null)
        {
            StudentNameLabel.Text =
                "Student not found";

            return;
        }

        StudentNameLabel.Text =
            $"Selected Student: {student.Name}";
    }
}