using Library.LMS.Models;
using Library.LMS.Services;
using Microsoft.Extensions.Logging;

namespace Maui.LMS;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont(
                    "OpenSans-Regular.ttf",
                    "OpenSansRegular"
                );

                fonts.AddFont(
                    "OpenSans-Semibold.ttf",
                    "OpenSansSemibold"
                );
            });

        Student? ryan =
            StudentServiceProxy.Current.Add(
                "Ryan",
                "RE24A",
                "Junior"
            );

        Student? alex =
            StudentServiceProxy.Current.Add(
                "Alex",
                "AS25B",
                "Sophomore"
            );

        Course? programming =
            CourseServiceProxy.Current.Courses
                .FirstOrDefault(
                    course =>
                        course.Code == "COP3330"
                );

        if (programming == null)
        {
            programming = new Course
            {
                Id = GetNextCourseId(),
                Name = "Programming",
                Code = "COP3330",
                Description =
                    "Object-oriented programming",
                Semester = "Fall 2026",
                Section = "001"
            };

            CourseServiceProxy.Current.Courses.Add(
                programming
            );
        }

        Course? circuits =
            CourseServiceProxy.Current.Courses
                .FirstOrDefault(
                    course =>
                        course.Code == "EEL3003"
                );

        if (circuits == null)
        {
            circuits = new Course
            {
                Id = GetNextCourseId(),
                Name = "Circuits",
                Code = "EEL3003",
                Description =
                    "Introduction to circuit analysis",
                Semester = "Fall 2026",
                Section = "001"
            };

            CourseServiceProxy.Current.Courses.Add(
                circuits
            );
        }

        if (ryan != null)
        {
            EnrollStudentDirectly(
                programming,
                ryan
            );

            EnrollStudentDirectly(
                circuits,
                ryan
            );
        }

        if (alex != null)
        {
            EnrollStudentDirectly(
                programming,
                alex
            );
        }

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static int GetNextCourseId()
    {
        List<Course> courses =
            CourseServiceProxy.Current.Courses;

        if (courses.Count == 0)
        {
            return 1;
        }

        return courses.Max(
            course => course.Id
        ) + 1;
    }

    private static void EnrollStudentDirectly(
        Course course,
        Student student)
    {
        bool alreadyEnrolled =
            course.Roster.Any(
                enrolledStudent =>
                    enrolledStudent.Id ==
                    student.Id
            );

        if (!alreadyEnrolled)
        {
            course.Roster.Add(student);
        }
    }
}