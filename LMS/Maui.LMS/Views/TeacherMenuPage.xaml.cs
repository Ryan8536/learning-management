using System.Collections.ObjectModel;
using Library.LMS.Models;
using Library.LMS.Services;
using Maui.LMS.ViewModels;

namespace Maui.LMS.Views;

public partial class TeacherMenuPage :
    ContentPage
{
    private readonly TeacherMenuViewModel
        viewModel;

    private readonly ObservableCollection<Student>
        displayedStudents;

    private Student? selectedStudent;

    public TeacherMenuPage()
    {
        InitializeComponent();

        viewModel =
            new TeacherMenuViewModel();

        displayedStudents =
            new ObservableCollection<Student>();

        BindingContext =
            viewModel;

        StudentsCollectionView.ItemsSource =
            displayedStudents;
    }

    private void TeacherMenuPageNavigatedTo(
        object? sender,
        NavigatedToEventArgs e)
    {
        viewModel.RefreshCourses();
        RefreshStudents();
    }

    private void RefreshStudents()
    {
        selectedStudent =
            null;

        StudentsCollectionView.SelectedItem =
            null;

        displayedStudents.Clear();

        foreach (
            Student student
            in StudentServiceProxy.Current.Students
                .OrderBy(
                    student => student.Name
                ))
        {
            displayedStudents.Add(student);
        }
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
            await DisplayAlertAsync(
                "No Course Selected",
                "Select a course before opening it.",
                "OK"
            );

            return;
        }

        await Shell.Current.GoToAsync(
            $"//CourseMenuPage" +
            $"?courseId={viewModel.SelectedCourse.Id}"
        );
    }

    private async void EditCourseClicked(
        object? sender,
        EventArgs e)
    {
        if (viewModel.SelectedCourse == null)
        {
            await DisplayAlertAsync(
                "No Course Selected",
                "Select a course before editing it.",
                "OK"
            );

            return;
        }

        await Shell.Current.GoToAsync(
            $"//CourseDetailPage" +
            $"?courseId={viewModel.SelectedCourse.Id}"
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

    private void StudentSelectionChanged(
        object? sender,
        SelectionChangedEventArgs e)
    {
        selectedStudent =
            StudentsCollectionView.SelectedItem
            as Student;
    }

    private async void AddStudentClicked(
        object? sender,
        EventArgs e)
    {
        string? name =
            await DisplayPromptAsync(
                "Add Student",
                "Enter the student's name:"
            );

        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        string? code =
            await DisplayPromptAsync(
                "Add Student",
                "Enter the student's code:"
            );

        if (string.IsNullOrWhiteSpace(code))
        {
            return;
        }

        string? classification =
            await DisplayPromptAsync(
                "Add Student",
                "Enter the student's classification:"
            );

        bool codeAlreadyExists =
            StudentServiceProxy.Current.Students.Any(
                student =>
                    student.Code == code.Trim()
            );

        if (codeAlreadyExists)
        {
            await DisplayAlertAsync(
                "Student Code Already Exists",
                "A student already uses that code.",
                "OK"
            );

            return;
        }

        Student? newStudent =
            StudentServiceProxy.Current.Add(
                name,
                code,
                classification
            );

        if (newStudent == null)
        {
            await DisplayAlertAsync(
                "Student Not Added",
                "The student could not be added.",
                "OK"
            );

            return;
        }

        RefreshStudents();

        await DisplayAlertAsync(
            "Student Added",
            $"{newStudent.Name} was added.",
            "OK"
        );
    }

    private async void EditStudentClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedStudent == null)
        {
            await DisplayAlertAsync(
                "No Student Selected",
                "Select a student before editing.",
                "OK"
            );

            return;
        }

        int selectedStudentId =
            selectedStudent.Id;

        string? name =
            await DisplayPromptAsync(
                "Edit Student",
                "Enter the student's name:",
                initialValue:
                    selectedStudent.Name
            );

        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        string? code =
            await DisplayPromptAsync(
                "Edit Student",
                "Enter the student's code:",
                initialValue:
                    selectedStudent.Code
            );

        if (string.IsNullOrWhiteSpace(code))
        {
            return;
        }

        string? classification =
            await DisplayPromptAsync(
                "Edit Student",
                "Enter the student's classification:",
                initialValue:
                    selectedStudent.Classification
            );

        bool updated =
            StudentServiceProxy.Current.Update(
                selectedStudentId,
                name,
                code,
                classification
            );

        if (!updated)
        {
            await DisplayAlertAsync(
                "Student Not Updated",
                "The student could not be updated. " +
                "The student code may already be in use.",
                "OK"
            );

            return;
        }

        RefreshStudents();

        await DisplayAlertAsync(
            "Student Updated",
            "The student information was updated.",
            "OK"
        );
    }

    private async void DeleteStudentClicked(
        object? sender,
        EventArgs e)
    {
        if (selectedStudent == null)
        {
            await DisplayAlertAsync(
                "No Student Selected",
                "Select a student before deleting.",
                "OK"
            );

            return;
        }

        Student studentToDelete =
            selectedStudent;

        bool confirmed =
            await DisplayAlertAsync(
                "Delete Student",
                $"Delete {studentToDelete.Name}? " +
                "This will remove the student from " +
                "all courses and delete their " +
                "submissions and grades.",
                "Delete",
                "Cancel"
            );

        if (!confirmed)
        {
            return;
        }

        bool deleted =
            StudentServiceProxy.Current.Delete(
                studentToDelete.Id
            );

        if (!deleted)
        {
            await DisplayAlertAsync(
                "Student Not Deleted",
                "The student could not be deleted.",
                "OK"
            );

            return;
        }

        RefreshStudents();
        viewModel.RefreshCourses();

        await DisplayAlertAsync(
            "Student Deleted",
            $"{studentToDelete.Name} was removed " +
            "from the university.",
            "OK"
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