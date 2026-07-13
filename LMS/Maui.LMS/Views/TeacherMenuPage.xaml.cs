using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

public partial class TeacherMenuPage : ContentPage
{
    private TeacherMenuViewModel viewModel;

    public TeacherMenuPage()
    {
        InitializeComponent();

        viewModel = new TeacherMenuViewModel();
        BindingContext = viewModel;
    }

    private void TeacherMenuPageNavigatedTo(
        object? sender,
        NavigatedToEventArgs e)
    {
        viewModel.RefreshCourses();
    }

    private async void AddCourseClicked(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("//CourseDetailPage");
    }
}