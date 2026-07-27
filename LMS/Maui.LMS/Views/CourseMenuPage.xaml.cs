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
    private Student? selectedRosterStudent;
    private AssignmentGroup? selectedAssignmentGroup;

    private ObservableCollection<Student> displayedRoster;
    private ObservableCollection<Student> displayedStudents;
    private ObservableCollection<Module> displayedModules;
    private ObservableCollection<ModuleItem> displayedContent;
    private ObservableCollection<Assignment> displayedAssignments;
    private ObservableCollection<Course> displayedDestinationCourses;
    
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

        displayedDestinationCourses =
            new ObservableCollection<Course>();

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

        DestinationCoursesCollectionView.ItemsSource =
            displayedDestinationCourses;

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
        selectedRosterStudent = null;

        RosterCollectionView.SelectedItem = null;
        StudentsCollectionView.SelectedItem = null;
        ModulesCollectionView.SelectedItem = null;
        ContentCollectionView.SelectedItem = null;
        AssignmentsCollectionView.SelectedItem = null;
        ModuleAssignmentCollectionView.SelectedItem = null;
        DestinationCoursesCollectionView.SelectedItem = null;
        AssignmentGroupsCollectionView.SelectedItem = null;
        GroupAssignmentsCollectionView.SelectedItem = null;

        AvailableGroupAssignmentsCollectionView.SelectedItem =
            null;

        StudentNameEntry.Text = "";
        StudentCodeEntry.Text = "";
        StudentClassificationEntry.Text = "";
        ModuleNameEntry.Text = "";
        AssignmentGroupNameEntry.Text = "";
        AssignmentGroupWeightEntry.Text = "";

        ClearModuleItemForm();
        ClearAssignmentForm();

        RefreshRoster();
        RefreshStudents();
        RefreshModules();
        RefreshContent();
        RefreshAssignments();
        RefreshDestinationCourses();
        RefreshAssignmentGroups();
        RefreshGroupAssignments();
    }
    
    private void RosterSelectionChanged(
    object? sender,
    SelectionChangedEventArgs e)
{
    selectedRosterStudent =
        RosterCollectionView.SelectedItem
        as Student;
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
            "Select an available university student first.",
            "OK"
        );

        return;
    }

    CourseServiceProxy.Current.EnrollStudent(
        CourseId,
        selectedStudent
    );

    selectedStudent = null;

    StudentsCollectionView.SelectedItem =
        null;

    RefreshRoster();
    RefreshStudents();
}
    
    private async void RemoveStudentFromRosterClicked(
    object? sender,
    EventArgs e)
{
    if (currentCourse == null)
    {
        await DisplayAlertAsync(
            "Course Not Found",
            "The course could not be found.",
            "OK"
        );

        return;
    }

    if (selectedRosterStudent == null)
    {
        await DisplayAlertAsync(
            "No Student Selected",
            "Select a student from the course roster first.",
            "OK"
        );

        return;
    }

    Student studentToRemove =
        selectedRosterStudent;

    bool shouldRemove =
        await DisplayAlertAsync(
            "Remove Student",
            $"Remove {studentToRemove.Name} from this course?",
            "Remove",
            "Cancel"
        );

    if (!shouldRemove)
    {
        return;
    }

    currentCourse.Roster.RemoveAll(
        student =>
            student.Id == studentToRemove.Id
    );

    foreach (
        Assignment assignment
        in currentCourse.Assignments)
    {
        assignment.Submissions.RemoveAll(
            submission =>
                submission.StudentId ==
                studentToRemove.Id
        );
    }

    selectedRosterStudent = null;

    RosterCollectionView.SelectedItem =
        null;

    RefreshRoster();
    RefreshStudents();

    await DisplayAlertAsync(
        "Student Removed",
        $"{studentToRemove.Name} was removed from the course.",
        "OK"
    );
}

    private async void AddModuleClicked(
    object? sender,
    EventArgs e)
{
    if (string.IsNullOrWhiteSpace(
        ModuleNameEntry.Text))
    {
        await DisplayAlertAsync(
            "Missing Module Name",
            "Enter a module name.",
            "OK"
        );

        return;
    }

    CourseServiceProxy.Current.AddModule(
        CourseId,
        ModuleNameEntry.Text
    );

    ModuleNameEntry.Text =
        string.Empty;

    RefreshModules();
}

    private void ModuleSelectionChanged(
    object? sender,
    SelectionChangedEventArgs e)
{
    selectedModule =
        ModulesCollectionView.SelectedItem
        as Module;

    ModuleNameEntry.Text =
        selectedModule?.Name
        ?? string.Empty;

    ClearModuleItemForm();
    RefreshContent();
}

private async void UpdateModuleClicked(
    object? sender,
    EventArgs e)
{
    if (selectedModule == null)
    {
        await DisplayAlertAsync(
            "No Module Selected",
            "Select a module before updating it.",
            "OK"
        );

        return;
    }

    if (string.IsNullOrWhiteSpace(
        ModuleNameEntry.Text))
    {
        await DisplayAlertAsync(
            "Missing Module Name",
            "Enter a module name.",
            "OK"
        );

        return;
    }

    CourseServiceProxy.Current.UpdateModule(
        CourseId,
        selectedModule.Id,
        ModuleNameEntry.Text
    );

    ModuleNameEntry.Text =
        string.Empty;

    selectedModule =
        null;

    ModulesCollectionView.SelectedItem =
        null;

    RefreshModules();
    RefreshContent();
}

private async void DeleteModuleClicked(
    object? sender,
    EventArgs e)
{
    if (selectedModule == null)
    {
        await DisplayAlertAsync(
            "No Module Selected",
            "Select a module before deleting it.",
            "OK"
        );

        return;
    }

    bool shouldDelete =
        await DisplayAlertAsync(
            "Delete Module",
            "Delete the selected module and remove all of its content?",
            "Delete",
            "Cancel"
        );

    if (!shouldDelete)
    {
        return;
    }

    CourseServiceProxy.Current.DeleteModule(
        CourseId,
        selectedModule.Id
    );

    selectedModule =
        null;

    selectedContent =
        null;

    ModuleNameEntry.Text =
        string.Empty;

    ModulesCollectionView.SelectedItem =
        null;

    ContentCollectionView.SelectedItem =
        null;

    RefreshModules();
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

    private async void CopyAssignmentClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedAssignment == null)
        {
            await DisplayAlertAsync(
                "No Assignment Selected",
                "Select an assignment to copy.",
                "OK"
            );

            return;
        }

        Course? destinationCourse =
            DestinationCoursesCollectionView.SelectedItem
            as Course;

        if (destinationCourse == null)
        {
            await DisplayAlertAsync(
                "No Destination Course Selected",
                "Select the course that should receive the assignment.",
                "OK"
            );

            return;
        }

        Assignment? copiedAssignment =
            CourseServiceProxy.Current.CopyAssignment(
                CourseId,
                selectedAssignment.Id,
                destinationCourse.Id
            );

        if (copiedAssignment == null)
        {
            await DisplayAlertAsync(
                "Copy Failed",
                "The assignment could not be copied.",
                "OK"
            );

            return;
        }

        DestinationCoursesCollectionView.SelectedItem =
            null;

        await DisplayAlertAsync(
            "Assignment Copied",
            $"{copiedAssignment.Name} was copied to {destinationCourse.Name}.",
            "OK"
        );
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
            AssignmentGroupWeightEntry.Text = "";
        }
        else
        {
            AssignmentGroupNameEntry.Text =
                selectedAssignmentGroup.Name;

            AssignmentGroupWeightEntry.Text =
                selectedAssignmentGroup.Weight.ToString();
        }

        RefreshGroupAssignments();
    }

    private async void AddAssignmentGroupClicked(
        object? sender,
        EventArgs e)
    {
        bool groupFormIsValid =
            await ValidateAssignmentGroupForm();

        if (!groupFormIsValid)
        {
            return;
        }

        double.TryParse(
            AssignmentGroupWeightEntry.Text,
            out double weight
        );

        CourseServiceProxy.Current.AddAssignmentGroup(
            CourseId,
            AssignmentGroupNameEntry.Text
        );

        AssignmentGroup? newGroup =
            currentCourse?.AssignmentGroups
                .OrderByDescending(
                    group => group.Id
                )
                .FirstOrDefault();

        if (newGroup != null)
        {
            CourseServiceProxy.Current
                .UpdateAssignmentGroupWeight(
                    CourseId,
                    newGroup.Id,
                    weight
                );
        }

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

        bool groupFormIsValid =
            await ValidateAssignmentGroupForm();

        if (!groupFormIsValid)
        {
            return;
        }

        double.TryParse(
            AssignmentGroupWeightEntry.Text,
            out double weight
        );

        CourseServiceProxy.Current.UpdateAssignmentGroup(
            CourseId,
            selectedAssignmentGroup.Id,
            AssignmentGroupNameEntry.Text
        );

        CourseServiceProxy.Current
            .UpdateAssignmentGroupWeight(
                CourseId,
                selectedAssignmentGroup.Id,
                weight
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

    private async Task<bool> ValidateAssignmentGroupForm()
    {
        if (string.IsNullOrWhiteSpace(
            AssignmentGroupNameEntry.Text))
        {
            await DisplayAlertAsync(
                "Missing Group Name",
                "Enter an assignment group name.",
                "OK"
            );

            return false;
        }

        bool weightIsValid =
            double.TryParse(
                AssignmentGroupWeightEntry.Text,
                out double weight
            );

        if (!weightIsValid)
        {
            await DisplayAlertAsync(
                "Invalid Weight",
                "Enter a numeric assignment group weight.",
                "OK"
            );

            return false;
        }

        if (weight < 0)
        {
            await DisplayAlertAsync(
                "Invalid Weight",
                "The assignment group weight cannot be negative.",
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
        AssignmentGroupWeightEntry.Text = "";
    }

    private void RefreshRoster()
    {
    displayedRoster.Clear();

    RosterCollectionView.SelectedItem =
        null;

    selectedRosterStudent =
        null;

    if (currentCourse == null)
    {
        return;
    }

    foreach (
        Student student
        in currentCourse.Roster
            .OrderBy(student => student.Name))
    {
        displayedRoster.Add(student);
    }
}

    private void RefreshStudents()
{
    displayedStudents.Clear();

    StudentsCollectionView.SelectedItem =
        null;

    selectedStudent =
        null;

    foreach (
        Student student
        in StudentServiceProxy.Current.Students
            .Where(
                student =>
                    currentCourse == null
                    ||
                    !currentCourse.Roster.Any(
                        enrolledStudent =>
                            enrolledStudent.Id ==
                            student.Id
                    )
            )
            .OrderBy(student => student.Name))
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

    private void RefreshDestinationCourses()
    {
        displayedDestinationCourses.Clear();

        DestinationCoursesCollectionView.SelectedItem =
            null;

        foreach (
            Course course
            in CourseServiceProxy.Current.Courses
                .Where(course => course.Id != CourseId)
                .OrderBy(course => course.Name))
        {
            displayedDestinationCourses.Add(course);
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