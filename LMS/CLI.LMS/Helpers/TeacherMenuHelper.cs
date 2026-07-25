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

        Console.Write("Course semester: ");
        string? semester = Console.ReadLine();

        Console.Write("Course section: ");
        string? section = Console.ReadLine();

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

        if (string.IsNullOrWhiteSpace(semester))
        {
            Console.WriteLine(
                "A course semester is required."
            );

            return;
        }

        Course newCourse = new Course
        {
            Name = name,
            Code = code,
            Semester = semester,
            Section = section,
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
                "2. Unenroll a Student"
            );

            Console.WriteLine(
                "3. Return to Teacher Menu"
            );

            userChoice = Console.ReadLine();

            if (userChoice == "1")
            {
                ReviewAndGradeSubmissions(
                    selectedCourse
                );
            }
            else if (userChoice == "2")
            {
                UnenrollStudent(selectedCourse);
            }
            else if (userChoice != "3")
            {
                Console.WriteLine(
                    "Invalid selection. Please enter 1, 2, or 3."
                );
            }

        } while (userChoice != "3");
    }

    private void UnenrollStudent(
        Course selectedCourse)
    {
        Console.WriteLine();
        Console.WriteLine("Course Roster:");

        if (selectedCourse.Roster.Count == 0)
        {
            Console.WriteLine(
                "There are no students enrolled."
            );

            return;
        }

        foreach (
            Student student
            in selectedCourse.Roster)
        {
            Console.WriteLine(
                $"ID: {student.Id} | " +
                $"Name: {student.Name} | " +
                $"Code: {student.Code}"
            );
        }

        Console.WriteLine();
        Console.Write(
            "Enter the student ID to unenroll: "
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

            return;
        }

        bool studentWasRemoved =
            CourseServiceProxy.Current.UnenrollStudent(
                selectedCourse.Id,
                studentId
            );

        if (!studentWasRemoved)
        {
            Console.WriteLine(
                "No enrolled student was found with that ID."
            );

            return;
        }

        Console.WriteLine(
            "The student was unenrolled from the course."
        );
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

        DisplaySubmission(
            selectedSubmission,
            selectedAssignment
        );

        Console.WriteLine();
        Console.WriteLine("Choose a grading method:");
        Console.WriteLine("1. Grade using points");
        Console.WriteLine("2. Grade using percentage");

        string? gradingMethod =
            Console.ReadLine();

        double gradeInPoints;

        if (gradingMethod == "1")
        {
            Console.Write(
                $"Enter points from 0 to " +
                $"{selectedAssignment.AvailablePoints}: "
            );

            string? pointsText =
                Console.ReadLine();

            bool pointsAreValid =
                double.TryParse(
                    pointsText,
                    out gradeInPoints
                );

            if (!pointsAreValid)
            {
                Console.WriteLine(
                    "The grade must be a number."
                );

                return;
            }
        }
        else if (gradingMethod == "2")
        {
            Console.Write(
                "Enter percentage from 0 to 100: "
            );

            string? percentageText =
                Console.ReadLine();

            bool percentageIsValid =
                double.TryParse(
                    percentageText,
                    out double percentage
                );

            if (!percentageIsValid)
            {
                Console.WriteLine(
                    "The percentage must be a number."
                );

                return;
            }

            if (
                percentage < 0
                ||
                percentage > 100
            )
            {
                Console.WriteLine(
                    "The percentage must be between 0 and 100."
                );

                return;
            }

            gradeInPoints =
                selectedAssignment.AvailablePoints
                * percentage
                / 100;
        }
        else
        {
            Console.WriteLine(
                "Invalid grading method."
            );

            return;
        }

        Console.Write(
            "Enter feedback for the student: "
        );

        string? feedback =
            Console.ReadLine();

        bool gradeWasSaved =
            CourseServiceProxy.Current.GradeSubmission(
                selectedCourse.Id,
                selectedAssignment.Id,
                selectedSubmission.Id,
                gradeInPoints,
                feedback
            );

        if (!gradeWasSaved)
        {
            Console.WriteLine(
                "The grade could not be saved. " +
                "Make sure it is within the available points."
            );

            return;
        }

        double percentageGrade = 0;

        if (selectedAssignment.AvailablePoints > 0)
        {
            percentageGrade =
                gradeInPoints
                / selectedAssignment.AvailablePoints
                * 100;
        }

        Console.WriteLine();
        Console.WriteLine(
            $"Submission {selectedSubmission.Id} " +
            $"was graded {selectedSubmission.Grade:0.##}/" +
            $"{selectedAssignment.AvailablePoints} " +
            $"({percentageGrade:0.##}%)."
        );

        Console.WriteLine(
            $"Feedback: {selectedSubmission.Feedback}"
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
            DisplaySubmission(
                submission,
                selectedAssignment
            );

            Console.WriteLine();
        }
    }

    private void DisplaySubmission(
        Submission submission,
        Assignment assignment)
    {
        Student? student =
            StudentServiceProxy.Current.GetById(
                submission.StudentId
            );

        string studentName =
            student?.Name ?? "Unknown Student";

        string gradeDisplay;

        if (submission.Grade.HasValue)
        {
            double percentageGrade = 0;

            if (assignment.AvailablePoints > 0)
            {
                percentageGrade =
                    submission.Grade.Value
                    / assignment.AvailablePoints
                    * 100;
            }

            gradeDisplay =
                $"{submission.Grade.Value:0.##}/" +
                $"{assignment.AvailablePoints} " +
                $"({percentageGrade:0.##}%)";
        }
        else
        {
            gradeDisplay =
                "Not graded";
        }

        string feedbackDisplay =
            string.IsNullOrWhiteSpace(
                submission.Feedback
            )
            ? "No feedback"
            : submission.Feedback;

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

        Console.WriteLine(
            $"Feedback: {feedbackDisplay}"
        );
    }
}