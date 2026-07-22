using System.Collections.ObjectModel;
using Library.LMS.Models;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class CourseMenuPage : ContentPage
{
    public int CourseId { get; set; }

    private Course? currentCourse;
    private Module? selectedModule;
    private string? selectedContent;
    private Assignment? selectedAssignment;
    private Student? selectedStudent;

    private ObservableCollection<Student> displayedRoster;
    private ObservableCollection<Student> displayedStudents;
    private ObservableCollection<Module> displayedModules;
    private ObservableCollection<string> displayedContent;
    private ObservableCollection<Assignment> displayedAssignments;

    public CourseMenuPage()
    {
        InitializeComponent();

        displayedRoster =
            new ObservableCollection<Student>();

        displayedStudents =
            new ObservableCollection<Student>();

        displayedModules =
            new ObservableCollection<Module>();

        displayedContent =
            new ObservableCollection<string>();

        displayedAssignments =
            new ObservableCollection<Assignment>();

        RosterCollectionView.ItemsSource =
            displayedRoster;

        StudentsCollectionView.ItemsSource =
            displayedStudents;

        ModulesCollectionView.ItemsSource =
            displayedModules;

        ContentCollectionView.ItemsSource =
            displayedContent;

        AssignmentsCollectionView.ItemsSource =
            displayedAssignments;
    }

    private void CourseMenuPageNavigatedTo(
        object? sender,
        NavigatedToEventArgs e)
    {
        currentCourse =
            CourseServiceProxy.Current.GetById(CourseId);

        BindingContext = currentCourse;

        selectedStudent = null;
        selectedModule = null;
        selectedContent = null;
        selectedAssignment = null;

        StudentsCollectionView.SelectedItem = null;
        ModulesCollectionView.SelectedItem = null;
        ContentCollectionView.SelectedItem = null;
        AssignmentsCollectionView.SelectedItem = null;

        StudentNameEntry.Text = "";
        StudentCodeEntry.Text = "";
        StudentClassificationEntry.Text = "";

        ContentEntry.Text = "";

        ClearAssignmentForm();

        RefreshRoster();
        RefreshStudents();
        RefreshModules();
        RefreshContent();
        RefreshAssignments();
    }

    private void StudentSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        selectedStudent =
            StudentsCollectionView.SelectedItem
            as Student;
    }

    private async void CreateAndEnrollStudentClicked(
        object? sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(
            StudentNameEntry.Text))
        {
            await DisplayAlertAsync(
                "Missing Name",
                "Enter the student's name.",
                "OK"
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(
            StudentCodeEntry.Text))
        {
            await DisplayAlertAsync(
                "Missing Student Code",
                "Enter the student's code.",
                "OK"
            );

            return;
        }

        Student? student =
            StudentServiceProxy.Current.Add(
                StudentNameEntry.Text,
                StudentCodeEntry.Text,
                StudentClassificationEntry.Text
            );

        CourseServiceProxy.Current.EnrollStudent(
            CourseId,
            student
        );

        StudentNameEntry.Text = "";
        StudentCodeEntry.Text = "";
        StudentClassificationEntry.Text = "";

        RefreshStudents();
        RefreshRoster();
    }

    private async void EnrollExistingStudentClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedStudent == null)
        {
            await DisplayAlertAsync(
                "No Student Selected",
                "Select an existing student first.",
                "OK"
            );

            return;
        }

        CourseServiceProxy.Current.EnrollStudent(
            CourseId,
            selectedStudent
        );

        selectedStudent = null;
        StudentsCollectionView.SelectedItem = null;

        RefreshRoster();
    }

    private void AddModuleClicked(
        object? sender,
        EventArgs e)
    {
        CourseServiceProxy.Current.AddModule(CourseId);

        RefreshModules();
    }

    private void ModuleSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        selectedModule =
            ModulesCollectionView.SelectedItem as Module;

        selectedContent = null;
        ContentEntry.Text = "";

        RefreshContent();
    }

    private void ContentSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        selectedContent =
            ContentCollectionView.SelectedItem as string;

        if (selectedContent != null)
        {
            ContentEntry.Text = selectedContent;
        }
    }

    private void AddContentClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedModule == null)
        {
            return;
        }

        CourseServiceProxy.Current.AddModuleContent(
            CourseId,
            selectedModule.Id,
            ContentEntry.Text
        );

        ContentEntry.Text = "";
        selectedContent = null;

        RefreshContent();
    }

    private void UpdateContentClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedModule == null)
        {
            return;
        }

        if (selectedContent == null)
        {
            return;
        }

        CourseServiceProxy.Current.UpdateModuleContent(
            CourseId,
            selectedModule.Id,
            selectedContent,
            ContentEntry.Text
        );

        ContentEntry.Text = "";
        selectedContent = null;

        RefreshContent();
    }

    private void RemoveContentClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedModule == null)
        {
            return;
        }

        if (selectedContent == null)
        {
            return;
        }

        CourseServiceProxy.Current.RemoveModuleContent(
            CourseId,
            selectedModule.Id,
            selectedContent
        );

        ContentEntry.Text = "";
        selectedContent = null;

        RefreshContent();
    }

    private void AssignmentSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        selectedAssignment =
            AssignmentsCollectionView.SelectedItem
            as Assignment;

        if (selectedAssignment == null)
        {
            return;
        }

        AssignmentNameEntry.Text =
            selectedAssignment.Name;

        AssignmentDescriptionEditor.Text =
            selectedAssignment.Description;

        AssignmentPointsEntry.Text =
            selectedAssignment.AvailablePoints.ToString();

        AssignmentDueDatePicker.Date =
            selectedAssignment.DueDate;
    }

    private async void AddAssignmentClicked(
        object? sender,
        EventArgs e)
    {
        bool formIsValid =
            await ValidateAssignmentForm();

        if (!formIsValid)
        {
            return;
        }

        int.TryParse(
            AssignmentPointsEntry.Text,
            out int availablePoints
        );

        DateTime dueDate =
            AssignmentDueDatePicker.Date
            ?? DateTime.Today;

        CourseServiceProxy.Current.AddAssignment(
            CourseId,
            AssignmentNameEntry.Text,
            AssignmentDescriptionEditor.Text,
            availablePoints,
            dueDate
        );

        ClearAssignmentSelection();
        RefreshAssignments();
    }

    private async void UpdateAssignmentClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedAssignment == null)
        {
            await DisplayAlertAsync(
                "No Assignment Selected",
                "Select an assignment before updating it.",
                "OK"
            );

            return;
        }

        bool formIsValid =
            await ValidateAssignmentForm();

        if (!formIsValid)
        {
            return;
        }

        int.TryParse(
            AssignmentPointsEntry.Text,
            out int availablePoints
        );

        DateTime dueDate =
            AssignmentDueDatePicker.Date
            ?? DateTime.Today;

        CourseServiceProxy.Current.UpdateAssignment(
            CourseId,
            selectedAssignment.Id,
            AssignmentNameEntry.Text,
            AssignmentDescriptionEditor.Text,
            availablePoints,
            dueDate
        );

        ClearAssignmentSelection();
        RefreshAssignments();
    }

    private async void DeleteAssignmentClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedAssignment == null)
        {
            await DisplayAlertAsync(
                "No Assignment Selected",
                "Select an assignment before deleting it.",
                "OK"
            );

            return;
        }

        bool shouldDelete =
            await DisplayAlertAsync(
                "Delete Assignment",
                "Delete the selected assignment and all of its submissions?",
                "Delete",
                "Cancel"
            );

        if (!shouldDelete)
        {
            return;
        }

        CourseServiceProxy.Current.DeleteAssignment(
            CourseId,
            selectedAssignment.Id
        );

        ClearAssignmentSelection();
        RefreshAssignments();
    }

    private void ClearAssignmentFormClicked(
        object? sender,
        EventArgs e)
    {
        ClearAssignmentSelection();
    }

    private async Task<bool> ValidateAssignmentForm()
    {
        if (string.IsNullOrWhiteSpace(
            AssignmentNameEntry.Text))
        {
            await DisplayAlertAsync(
                "Missing Name",
                "Enter an assignment name.",
                "OK"
            );

            return false;
        }

        bool pointsAreValid = int.TryParse(
            AssignmentPointsEntry.Text,
            out int availablePoints
        );

        if (!pointsAreValid)
        {
            await DisplayAlertAsync(
                "Invalid Points",
                "Available points must be a whole number.",
                "OK"
            );

            return false;
        }

        return true;
    }

    private void ClearAssignmentSelection()
    {
        selectedAssignment = null;
        AssignmentsCollectionView.SelectedItem = null;

        ClearAssignmentForm();
    }

    private void ClearAssignmentForm()
    {
        AssignmentNameEntry.Text = "";
        AssignmentDescriptionEditor.Text = "";
        AssignmentPointsEntry.Text = "";
        AssignmentDueDatePicker.Date = DateTime.Today;
    }

    private void RefreshRoster()
    {
        displayedRoster.Clear();

        if (currentCourse == null)
        {
            return;
        }

        foreach (Student student in currentCourse.Roster)
        {
            displayedRoster.Add(student);
        }
    }

    private void RefreshStudents()
    {
        displayedStudents.Clear();

        foreach (
            Student student
            in StudentServiceProxy.Current.Students)
        {
            displayedStudents.Add(student);
        }
    }

    private void RefreshModules()
    {
        displayedModules.Clear();

        if (currentCourse == null)
        {
            return;
        }

        foreach (Module module in currentCourse.Modules)
        {
            displayedModules.Add(module);
        }
    }

    private void RefreshContent()
    {
        displayedContent.Clear();

        ContentCollectionView.SelectedItem = null;

        if (selectedModule == null)
        {
            return;
        }

        foreach (string content in selectedModule.Content)
        {
            displayedContent.Add(content);
        }
    }

    private void RefreshAssignments()
    {
        displayedAssignments.Clear();

        AssignmentsCollectionView.SelectedItem = null;

        if (currentCourse == null)
        {
            return;
        }

        foreach (
            Assignment assignment
            in currentCourse.Assignments)
        {
            displayedAssignments.Add(assignment);
        }
    }

    private async void BackClicked(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            "//TeacherMenuPage"
        );
    }
}