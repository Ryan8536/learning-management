using CLI.LMS.Helpers;

namespace CLI.LMS;

internal class Program
{
    static void Main(string[] args)
    {
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
}