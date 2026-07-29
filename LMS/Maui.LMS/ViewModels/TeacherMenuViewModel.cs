using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Library.LMS.Models;
using Library.LMS.Services;

namespace Maui.LMS.ViewModels;

public class TeacherMenuViewModel :
    INotifyPropertyChanged
{
    private ObservableCollection<Course> courses =
        new ObservableCollection<Course>();

    private ObservableCollection<string>
        semesterOptions =
            new ObservableCollection<string>();

    private Course? selectedCourse;
    private string? selectedSemester;

    public ObservableCollection<Course> Courses
    {
        get
        {
            return courses;
        }

        set
        {
            courses = value;
            NotifyPropertyChanged();
        }
    }

    public ObservableCollection<string>
        SemesterOptions
    {
        get
        {
            return semesterOptions;
        }

        set
        {
            semesterOptions = value;
            NotifyPropertyChanged();
        }
    }

    public Course? SelectedCourse
    {
        get
        {
            return selectedCourse;
        }

        set
        {
            selectedCourse = value;
            NotifyPropertyChanged();
        }
    }

    public string? SelectedSemester
    {
        get
        {
            return selectedSemester;
        }

        set
        {
            selectedSemester = value;
            NotifyPropertyChanged();

            RefreshDisplayedCourses();
        }
    }

    public void RefreshCourses()
    {
        CourseServiceProxy.Current.Refresh();

        string? previousSemester =
            SelectedSemester;

        List<string> semesters =
            CourseServiceProxy.Current.Courses
                .Where(
                    course =>
                        !string.IsNullOrWhiteSpace(
                            course.Semester
                        )
                )
                .Select(
                    course =>
                        course.Semester!
                )
                .Distinct()
                .OrderBy(
                    semester =>
                        GetSemesterYear(semester)
                )
                .ThenBy(
                    semester =>
                        GetSemesterTermOrder(
                            semester
                        )
                )
                .ThenBy(
                    semester => semester
                )
                .ToList();

        SemesterOptions =
            new ObservableCollection<string>();

        SemesterOptions.Add(
            "All Semesters"
        );

        foreach (
            string semester
            in semesters)
        {
            SemesterOptions.Add(semester);
        }

        if (
            previousSemester != null
            &&
            SemesterOptions.Contains(
                previousSemester
            )
        )
        {
            selectedSemester =
                previousSemester;
        }
        else
        {
            selectedSemester =
                "All Semesters";
        }

        NotifyPropertyChanged(
            nameof(SelectedSemester)
        );

        RefreshDisplayedCourses();
    }

    public Course? CopySelectedCourse()
    {
        if (SelectedCourse == null)
        {
            return null;
        }

        Course? copiedCourse =
            CourseServiceProxy.Current.CopyCourse(
                SelectedCourse.Id
            );

        RefreshCourses();

        return copiedCourse;
    }

    private void RefreshDisplayedCourses()
    {
        IEnumerable<Course> filteredCourses =
            CourseServiceProxy.Current.Courses;

        if (
            !string.IsNullOrWhiteSpace(
                SelectedSemester
            )
            &&
            SelectedSemester
                != "All Semesters"
        )
        {
            filteredCourses =
                filteredCourses.Where(
                    course =>
                        course.Semester
                        == SelectedSemester
                );
        }

        List<Course> sortedCourses =
            filteredCourses
                .OrderBy(
                    course =>
                        GetSemesterYear(
                            course.Semester
                        )
                )
                .ThenBy(
                    course =>
                        GetSemesterTermOrder(
                            course.Semester
                        )
                )
                .ThenBy(
                    course => course.Name
                )
                .ToList();

        Courses =
            new ObservableCollection<Course>(
                sortedCourses
            );

        SelectedCourse = null;
    }

    private int GetSemesterYear(
        string? semester)
    {
        if (string.IsNullOrWhiteSpace(semester))
        {
            return int.MaxValue;
        }

        string[] semesterParts =
            semester.Split(
                ' ',
                StringSplitOptions
                    .RemoveEmptyEntries
            );

        foreach (
            string part
            in semesterParts)
        {
            if (
                int.TryParse(
                    part,
                    out int year
                )
            )
            {
                return year;
            }
        }

        return int.MaxValue;
    }

    private int GetSemesterTermOrder(
        string? semester)
    {
        if (string.IsNullOrWhiteSpace(semester))
        {
            return int.MaxValue;
        }

        string lowerSemester =
            semester.ToLower();

        if (lowerSemester.Contains("spring"))
        {
            return 1;
        }

        if (lowerSemester.Contains("summer"))
        {
            return 2;
        }

        if (lowerSemester.Contains("fall"))
        {
            return 3;
        }

        return 4;
    }

    public void DeleteSelectedCourse()
    {
        if (SelectedCourse == null)
        {
            return;
        }

        CourseServiceProxy.Current.Delete(
            SelectedCourse.Id
        );

        SelectedCourse = null;

        RefreshCourses();
    }

    public event PropertyChangedEventHandler?
        PropertyChanged;

    private void NotifyPropertyChanged(
        [CallerMemberName]
        string propertyName = "")
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                propertyName
            )
        );
    }
}