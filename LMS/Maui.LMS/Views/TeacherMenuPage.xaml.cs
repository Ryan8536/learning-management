using Library.LMS.Models;
using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

public partial class TeacherMenuPage :
    ContentPage
{
    private TeacherMenuViewModel viewModel;

    public TeacherMenuPage()
    {
        InitializeComponent();

        viewModel =
            new TeacherMenuViewModel();

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

    private async void OpenCourseClicked(
        object? sender,
        EventArgs e)
    {
        if (viewModel.SelectedCourse == null)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"//CourseMenuPage?courseId={viewModel.SelectedCourse.Id}"
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

    private async void CopyCourseClicked(
        object? sender,
        EventArgs e)
    {
        if (viewModel.SelectedCourse == null)
        {
            await DisplayAlertAsync(
                "No Course Selected",
                "Select a course before copying it.",
                "OK"
            );

            return;
        }

        Course? copiedCourse =
            viewModel.CopySelectedCourse();

        if (copiedCourse == null)
        {
            await DisplayAlertAsync(
                "Copy Failed",
                "The course could not be copied.",
                "OK"
            );

            return;
        }

        await DisplayAlertAsync(
            "Course Copied",
            $"{copiedCourse.Name} was created.",
            "OK"
        );
    }

    private void DeleteCourseClicked(
        object? sender,
        EventArgs e)
    {
        viewModel.DeleteSelectedCourse();
    }

    private async void ReturnToMainMenuClicked(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            "//MainPage"
        );
    }
}