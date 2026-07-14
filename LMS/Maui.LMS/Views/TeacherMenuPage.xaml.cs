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
        await Shell.Current.GoToAsync(
            "//CourseDetailPage"
        );
    }

    private async void EditCourseClicked(
        object? sender,
        EventArgs e)
    {
        if (viewModel.SelectedCourse == null)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"//CourseDetailPage?courseId={viewModel.SelectedCourse.Id}"
        );
    }

    private void DeleteCourseClicked(
        object? sender,
        EventArgs e)
    {
        viewModel.DeleteSelectedCourse();
    }
}