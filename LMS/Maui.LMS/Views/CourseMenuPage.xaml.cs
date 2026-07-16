using Library.LMS.Models;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class CourseMenuPage : ContentPage
{
    public int CourseId { get; set; }

    private Course? currentCourse;

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

        RefreshModules();
    }

    private void AddModuleClicked(
        object? sender,
        EventArgs e)
    {
        CourseServiceProxy.Current.AddModule(CourseId);

        RefreshModules();
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

    private async void BackClicked(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            "//TeacherMenuPage"
        );
    }
}