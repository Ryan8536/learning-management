using Library.LMS.Models;
using Library.LMS.Services;

namespace CLI.LMS.Helpers;

public class StudentMenuHelper
{
    public void EnterMainMenu()
    {
        Student? selectedStudent = SelectStudent();

        if (selectedStudent == null)
        {
            return;
        }

        ShowStudentMenu(selectedStudent);
    }

    private Student? SelectStudent()
    {
        Console.WriteLine();
        Console.WriteLine("--=========================--");
        Console.WriteLine("Select a Student:");
        Console.WriteLine("--=========================--");

        if (StudentServiceProxy.Current.Students.Count == 0)
        {
            Console.WriteLine(
                "There are no students available."
            );

            return null;
        }

        foreach (
            Student student
            in StudentServiceProxy.Current.Students)
        {
            Console.WriteLine(
                $"ID: {student.Id} | " +
                $"Name: {student.Name} | " +
                $"Code: {student.Code}"
            );
        }

        Console.WriteLine();
        Console.Write(
            "Enter the ID of the student to use: "
        );

        string? studentIdText = Console.ReadLine();

        bool idIsValid = int.TryParse(
            studentIdText,
            out int studentId
        );

        if (!idIsValid)
        {
            Console.WriteLine(
                "The student ID must be a whole number."
            );

            return null;
        }

        Student? selectedStudent =
            StudentServiceProxy.Current.GetById(
                studentId
            );

        if (selectedStudent == null)
        {
            Console.WriteLine(
                "No student was found with that ID."
            );

            return null;
        }

        return selectedStudent;
    }

    private void ShowStudentMenu(
        Student selectedStudent)
    {
        Console.WriteLine();
        Console.WriteLine("--=========================--");
        Console.WriteLine("Student Main Menu:");
        Console.WriteLine("--=========================--");

        Console.WriteLine(
            $"Logged in as: {selectedStudent.Name}"
        );

        Console.WriteLine(
            $"Student Code: {selectedStudent.Code}"
        );

        Console.WriteLine(
            $"Classification: " +
            $"{selectedStudent.Classification}"
        );

        Console.WriteLine();
        Console.WriteLine(
            "Press Enter to return to the main menu."
        );

        Console.ReadLine();
    }
}