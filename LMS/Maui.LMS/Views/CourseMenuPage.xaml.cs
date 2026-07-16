using Library.LMS.Models;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class CourseMenuPage : ContentPage
{
    public int CourseId { get; set; }

    private Course? currentCourse;
    private Module? selectedModule;

    public CourseMenuPage()
    {
        InitializeComponent();
    }

    private void CourseMenuPageNavigatedTo(
        object? sender,
        NavigatedToEventArgs e)
    {
        currentCourse =
            CourseServiceProxy.Current.GetById(CourseId);

        BindingContext = currentCourse;

        selectedModule = null;
        ContentEntry.Text = "";

        RefreshModules();
        RefreshContent();
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

        RefreshContent();
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

        RefreshContent();
    }

    private void RefreshModules()
    {
        if (currentCourse == null)
        {
            ModulesCollectionView.ItemsSource = null;
            return;
        }

        ModulesCollectionView.ItemsSource =
            new List<Module>(currentCourse.Modules);
    }

    private void RefreshContent()
    {
        if (selectedModule == null)
        {
            ContentCollectionView.ItemsSource = null;
            return;
        }

        ContentCollectionView.ItemsSource =
            new List<string>(selectedModule.Content);
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