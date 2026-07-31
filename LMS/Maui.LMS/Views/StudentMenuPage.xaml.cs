using System.Collections.ObjectModel;
using Library.LMS.Models;
using Library.LMS.Services;

namespace Maui.LMS.Views;

public partial class StudentMenuPage : ContentPage
{
    private Student? selectedStudent;

    private readonly ObservableCollection<Student>
        displayedStudents;

    public StudentMenuPage()
    {
        InitializeComponent();

        displayedStudents =
            new ObservableCollection<Student>();

        StudentsCollectionView.ItemsSource =
            displayedStudents;
    }

    private async void StudentMenuPageNavigatedTo(
        object? sender,
        NavigatedToEventArgs e)
    {
        selectedStudent = null;

        StudentsCollectionView.SelectedItem =
            null;

        IsBusy = true;

        await StudentServiceProxy.Current
            .RefreshAsync();

        RefreshStudents();

        IsBusy = false;
    }

    private void RefreshStudents()
    {
        displayedStudents.Clear();

        foreach (
            Student student
            in StudentServiceProxy.Current.Students
                .OrderBy(
                    student =>
                        student.Name
                ))
        {
            displayedStudents.Add(student);
        }
    }

    private void StudentSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        selectedStudent =
            StudentsCollectionView.SelectedItem
            as Student;
    }

    private async void ContinueClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedStudent == null)
        {
            await DisplayAlertAsync(
                "No Student Selected",
                "Select a student before continuing.",
                "OK"
            );

            return;
        }

        await Shell.Current.GoToAsync(
            $"//StudentCoursePage" +
            $"?studentId={selectedStudent.Id}"
        );
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