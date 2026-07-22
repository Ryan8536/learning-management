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

        EnterStudentMenu(selectedStudent);
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

    private void EnterStudentMenu(
        Student selectedStudent)
    {
        string? userChoice;

        do
        {
            Console.WriteLine();
            Console.WriteLine("--=========================--");
            Console.WriteLine("Student Main Menu:");
            Console.WriteLine("--=========================--");

            Console.WriteLine(
                $"Logged in as: {selectedStudent.Name}"
            );

            Console.WriteLine();
            Console.WriteLine("1. Select an Enrolled Course");
            Console.WriteLine("2. Return to Main Menu");

            userChoice = Console.ReadLine();

            if (userChoice == "1")
            {
                SelectCourse(selectedStudent);
            }
            else if (userChoice != "2")
            {
                Console.WriteLine(
                    "Invalid selection. Please enter 1 or 2."
                );
            }

        } while (userChoice != "2");
    }

    private void SelectCourse(
        Student selectedStudent)
    {
        List<Course> enrolledCourses =
            CourseServiceProxy.Current.Courses
                .Where(
                    course => course.Roster.Any(
                        student =>
                            student.Id ==
                            selectedStudent.Id
                    )
                )
                .ToList();

        Console.WriteLine();
        Console.WriteLine("Enrolled Courses:");

        if (enrolledCourses.Count == 0)
        {
            Console.WriteLine(
                "This student is not enrolled in any courses."
            );

            return;
        }

        foreach (Course course in enrolledCourses)
        {
            Console.WriteLine(
                $"ID: {course.Id} | " +
                $"Name: {course.Name} | " +
                $"Code: {course.Code}"
            );
        }

        Console.WriteLine();
        Console.Write(
            "Enter the ID of the course to open: "
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
            enrolledCourses.FirstOrDefault(
                course => course.Id == courseId
            );

        if (selectedCourse == null)
        {
            Console.WriteLine(
                "The selected student is not enrolled " +
                "in a course with that ID."
            );

            return;
        }

        EnterCourseMenu(
            selectedCourse,
            selectedStudent
        );
    }

    private void EnterCourseMenu(
        Course selectedCourse,
        Student selectedStudent)
    {
        string? userChoice;

        do
        {
            Console.WriteLine();
            Console.WriteLine("--=========================--");
            Console.WriteLine("Course Main Menu:");
            Console.WriteLine("--=========================--");

            Console.WriteLine(
                $"Course: {selectedCourse.Name}"
            );

            Console.WriteLine(
                $"Code: {selectedCourse.Code}"
            );

            Console.WriteLine();
            Console.WriteLine("1. View Modules and Content");
            Console.WriteLine("2. View Assignments");
            Console.WriteLine("3. View Other Students");
            Console.WriteLine("4. View Course Schedule");
            Console.WriteLine("5. Return to Student Menu");

            userChoice = Console.ReadLine();

            if (userChoice == "1")
            {
                DisplayModules(selectedCourse);
            }
            else if (userChoice == "2")
            {
                DisplayAssignments(selectedCourse);
            }
            else if (userChoice == "3")
            {
                DisplayOtherStudents(
                    selectedCourse,
                    selectedStudent
                );
            }
            else if (userChoice == "4")
            {
                DisplaySchedule(selectedCourse);
            }
            else if (userChoice != "5")
            {
                Console.WriteLine(
                    "Invalid selection. " +
                    "Please enter 1, 2, 3, 4, or 5."
                );
            }

        } while (userChoice != "5");
    }

    private void DisplayModules(
        Course selectedCourse)
    {
        Console.WriteLine();
        Console.WriteLine("Modules and Content:");

        if (selectedCourse.Modules.Count == 0)
        {
            Console.WriteLine(
                "This course has no modules."
            );

            return;
        }

        foreach (Module module in selectedCourse.Modules)
        {
            Console.WriteLine();
            Console.WriteLine(
                $"Module {module.Id}"
            );

            if (module.Content.Count == 0)
            {
                Console.WriteLine(
                    "No content has been added."
                );

                continue;
            }

            foreach (string content in module.Content)
            {
                Console.WriteLine(
                    $"- {content}"
                );
            }
        }
    }

    private void DisplayAssignments(
        Course selectedCourse)
    {
        Console.WriteLine();
        Console.WriteLine("Assignments:");

        if (selectedCourse.Assignments.Count == 0)
        {
            Console.WriteLine(
                "This course has no assignments."
            );

            return;
        }

        foreach (
            Assignment assignment
            in selectedCourse.Assignments)
        {
            Console.WriteLine();
            Console.WriteLine(
                $"ID: {assignment.Id}"
            );

            Console.WriteLine(
                $"Name: {assignment.Name}"
            );

            Console.WriteLine(
                $"Description: {assignment.Description}"
            );

            Console.WriteLine(
                $"Points: {assignment.AvailablePoints}"
            );

            Console.WriteLine(
                $"Due: {assignment.DueDate:MM/dd/yyyy}"
            );
        }
    }

    private void DisplayOtherStudents(
        Course selectedCourse,
        Student selectedStudent)
    {
        Console.WriteLine();
        Console.WriteLine("Other Students:");

        List<Student> otherStudents =
            selectedCourse.Roster
                .Where(
                    student =>
                        student.Id != selectedStudent.Id
                )
                .ToList();

        if (otherStudents.Count == 0)
        {
            Console.WriteLine(
                "There are no other students in this course."
            );

            return;
        }

        foreach (Student student in otherStudents)
        {
            Console.WriteLine(
                $"ID: {student.Id} | " +
                $"Name: {student.Name} | " +
                $"Code: {student.Code}"
            );
        }
    }

    private void DisplaySchedule(
        Course selectedCourse)
    {
        Console.WriteLine();
        Console.WriteLine("Course Schedule:");

        if (selectedCourse.Assignments.Count == 0)
        {
            Console.WriteLine(
                "There are no scheduled assignments."
            );

            return;
        }

        List<Assignment> scheduledAssignments =
            selectedCourse.Assignments
                .OrderBy(
                    assignment => assignment.DueDate
                )
                .ToList();

        foreach (
            Assignment assignment
            in scheduledAssignments)
        {
            Console.WriteLine(
                $"{assignment.DueDate:MM/dd/yyyy} - " +
                $"{assignment.Name}"
            );
        }
    }
}