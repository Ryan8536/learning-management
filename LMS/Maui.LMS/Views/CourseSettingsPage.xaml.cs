using Library.LMS.Models;
using Library.LMS.Services;

namespace Maui.LMS.Views;

[QueryProperty(nameof(CourseId), "courseId")]
public partial class CourseSettingsPage :
    ContentPage
{
    private Course? selectedCourse;

    public string? CourseId { get; set; }

    public CourseSettingsPage()
    {
        InitializeComponent();
    }

    private void CourseSettingsPageNavigatedTo(
        object? sender,
        NavigatedToEventArgs e)
    {
        if (
            !int.TryParse(
                CourseId,
                out int selectedCourseId
            )
        )
        {
            DisplayMissingCourse();

            return;
        }

        selectedCourse =
            CourseServiceProxy.Current.GetById(
                selectedCourseId
            );

        if (selectedCourse == null)
        {
            DisplayMissingCourse();

            return;
        }

        CourseNameLabel.Text =
            selectedCourse.Name;

        RefreshGradeRangeFields();
    }

    private void RefreshGradeRangeFields()
    {
        if (selectedCourse == null)
        {
            return;
        }

        MinimumAPercentageEntry.Text =
            selectedCourse.MinimumAPercentage
                .ToString("0.##");

        MinimumBPercentageEntry.Text =
            selectedCourse.MinimumBPercentage
                .ToString("0.##");

        MinimumCPercentageEntry.Text =
            selectedCourse.MinimumCPercentage
                .ToString("0.##");

        MinimumDPercentageEntry.Text =
            selectedCourse.MinimumDPercentage
                .ToString("0.##");
    }

    private async void SaveGradeRangesClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedCourse == null)
        {
            await DisplayAlertAsync(
                "Course Not Found",
                "The course could not be found.",
                "OK"
            );

            return;
        }

        bool aIsValid =
            double.TryParse(
                MinimumAPercentageEntry.Text,
                out double minimumA
            );

        bool bIsValid =
            double.TryParse(
                MinimumBPercentageEntry.Text,
                out double minimumB
            );

        bool cIsValid =
            double.TryParse(
                MinimumCPercentageEntry.Text,
                out double minimumC
            );

        bool dIsValid =
            double.TryParse(
                MinimumDPercentageEntry.Text,
                out double minimumD
            );

        if (
            !aIsValid
            ||
            !bIsValid
            ||
            !cIsValid
            ||
            !dIsValid
        )
        {
            await DisplayAlertAsync(
                "Invalid Grade Range",
                "Each grade range must be a number.",
                "OK"
            );

            return;
        }

        bool percentagesAreInRange =
            minimumA >= 0
            &&
            minimumA <= 100
            &&
            minimumB >= 0
            &&
            minimumB <= 100
            &&
            minimumC >= 0
            &&
            minimumC <= 100
            &&
            minimumD >= 0
            &&
            minimumD <= 100;

        if (!percentagesAreInRange)
        {
            await DisplayAlertAsync(
                "Invalid Grade Range",
                "Each percentage must be between 0 and 100.",
                "OK"
            );

            return;
        }

        bool rangesAreOrdered =
            minimumA > minimumB
            &&
            minimumB > minimumC
            &&
            minimumC > minimumD;

        if (!rangesAreOrdered)
        {
            await DisplayAlertAsync(
                "Invalid Grade Order",
                "The ranges must follow A > B > C > D.",
                "OK"
            );

            return;
        }

        bool updateSucceeded =
            CourseServiceProxy.Current
                .UpdateGradeRanges(
                    selectedCourse.Id,
                    minimumA,
                    minimumB,
                    minimumC,
                    minimumD
                );

        if (!updateSucceeded)
        {
            await DisplayAlertAsync(
                "Update Failed",
                "The grade ranges could not be updated.",
                "OK"
            );

            return;
        }

        RefreshGradeRangeFields();

        await DisplayAlertAsync(
            "Grade Ranges Saved",
            "The course letter-grade ranges were updated.",
            "OK"
        );
    }

    private async void RestoreDefaultsClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedCourse == null)
        {
            await DisplayAlertAsync(
                "Course Not Found",
                "The course could not be found.",
                "OK"
            );

            return;
        }

        bool shouldRestore =
            await DisplayAlertAsync(
                "Restore Defaults",
                "Restore the standard 90, 80, 70, and 60 grade ranges?",
                "Restore",
                "Cancel"
            );

        if (!shouldRestore)
        {
            return;
        }

        CourseServiceProxy.Current.UpdateGradeRanges(
            selectedCourse.Id,
            90,
            80,
            70,
            60
        );

        RefreshGradeRangeFields();

        await DisplayAlertAsync(
            "Defaults Restored",
            "The standard letter-grade ranges were restored.",
            "OK"
        );
    }

    private void DisplayMissingCourse()
    {
        selectedCourse = null;

        CourseNameLabel.Text =
            "Course not found";

        MinimumAPercentageEntry.Text =
            string.Empty;

        MinimumBPercentageEntry.Text =
            string.Empty;

        MinimumCPercentageEntry.Text =
            string.Empty;

        MinimumDPercentageEntry.Text =
            string.Empty;
    }

   private async void ReturnToCourseMenuClicked(
    object? sender,
    EventArgs e)
{
    await Shell.Current.GoToAsync("..");
}
}