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

        Course programming =
            GetOrCreateCourse(
                "Programming",
                "COP3330",
                "Object-oriented programming",
                "Fall 2026",
                "001"
            );

        Course circuits =
            GetOrCreateCourse(
                "Circuits",
                "EEL3003",
                "Introduction to circuit analysis",
                "Fall 2026",
                "001"
            );

        AddProgrammingAssignment(
            programming
        );

        AddCircuitsAssignment(
            circuits
        );

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

            EnrollStudentDirectly(
                circuits,
                alex
            );
        }

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static Course GetOrCreateCourse(
        string name,
        string code,
        string description,
        string semester,
        string section)
    {
        Course? existingCourse =
            CourseServiceProxy.Current.Courses
                .FirstOrDefault(
                    course =>
                        course.Code == code
                );

        if (existingCourse != null)
        {
            return existingCourse;
        }

        Course newCourse =
            new Course
            {
                Id = GetNextCourseId(),
                Name = name,
                Code = code,
                Description = description,
                Semester = semester,
                Section = section
            };

        CourseServiceProxy.Current.Courses.Add(
            newCourse
        );

        return newCourse;
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

    private static void AddProgrammingAssignment(
        Course course)
    {
        bool assignmentExists =
            course.Assignments.Any(
                assignment =>
                    assignment.Name ==
                    "Programming Exercise 1"
            );

        if (assignmentExists)
        {
            return;
        }

        course.Assignments.Add(
            new Assignment
            {
                Id = GetNextAssignmentId(course),
                Name = "Programming Exercise 1",
                Description =
                    "Describe how classes and objects work.",
                AvailablePoints = 100,
                DueDate = DateTime.Today.AddDays(7)
            }
        );
    }

    private static void AddCircuitsAssignment(
        Course course)
    {
        bool assignmentExists =
            course.Assignments.Any(
                assignment =>
                    assignment.Name ==
                    "Circuit Analysis Response"
            );

        if (assignmentExists)
        {
            return;
        }

        course.Assignments.Add(
            new Assignment
            {
                Id = GetNextAssignmentId(course),
                Name = "Circuit Analysis Response",
                Description =
                    "Explain the difference between series and parallel circuits.",
                AvailablePoints = 50,
                DueDate = DateTime.Today.AddDays(10)
            }
        );
    }

    private static int GetNextAssignmentId(
        Course course)
    {
        if (course.Assignments.Count == 0)
        {
            return 1;
        }

        return course.Assignments.Max(
            assignment => assignment.Id
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