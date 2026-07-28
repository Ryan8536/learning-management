using System.Collections.ObjectModel;
using Library.LMS.Models;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
[QueryProperty(nameof(CourseId), "courseId")]
public partial class StudentCourseDetailPage :
    ContentPage
{
    private readonly ObservableCollection<Announcement>
        displayedAnnouncements;

    private readonly ObservableCollection<string>
        displayedModuleContent;

    private readonly ObservableCollection<string>
        displayedGrades;

    private Student? selectedStudent;
    private Course? selectedCourse;
    private Assignment? selectedAssignment;

    public string? StudentId { get; set; }

    public string? CourseId { get; set; }

    public StudentCourseDetailPage()
    {
        InitializeComponent();

        displayedAnnouncements =
            new ObservableCollection<Announcement>();

        displayedModuleContent =
            new ObservableCollection<string>();

        displayedGrades =
            new ObservableCollection<string>();

        AnnouncementsCollectionView.ItemsSource =
            displayedAnnouncements;

        ModuleContentCollectionView.ItemsSource =
            displayedModuleContent;

        GradesCollectionView.ItemsSource =
            displayedGrades;
    }

    private void StudentCourseDetailPageNavigatedTo(
        object? sender,
        NavigatedToEventArgs e)
    {
        selectedAssignment = null;

        AssignmentsCollectionView.SelectedItem =
            null;

        ResponseEditor.Text =
            string.Empty;

        SelectedAssignmentLabel.Text =
            "No assignment selected";

        SubmissionStatusLabel.Text =
            string.Empty;

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
        RefreshAnnouncements();
        RefreshCourseGrade();
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

    private void RefreshAnnouncements()
    {
        displayedAnnouncements.Clear();

        if (selectedCourse == null)
        {
            return;
        }

        foreach (
            Announcement announcement
            in selectedCourse.Announcements
                .OrderByDescending(
                    announcement =>
                        announcement.PostedDate
                ))
        {
            displayedAnnouncements.Add(
                announcement
            );
        }
    }

    private void RefreshCourseGrade()
    {
        if (
            selectedStudent == null
            ||
            selectedCourse == null
        )
        {
            LetterGradeLabel.Text =
                "Not Available";

            CoursePercentageLabel.Text =
                string.Empty;

            return;
        }

        double? courseGrade =
            CourseServiceProxy.Current.CalculateCourseGrade(
                selectedCourse.Id,
                selectedStudent.Id
            );

        if (!courseGrade.HasValue)
        {
            LetterGradeLabel.Text =
                "Not Available";

            CoursePercentageLabel.Text =
                "No graded submissions";

            return;
        }

        double percentage =
            courseGrade.Value;

        LetterGradeLabel.Text =
            GetLetterGrade(percentage);

        CoursePercentageLabel.Text =
            $"{percentage:0.##}%";
    }

    private static string GetLetterGrade(
        double percentage)
    {
        if (percentage >= 90)
        {
            return "A";
        }

        if (percentage >= 80)
        {
            return "B";
        }

        if (percentage >= 70)
        {
            return "C";
        }

        if (percentage >= 60)
        {
            return "D";
        }

        return "F";
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

    private void AssignmentSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        selectedAssignment =
            AssignmentsCollectionView.SelectedItem
            as Assignment;

        ResponseEditor.Text =
            string.Empty;

        SubmissionStatusLabel.Text =
            string.Empty;

        if (selectedAssignment == null)
        {
            SelectedAssignmentLabel.Text =
                "No assignment selected";

            return;
        }

        SelectedAssignmentLabel.Text =
            $"Selected Assignment: " +
            $"{selectedAssignment.Name}";
    }

    private async void SubmitResponseClicked(
        object? sender,
        EventArgs e)
    {
        if (
            selectedStudent == null
            ||
            selectedCourse == null
        )
        {
            await DisplayAlertAsync(
                "Submission Failed",
                "The student or course could not be found.",
                "OK"
            );

            return;
        }

        if (selectedAssignment == null)
        {
            await DisplayAlertAsync(
                "No Assignment Selected",
                "Select an assignment before submitting.",
                "OK"
            );

            return;
        }

        string response =
            ResponseEditor.Text?.Trim()
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(response))
        {
            await DisplayAlertAsync(
                "Response Required",
                "Enter a response before submitting.",
                "OK"
            );

            return;
        }

        Submission? submission =
            CourseServiceProxy.Current.AddSubmission(
                selectedCourse.Id,
                selectedAssignment.Id,
                selectedStudent.Id,
                response
            );

        if (submission == null)
        {
            await DisplayAlertAsync(
                "Submission Failed",
                "The response could not be submitted.",
                "OK"
            );

            return;
        }

        SubmissionStatusLabel.Text =
            $"Submitted on " +
            $"{submission.SubmissionDate:MM/dd/yyyy h:mm tt}";

        ResponseEditor.Text =
            string.Empty;

        await DisplayAlertAsync(
            "Response Submitted",
            $"Your response to " +
            $"{selectedAssignment.Name} was submitted.",
            "OK"
        );
    }

    private void RefreshModuleContent()
    {
        displayedModuleContent.Clear();

        if (selectedCourse == null)
        {
            return;
        }

        foreach (
            Module module
            in selectedCourse.Modules)
        {
            displayedModuleContent.Add(
                module.DisplayText
            );

            foreach (
                ModuleItem item
                in module.Content)
            {
                displayedModuleContent.Add(
                    $"   {item.DisplayText}"
                );
            }
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
        selectedStudent = null;
        selectedCourse = null;
        selectedAssignment = null;

        StudentNameLabel.Text =
            "Student or course not found";

        CourseNameLabel.Text =
            string.Empty;

        CourseCodeLabel.Text =
            string.Empty;

        SemesterLabel.Text =
            string.Empty;

        SectionLabel.Text =
            string.Empty;

        DescriptionLabel.Text =
            string.Empty;

        LetterGradeLabel.Text =
            "Not Available";

        CoursePercentageLabel.Text =
            string.Empty;

        SelectedAssignmentLabel.Text =
            "No assignment selected";

        ResponseEditor.Text =
            string.Empty;

        SubmissionStatusLabel.Text =
            string.Empty;

        AssignmentsCollectionView.ItemsSource =
            null;

        displayedAnnouncements.Clear();
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