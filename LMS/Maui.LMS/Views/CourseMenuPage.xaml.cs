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

    private ObservableCollection<Module> displayedModules;
    private ObservableCollection<string> displayedContent;
    private ObservableCollection<Assignment> displayedAssignments;

    public CourseMenuPage()
    {
        InitializeComponent();

        displayedModules =
            new ObservableCollection<Module>();

        displayedContent =
            new ObservableCollection<string>();

        displayedAssignments =
            new ObservableCollection<Assignment>();

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

        selectedModule = null;
        selectedContent = null;

        ContentEntry.Text = "";

        AssignmentNameEntry.Text = "";
        AssignmentDescriptionEditor.Text = "";
        AssignmentPointsEntry.Text = "";
        AssignmentDueDatePicker.Date = DateTime.Today;

        RefreshModules();
        RefreshContent();
        RefreshAssignments();
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

    private async void AddAssignmentClicked(
        object? sender,
        EventArgs e)
    {
        int availablePoints;

        bool pointsAreValid = int.TryParse(
            AssignmentPointsEntry.Text,
            out availablePoints
        );

        if (!pointsAreValid)
        {
            await DisplayAlertAsync(
                "Invalid Points",
                "Available points must be a whole number.",
                "OK"
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(
            AssignmentNameEntry.Text))
        {
            await DisplayAlertAsync(
                "Missing Name",
                "Enter an assignment name.",
                "OK"
            );

            return;
        }

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

        AssignmentNameEntry.Text = "";
        AssignmentDescriptionEditor.Text = "";
        AssignmentPointsEntry.Text = "";
        AssignmentDueDatePicker.Date = DateTime.Today;

        RefreshAssignments();
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