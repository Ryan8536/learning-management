using CLI.LMS.Helpers;
using Library.LMS.Models;
using Library.LMS.Services;

namespace CLI.LMS;

internal class Program
{
    static void Main(string[] args)
    {
        AddStarterData();

        string? userChoice;

        do
        {
            Console.WriteLine();
            Console.WriteLine(
                "Welcome to the Learning Management System"
            );

            Console.WriteLine(
                "Please select a user type:"
            );

            Console.WriteLine("1. Student");
            Console.WriteLine("2. Teacher");
            Console.WriteLine("3. Quit Application");

            userChoice = Console.ReadLine();

            if (userChoice == "1")
            {
                StudentMenuHelper studentMenu =
                    new StudentMenuHelper();

                studentMenu.EnterMainMenu();
            }
            else if (userChoice == "2")
            {
                TeacherMenuHelper teacherMenu =
                    new TeacherMenuHelper();

                teacherMenu.EnterMainMenu();
            }
            else if (userChoice != "3")
            {
                Console.WriteLine(
                    "Invalid selection. Please enter 1, 2, or 3."
                );
            }

        } while (userChoice != "3");

        Console.WriteLine(
            "Closing the Learning Management System."
        );
    }

    private static void AddStarterData()
    {
        Student? ryan =
            StudentServiceProxy.Current.Add(
                "Ryan Elharrada",
                "4804137076",
                "Junior"
            );

        Student? alex =
            StudentServiceProxy.Current.Add(
                "Alex Smith",
                "1000000001",
                "Sophomore"
            );

        if (CourseServiceProxy.Current.Courses.Count > 0)
        {
            return;
        }

        Course programmingCourse = new Course
        {
            Name = "Programming",
            Code = "COP4870",
            Semester = "Fall 2026",
            Section = "001",
            Description =
                "Full-stack application development"
        };

        CourseServiceProxy.Current.Add(
            programmingCourse
        );

        CourseServiceProxy.Current.EnrollStudent(
            programmingCourse.Id,
            ryan
        );

        CourseServiceProxy.Current.EnrollStudent(
            programmingCourse.Id,
            alex
        );

        Module programmingModule = new Module
        {
            Id = 1
        };

        programmingModule.Content.Add(
            new ModulePage
            {
                Id = 1,
                Name = "Introduction",
                Body = "Introduction to C#"
            }
        );

        programmingModule.Content.Add(
            new ModulePage
            {
                Id = 2,
                Name = "Application Structure",
                Body =
                    "Models, services, and user interfaces"
            }
        );

        programmingCourse.Modules.Add(
            programmingModule
        );

        Assignment programmingAssignment =
            new Assignment
            {
                Id = 1,
                Name = "LMS Milestone",
                Description =
                    "Complete the first LMS milestone.",
                AvailablePoints = 100,
                DueDate = DateTime.Today.AddDays(7)
            };

        programmingCourse.Assignments.Add(
            programmingAssignment
        );

        programmingModule.Content.Add(
            programmingAssignment
        );

        Course circuitsCourse = new Course
        {
            Name = "Circuits",
            Code = "EEL3003",
            Semester = "Fall 2026",
            Section = "001",
            Description =
                "Electrical circuit analysis"
        };

        CourseServiceProxy.Current.Add(
            circuitsCourse
        );

        CourseServiceProxy.Current.EnrollStudent(
            circuitsCourse.Id,
            ryan
        );

        Module circuitsModule = new Module
        {
            Id = 1
        };

        circuitsModule.Content.Add(
            new ModulePage
            {
                Id = 1,
                Name = "First-Order Circuits",
                Body = "First-order circuits"
            }
        );

        circuitsModule.Content.Add(
            new ModulePage
            {
                Id = 2,
                Name = "Second-Order Circuits",
                Body = "Second-order circuits"
            }
        );

        circuitsCourse.Modules.Add(
            circuitsModule
        );

        Assignment circuitsAssignment =
            new Assignment
            {
                Id = 1,
                Name = "Transient Circuit Homework",
                Description =
                    "Solve the assigned transient circuits.",
                AvailablePoints = 50,
                DueDate = DateTime.Today.AddDays(10)
            };

        circuitsCourse.Assignments.Add(
            circuitsAssignment
        );

        circuitsModule.Content.Add(
            circuitsAssignment
        );
    }
}