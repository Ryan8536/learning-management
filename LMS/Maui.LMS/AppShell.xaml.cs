using Maui.LMS.Views;

namespace Maui.LMS;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(
            nameof(CourseSettingsPage),
            typeof(CourseSettingsPage)
        );
    }
}