namespace Maui.LMS;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void TeacherClicked(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            "//TeacherMenuPage"
        );
    }

    private async void StudentClicked(
        object? sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            "//StudentMenuPage"
        );
    }
}