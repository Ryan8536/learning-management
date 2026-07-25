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
    private ModuleItem? selectedContent;
    private Assignment? selectedAssignment;
    private Student? selectedStudent;
    private AssignmentGroup? selectedAssignmentGroup;

    private ObservableCollection<Student> displayedRoster;
    private ObservableCollection<Student> displayedStudents;
    private ObservableCollection<Module> displayedModules;
    private ObservableCollection<ModuleItem> displayedContent;
    private ObservableCollection<Assignment> displayedAssignments;
    private ObservableCollection<AssignmentGroup>
        displayedAssignmentGroups;
    private ObservableCollection<Assignment>
        displayedGroupAssignments;

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
            new ObservableCollection<ModuleItem>();

        displayedAssignments =
            new ObservableCollection<Assignment>();

        displayedAssignmentGroups =
            new ObservableCollection<AssignmentGroup>();

        displayedGroupAssignments =
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

        ModuleAssignmentCollectionView.ItemsSource =
            displayedAssignments;

        AssignmentGroupsCollectionView.ItemsSource =
            displayedAssignmentGroups;

        GroupAssignmentsCollectionView.ItemsSource =
            displayedGroupAssignments;

        AvailableGroupAssignmentsCollectionView.ItemsSource =
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
        selectedAssignmentGroup = null;

        StudentsCollectionView.SelectedItem = null;
        ModulesCollectionView.SelectedItem = null;
        ContentCollectionView.SelectedItem = null;
        AssignmentsCollectionView.SelectedItem = null;
        ModuleAssignmentCollectionView.SelectedItem = null;
        AssignmentGroupsCollectionView.SelectedItem = null;
        GroupAssignmentsCollectionView.SelectedItem = null;
        AvailableGroupAssignmentsCollectionView.SelectedItem =
            null;

        StudentNameEntry.Text = "";
        StudentCodeEntry.Text = "";
        StudentClassificationEntry.Text = "";
        AssignmentGroupNameEntry.Text = "";

        ClearModuleItemForm();
        ClearAssignmentForm();

        RefreshRoster();
        RefreshStudents();
        RefreshModules();
        RefreshContent();
        RefreshAssignments();
        RefreshAssignmentGroups();
        RefreshGroupAssignments();
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
            ModulesCollectionView.SelectedItem
            as Module;

        ClearModuleItemForm();
        RefreshContent();
    }

    private void ContentSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        selectedContent =
            ContentCollectionView.SelectedItem
            as ModuleItem;

        ContentNameEntry.Text = "";
        ContentDetailsEditor.Text = "";

        if (selectedContent is ModulePage page)
        {
            ContentNameEntry.Text =
                page.Name;

            ContentDetailsEditor.Text =
                page.Body;
        }
        else if (selectedContent is ModuleFile file)
        {
            ContentNameEntry.Text =
                file.Name;

            ContentDetailsEditor.Text =
                file.FilePath;
        }
    }

    private async void AddPageClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedModule == null)
        {
            await DisplayAlertAsync(
                "No Module Selected",
                "Select a module first.",
                "OK"
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(
            ContentNameEntry.Text))
        {
            await DisplayAlertAsync(
                "Missing Page Name",
                "Enter a name for the page.",
                "OK"
            );

            return;
        }

        CourseServiceProxy.Current.AddModulePage(
            CourseId,
            selectedModule.Id,
            ContentNameEntry.Text,
            ContentDetailsEditor.Text
        );

        ClearModuleItemForm();
        RefreshContent();
    }

    private async void AddFileClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedModule == null)
        {
            await DisplayAlertAsync(
                "No Module Selected",
                "Select a module first.",
                "OK"
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(
            ContentNameEntry.Text))
        {
            await DisplayAlertAsync(
                "Missing File Name",
                "Enter a name for the file.",
                "OK"
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(
            ContentDetailsEditor.Text))
        {
            await DisplayAlertAsync(
                "Missing File Path",
                "Enter the path or location of the file.",
                "OK"
            );

            return;
        }

        CourseServiceProxy.Current.AddModuleFile(
            CourseId,
            selectedModule.Id,
            ContentNameEntry.Text,
            ContentDetailsEditor.Text
        );

        ClearModuleItemForm();
        RefreshContent();
    }

    private async void AddAssignmentToModuleClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedModule == null)
        {
            await DisplayAlertAsync(
                "No Module Selected",
                "Select a module first.",
                "OK"
            );

            return;
        }

        Assignment? assignment =
            ModuleAssignmentCollectionView.SelectedItem
            as Assignment;

        if (assignment == null)
        {
            await DisplayAlertAsync(
                "No Assignment Selected",
                "Select an assignment first.",
                "OK"
            );

            return;
        }

        CourseServiceProxy.Current.AddAssignmentToModule(
            CourseId,
            selectedModule.Id,
            assignment.Id
        );

        ModuleAssignmentCollectionView.SelectedItem =
            null;

        RefreshContent();
    }

    private async void UpdateContentClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedModule == null)
        {
            await DisplayAlertAsync(
                "No Module Selected",
                "Select a module first.",
                "OK"
            );

            return;
        }

        if (selectedContent == null)
        {
            await DisplayAlertAsync(
                "No Module Item Selected",
                "Select a page or file first.",
                "OK"
            );

            return;
        }

        if (selectedContent is Assignment)
        {
            await DisplayAlertAsync(
                "Assignment Selected",
                "Edit assignments using the assignment section.",
                "OK"
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(
            ContentNameEntry.Text))
        {
            await DisplayAlertAsync(
                "Missing Name",
                "Enter a name for the module item.",
                "OK"
            );

            return;
        }

        CourseServiceProxy.Current.UpdateModuleItem(
            CourseId,
            selectedModule.Id,
            selectedContent,
            ContentNameEntry.Text,
            ContentDetailsEditor.Text
        );

        ClearModuleItemForm();
        RefreshContent();
    }

    private async void RemoveContentClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedModule == null)
        {
            await DisplayAlertAsync(
                "No Module Selected",
                "Select a module first.",
                "OK"
            );

            return;
        }

        if (selectedContent == null)
        {
            await DisplayAlertAsync(
                "No Module Item Selected",
                "Select a module item first.",
                "OK"
            );

            return;
        }

        bool shouldRemove =
            await DisplayAlertAsync(
                "Remove Module Item",
                "Remove the selected item from this module?",
                "Remove",
                "Cancel"
            );

        if (!shouldRemove)
        {
            return;
        }

        CourseServiceProxy.Current.RemoveModuleItem(
            CourseId,
            selectedModule.Id,
            selectedContent
        );

        ClearModuleItemForm();
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
        RefreshContent();
        RefreshAssignmentGroups();
        RefreshGroupAssignments();
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
        RefreshContent();
        RefreshAssignmentGroups();
        RefreshGroupAssignments();
    }

    private void ClearAssignmentFormClicked(
        object? sender,
        EventArgs e)
    {
        ClearAssignmentSelection();
    }

    private void AssignmentGroupSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        selectedAssignmentGroup =
            AssignmentGroupsCollectionView.SelectedItem
            as AssignmentGroup;

        GroupAssignmentsCollectionView.SelectedItem =
            null;

        AvailableGroupAssignmentsCollectionView.SelectedItem =
            null;

        if (selectedAssignmentGroup == null)
        {
            AssignmentGroupNameEntry.Text = "";
        }
        else
        {
            AssignmentGroupNameEntry.Text =
                selectedAssignmentGroup.Name;
        }

        RefreshGroupAssignments();
    }

    private async void AddAssignmentGroupClicked(
        object? sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(
            AssignmentGroupNameEntry.Text))
        {
            await DisplayAlertAsync(
                "Missing Group Name",
                "Enter an assignment group name.",
                "OK"
            );

            return;
        }

        CourseServiceProxy.Current.AddAssignmentGroup(
            CourseId,
            AssignmentGroupNameEntry.Text
        );

        ClearAssignmentGroupSelection();
        RefreshAssignmentGroups();
    }

    private async void UpdateAssignmentGroupClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedAssignmentGroup == null)
        {
            await DisplayAlertAsync(
                "No Group Selected",
                "Select an assignment group first.",
                "OK"
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(
            AssignmentGroupNameEntry.Text))
        {
            await DisplayAlertAsync(
                "Missing Group Name",
                "Enter an assignment group name.",
                "OK"
            );

            return;
        }

        CourseServiceProxy.Current.UpdateAssignmentGroup(
            CourseId,
            selectedAssignmentGroup.Id,
            AssignmentGroupNameEntry.Text
        );

        ClearAssignmentGroupSelection();
        RefreshAssignmentGroups();
    }

    private async void DeleteAssignmentGroupClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedAssignmentGroup == null)
        {
            await DisplayAlertAsync(
                "No Group Selected",
                "Select an assignment group first.",
                "OK"
            );

            return;
        }

        bool shouldDelete =
            await DisplayAlertAsync(
                "Delete Assignment Group",
                "Delete the selected assignment group?",
                "Delete",
                "Cancel"
            );

        if (!shouldDelete)
        {
            return;
        }

        CourseServiceProxy.Current.DeleteAssignmentGroup(
            CourseId,
            selectedAssignmentGroup.Id
        );

        ClearAssignmentGroupSelection();
        RefreshAssignmentGroups();
        RefreshGroupAssignments();
    }

    private async void AddAssignmentToGroupClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedAssignmentGroup == null)
        {
            await DisplayAlertAsync(
                "No Group Selected",
                "Select an assignment group first.",
                "OK"
            );

            return;
        }

        Assignment? assignment =
            AvailableGroupAssignmentsCollectionView
                .SelectedItem
            as Assignment;

        if (assignment == null)
        {
            await DisplayAlertAsync(
                "No Assignment Selected",
                "Select an assignment first.",
                "OK"
            );

            return;
        }

        CourseServiceProxy.Current.AddAssignmentToGroup(
            CourseId,
            selectedAssignmentGroup.Id,
            assignment.Id
        );

        AvailableGroupAssignmentsCollectionView.SelectedItem =
            null;

        RefreshAssignmentGroups();
        RefreshGroupAssignments();
    }

    private async void RemoveAssignmentFromGroupClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedAssignmentGroup == null)
        {
            await DisplayAlertAsync(
                "No Group Selected",
                "Select an assignment group first.",
                "OK"
            );

            return;
        }

        Assignment? assignment =
            GroupAssignmentsCollectionView.SelectedItem
            as Assignment;

        if (assignment == null)
        {
            await DisplayAlertAsync(
                "No Assignment Selected",
                "Select an assignment from the group first.",
                "OK"
            );

            return;
        }

        CourseServiceProxy.Current
            .RemoveAssignmentFromGroup(
                CourseId,
                selectedAssignmentGroup.Id,
                assignment.Id
            );

        GroupAssignmentsCollectionView.SelectedItem =
            null;

        RefreshAssignmentGroups();
        RefreshGroupAssignments();
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

        bool pointsAreValid =
            int.TryParse(
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

        if (availablePoints < 0)
        {
            await DisplayAlertAsync(
                "Invalid Points",
                "Available points cannot be negative.",
                "OK"
            );

            return false;
        }

        return true;
    }

    private void ClearModuleItemForm()
    {
        selectedContent = null;

        ContentCollectionView.SelectedItem =
            null;

        ContentNameEntry.Text = "";
        ContentDetailsEditor.Text = "";
    }

    private void ClearAssignmentSelection()
    {
        selectedAssignment = null;

        AssignmentsCollectionView.SelectedItem =
            null;

        ClearAssignmentForm();
    }

    private void ClearAssignmentForm()
    {
        AssignmentNameEntry.Text = "";
        AssignmentDescriptionEditor.Text = "";
        AssignmentPointsEntry.Text = "";

        AssignmentDueDatePicker.Date =
            DateTime.Today;
    }

    private void ClearAssignmentGroupSelection()
    {
        selectedAssignmentGroup = null;

        AssignmentGroupsCollectionView.SelectedItem =
            null;

        GroupAssignmentsCollectionView.SelectedItem =
            null;

        AvailableGroupAssignmentsCollectionView.SelectedItem =
            null;

        AssignmentGroupNameEntry.Text = "";
    }

    private void RefreshRoster()
    {
        displayedRoster.Clear();

        if (currentCourse == null)
        {
            return;
        }

        foreach (
            Student student
            in currentCourse.Roster)
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

        foreach (
            Module module
            in currentCourse.Modules)
        {
            displayedModules.Add(module);
        }
    }

    private void RefreshContent()
    {
        displayedContent.Clear();

        ContentCollectionView.SelectedItem =
            null;

        if (selectedModule == null)
        {
            return;
        }

        foreach (
            ModuleItem item
            in selectedModule.Content)
        {
            displayedContent.Add(item);
        }
    }

    private void RefreshAssignments()
    {
        displayedAssignments.Clear();

        AssignmentsCollectionView.SelectedItem =
            null;

        ModuleAssignmentCollectionView.SelectedItem =
            null;

        AvailableGroupAssignmentsCollectionView.SelectedItem =
            null;

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

    private void RefreshAssignmentGroups()
    {
        displayedAssignmentGroups.Clear();

        if (currentCourse == null)
        {
            return;
        }

        foreach (
            AssignmentGroup group
            in currentCourse.AssignmentGroups)
        {
            displayedAssignmentGroups.Add(group);
        }
    }

    private void RefreshGroupAssignments()
    {
        displayedGroupAssignments.Clear();

        GroupAssignmentsCollectionView.SelectedItem =
            null;

        if (selectedAssignmentGroup == null)
        {
            return;
        }

        foreach (
            Assignment assignment
            in selectedAssignmentGroup.Assignments)
        {
            displayedGroupAssignments.Add(assignment);
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