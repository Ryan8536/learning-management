using Library.LMS.Models;
using Library.LMS.Services;

namespace CLI.LMS.Helpers;

public class TeacherMenuHelper
{
    public void EnterMainMenu()
    {
        string? userChoice;

        do
        {
            Console.WriteLine();
            Console.WriteLine("--=========================--");
            Console.WriteLine("Teacher Main Menu:");
            Console.WriteLine("--=========================--");

            DisplayCourses();

            Console.WriteLine();
            Console.WriteLine("1. Add a New Course");
            Console.WriteLine("2. Select an Existing Course");
            Console.WriteLine("3. Return to Main Menu");

            userChoice = Console.ReadLine();

            if (userChoice == "1")
            {
                AddCourse();
            }
            else if (userChoice == "2")
            {
                SelectCourse();
            }
            else if (userChoice != "3")
            {
                Console.WriteLine(
                    "Invalid selection. Please enter 1, 2, or 3."
                );
            }

        } while (userChoice != "3");
    }

    private void DisplayCourses()
    {
        Console.WriteLine();
        Console.WriteLine("Existing Courses:");

        if (CourseServiceProxy.Current.Courses.Count == 0)
        {
            Console.WriteLine("No courses have been added.");
            return;
        }

        foreach (
            Course course
            in CourseServiceProxy.Current.Courses)
        {
            Console.WriteLine(
                $"ID: {course.Id} | " +
                $"Name: {course.Name} | " +
                $"Code: {course.Code}"
            );
        }
    }

    private void AddCourse()
    {
        Console.WriteLine();
        Console.WriteLine("Add a New Course");

        Console.Write("Course name: ");
        string? name = Console.ReadLine();

        Console.Write("Course code: ");
        string? code = Console.ReadLine();

        Console.Write("Course description: ");
        string? description = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine(
                "A course name is required."
            );

            return;
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            Console.WriteLine(
                "A course code is required."
            );

            return;
        }

        Course newCourse = new Course
        {
            Name = name,
            Code = code,
            Description = description
        };

        CourseServiceProxy.Current.Add(newCourse);

        Console.WriteLine();
        Console.WriteLine(
            $"Course added with ID {newCourse.Id}."
        );
    }

    private void SelectCourse()
    {
        if (CourseServiceProxy.Current.Courses.Count == 0)
        {
            Console.WriteLine();
            Console.WriteLine(
                "There are no courses to select."
            );

            return;
        }

        Console.WriteLine();
        Console.Write(
            "Enter the ID of the course to select: "
        );

        string? courseIdText = Console.ReadLine();

        bool idIsValid = int.TryParse(
            courseIdText,
            out int courseId
        );

        if (!idIsValid)
        {
            Console.WriteLine(
                "The course ID must be a whole number."
            );

            return;
        }

        Course? selectedCourse =
            CourseServiceProxy.Current.GetById(courseId);

        if (selectedCourse == null)
        {
            Console.WriteLine(
                "No course was found with that ID."
            );

            return;
        }

        Console.WriteLine();
        Console.WriteLine("Selected Course:");
        Console.WriteLine(
            $"ID: {selectedCourse.Id}"
        );
        Console.WriteLine(
            $"Name: {selectedCourse.Name}"
        );
        Console.WriteLine(
            $"Code: {selectedCourse.Code}"
        );
        Console.WriteLine(
            $"Description: {selectedCourse.Description}"
        );
    }
}