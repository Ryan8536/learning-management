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
            Console.WriteLine(
                "No courses have been added."
            );

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

        EnterCourseMenu(selectedCourse);
    }

    private void EnterCourseMenu(
        Course selectedCourse)
    {
        string? userChoice;

        do
        {
            Console.WriteLine();
            Console.WriteLine("--=========================--");
            Console.WriteLine("Teacher Course Menu:");
            Console.WriteLine("--=========================--");

            Console.WriteLine(
                $"Course: {selectedCourse.Name}"
            );

            Console.WriteLine(
                $"Code: {selectedCourse.Code}"
            );

            Console.WriteLine();
            Console.WriteLine(
                "1. Review and Grade Submissions"
            );

            Console.WriteLine(
                "2. Return to Teacher Menu"
            );

            userChoice = Console.ReadLine();

            if (userChoice == "1")
            {
                ReviewAndGradeSubmissions(
                    selectedCourse
                );
            }
            else if (userChoice != "2")
            {
                Console.WriteLine(
                    "Invalid selection. Please enter 1 or 2."
                );
            }

        } while (userChoice != "2");
    }

    private void ReviewAndGradeSubmissions(
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
            Console.WriteLine(
                $"ID: {assignment.Id} | " +
                $"Name: {assignment.Name} | " +
                $"Points: {assignment.AvailablePoints} | " +
                $"Submissions: {assignment.Submissions.Count}"
            );
        }

        Console.WriteLine();
        Console.Write(
            "Enter the assignment ID: "
        );

        string? assignmentIdText =
            Console.ReadLine();

        bool assignmentIdIsValid = int.TryParse(
            assignmentIdText,
            out int assignmentId
        );

        if (!assignmentIdIsValid)
        {
            Console.WriteLine(
                "The assignment ID must be a whole number."
            );

            return;
        }

        Assignment? selectedAssignment =
            selectedCourse.Assignments.FirstOrDefault(
                assignment =>
                    assignment.Id == assignmentId
            );

        if (selectedAssignment == null)
        {
            Console.WriteLine(
                "No assignment was found with that ID."
            );

            return;
        }

        if (selectedAssignment.Submissions.Count == 0)
        {
            Console.WriteLine(
                "This assignment has no submissions."
            );

            return;
        }

        DisplaySubmissions(selectedAssignment);

        Console.WriteLine();
        Console.Write(
            "Enter the submission ID to grade: "
        );

        string? submissionIdText =
            Console.ReadLine();

        bool submissionIdIsValid = int.TryParse(
            submissionIdText,
            out int submissionId
        );

        if (!submissionIdIsValid)
        {
            Console.WriteLine(
                "The submission ID must be a whole number."
            );

            return;
        }

        Submission? selectedSubmission =
            selectedAssignment.Submissions
                .FirstOrDefault(
                    submission =>
                        submission.Id == submissionId
                );

        if (selectedSubmission == null)
        {
            Console.WriteLine(
                "No submission was found with that ID."
            );

            return;
        }

        Console.WriteLine();
        Console.WriteLine("Selected Submission:");
        DisplaySubmission(selectedSubmission);

        Console.WriteLine();
        Console.Write(
            $"Enter grade from 0 to " +
            $"{selectedAssignment.AvailablePoints}: "
        );

        string? gradeText = Console.ReadLine();

        bool gradeIsValid = int.TryParse(
            gradeText,
            out int grade
        );

        if (!gradeIsValid)
        {
            Console.WriteLine(
                "The grade must be a whole number."
            );

            return;
        }

        bool gradeWasSaved =
            CourseServiceProxy.Current.GradeSubmission(
                selectedCourse.Id,
                selectedAssignment.Id,
                selectedSubmission.Id,
                grade
            );

        if (!gradeWasSaved)
        {
            Console.WriteLine(
                "The grade could not be saved. " +
                "Make sure it is within the available points."
            );

            return;
        }

        Console.WriteLine();
        Console.WriteLine(
            $"Submission {selectedSubmission.Id} " +
            $"was graded {selectedSubmission.Grade}/" +
            $"{selectedAssignment.AvailablePoints}."
        );
    }

    private void DisplaySubmissions(
        Assignment selectedAssignment)
    {
        Console.WriteLine();
        Console.WriteLine("Submissions:");

        foreach (
            Submission submission
            in selectedAssignment.Submissions)
        {
            DisplaySubmission(submission);
            Console.WriteLine();
        }
    }

    private void DisplaySubmission(
        Submission submission)
    {
        Student? student =
            StudentServiceProxy.Current.GetById(
                submission.StudentId
            );

        string studentName =
            student?.Name ?? "Unknown Student";

        string gradeDisplay =
            submission.Grade.HasValue
            ? submission.Grade.Value.ToString()
            : "Not graded";

        Console.WriteLine(
            $"Submission ID: {submission.Id}"
        );

        Console.WriteLine(
            $"Student: {studentName}"
        );

        Console.WriteLine(
            $"Content: {submission.Content}"
        );

        Console.WriteLine(
            $"Submitted: {submission.SubmissionDate:g}"
        );

        Console.WriteLine(
            $"Grade: {gradeDisplay}"
        );
    }
}