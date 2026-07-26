using System.Collections.ObjectModel;
using Library.LMS.Models;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
public partial class StudentCoursePage : ContentPage
{
    private string? studentId;
    private Student? selectedStudent;
    private Course? selectedCourse;

    private readonly ObservableCollection<Course>
        displayedCourses;

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

        displayedCourses =
            new ObservableCollection<Course>();

        CoursesCollectionView.ItemsSource =
            displayedCourses;
    }

    private void StudentCoursePageNavigatedTo(
        object? sender,
        NavigatedToEventArgs e)
    {
        selectedCourse = null;

        CoursesCollectionView.SelectedItem =
            null;

        if (
            !int.TryParse(
                StudentId,
                out int selectedStudentId
            )
        )
        {
            StudentNameLabel.Text =
                "Student not found";

            displayedCourses.Clear();

            return;
        }

        selectedStudent =
            StudentServiceProxy.Current.GetById(
                selectedStudentId
            );

        if (selectedStudent == null)
        {
            StudentNameLabel.Text =
                "Student not found";

            displayedCourses.Clear();

            return;
        }

        StudentNameLabel.Text =
            $"Selected Student: {selectedStudent.Name}";

        RefreshCourses();
    }

    private void RefreshCourses()
    {
        displayedCourses.Clear();

        if (selectedStudent == null)
        {
            return;
        }

        IEnumerable<Course> enrolledCourses =
            CourseServiceProxy.Current.Courses
                .Where(
                    course =>
                        course.Roster.Any(
                            student =>
                                student.Id ==
                                selectedStudent.Id
                        )
                )
                .OrderBy(course => course.Name);

        foreach (Course course in enrolledCourses)
        {
            displayedCourses.Add(course);
        }
    }

    private void CourseSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        selectedCourse =
            CoursesCollectionView.SelectedItem
            as Course;
    }

    private async void OpenCourseClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedStudent == null)
        {
            await DisplayAlertAsync(
                "Student Not Found",
                "Return to the student menu and select a student.",
                "OK"
            );

            return;
        }

        if (selectedCourse == null)
        {
            await DisplayAlertAsync(
                "No Course Selected",
                "Select a course before continuing.",
                "OK"
            );

            return;
        }

        await Shell.Current.GoToAsync(
            $"//StudentCourseDetailPage" +
            $"?studentId={selectedStudent.Id}" +
            $"&courseId={selectedCourse.Id}"
        );
    }

    private async void SelectDifferentStudentClicked(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            "//StudentMenuPage"
        );
    }
}