using System.Collections.ObjectModel;
using Library.LMS.Models;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
[QueryProperty(nameof(CourseId), "courseId")]
public partial class StudentCourseDetailPage :
    ContentPage
{
    private readonly ObservableCollection<string>
        displayedModuleContent;

    private readonly ObservableCollection<string>
        displayedGrades;

    private Student? selectedStudent;
    private Course? selectedCourse;

    public string? StudentId { get; set; }

    public string? CourseId { get; set; }

    public StudentCourseDetailPage()
    {
        InitializeComponent();

        displayedModuleContent =
            new ObservableCollection<string>();

        displayedGrades =
            new ObservableCollection<string>();

        ModuleContentCollectionView.ItemsSource =
            displayedModuleContent;

        GradesCollectionView.ItemsSource =
            displayedGrades;
    }

    private void StudentCourseDetailPageNavigatedTo(
        object? sender,
        NavigatedToEventArgs e)
    {
        if (
            !int.TryParse(
                StudentId,
                out int selectedStudentId
            )
            ||
            !int.TryParse(
                CourseId,
                out int selectedCourseId
            )
        )
        {
            DisplayMissingInformation();

            return;
        }

        selectedStudent =
            StudentServiceProxy.Current.GetById(
                selectedStudentId
            );

        selectedCourse =
            CourseServiceProxy.Current.GetById(
                selectedCourseId
            );

        if (
            selectedStudent == null
            ||
            selectedCourse == null
        )
        {
            DisplayMissingInformation();

            return;
        }

        bool isEnrolled =
            selectedCourse.Roster.Any(
                student =>
                    student.Id ==
                    selectedStudent.Id
            );

        if (!isEnrolled)
        {
            DisplayMissingInformation();

            return;
        }

        DisplayCourseInformation();
        RefreshAssignments();
        RefreshModuleContent();
        RefreshGrades();
    }

    private void DisplayCourseInformation()
    {
        if (
            selectedStudent == null
            ||
            selectedCourse == null
        )
        {
            return;
        }

        StudentNameLabel.Text =
            $"Student: {selectedStudent.Name}";

        CourseNameLabel.Text =
            selectedCourse.Name;

        CourseCodeLabel.Text =
            $"Course Code: {selectedCourse.Code}";

        SemesterLabel.Text =
            $"Semester: {selectedCourse.Semester}";

        SectionLabel.Text =
            $"Section: {selectedCourse.Section}";

        DescriptionLabel.Text =
            selectedCourse.Description;
    }

    private void RefreshAssignments()
    {
        if (selectedCourse == null)
        {
            AssignmentsCollectionView.ItemsSource =
                null;

            return;
        }

        AssignmentsCollectionView.ItemsSource =
            selectedCourse.Assignments
                .OrderBy(
                    assignment =>
                        assignment.DueDate
                )
                .ToList();
    }

    private void RefreshModuleContent()
    {
        displayedModuleContent.Clear();

        if (selectedCourse == null)
        {
            return;
        }

        int moduleNumber = 1;

        foreach (
            Module module
            in selectedCourse.Modules)
        {
            displayedModuleContent.Add(
                $"Module {moduleNumber}"
            );

            foreach (
                ModuleItem item
                in module.Content)
            {
                displayedModuleContent.Add(
                    $"   {item.DisplayText}"
                );
            }

            moduleNumber++;
        }
    }

    private void RefreshGrades()
    {
        displayedGrades.Clear();

        if (
            selectedStudent == null
            ||
            selectedCourse == null
        )
        {
            return;
        }

        foreach (
            Assignment assignment
            in selectedCourse.Assignments)
        {
            Submission? latestGradedSubmission =
                assignment.Submissions
                    .Where(
                        submission =>
                            submission.StudentId ==
                                selectedStudent.Id
                            &&
                            submission.Grade.HasValue
                    )
                    .OrderByDescending(
                        submission =>
                            submission.SubmissionDate
                    )
                    .FirstOrDefault();

            if (latestGradedSubmission == null)
            {
                continue;
            }

            double percentage =
                assignment.AvailablePoints > 0
                    ? latestGradedSubmission.Grade!.Value
                        / assignment.AvailablePoints
                        * 100
                    : 0;

            displayedGrades.Add(
                $"{assignment.Name}: " +
                $"{latestGradedSubmission.Grade:0.##}" +
                $"/{assignment.AvailablePoints} " +
                $"({percentage:0.##}%)"
            );
        }
    }

    private void DisplayMissingInformation()
    {
        StudentNameLabel.Text =
            "Student or course not found";

        CourseNameLabel.Text = string.Empty;
        CourseCodeLabel.Text = string.Empty;
        SemesterLabel.Text = string.Empty;
        SectionLabel.Text = string.Empty;
        DescriptionLabel.Text = string.Empty;

        AssignmentsCollectionView.ItemsSource =
            null;

        displayedModuleContent.Clear();
        displayedGrades.Clear();
    }

    private async void ReturnToCourseListClicked(
        object? sender,
        EventArgs e)
    {
        if (
            selectedStudent == null
            ||
            string.IsNullOrWhiteSpace(StudentId)
        )
        {
            await Shell.Current.GoToAsync(
                "//StudentMenuPage"
            );

            return;
        }

        await Shell.Current.GoToAsync(
            $"//StudentCoursePage" +
            $"?studentId={selectedStudent.Id}"
        );
    }
}