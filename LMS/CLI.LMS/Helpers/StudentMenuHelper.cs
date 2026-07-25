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
            Console.WriteLine(
                "1. Select an Enrolled Course"
            );

            Console.WriteLine(
                "2. Return to Main Menu"
            );

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

        foreach (
            Course course
            in enrolledCourses)
        {
            Console.WriteLine(
                $"ID: {course.Id} | " +
                $"Name: {course.Name} | " +
                $"Code: {course.Code} | " +
                $"Semester: {course.Semester} | " +
                $"Section: {course.Section}"
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
            Console.WriteLine(
                "1. View Modules and Content"
            );

            Console.WriteLine(
                "2. View Assignments"
            );

            Console.WriteLine(
                "3. View Other Students"
            );

            Console.WriteLine(
                "4. View Course Schedule"
            );

            Console.WriteLine(
                "5. Submit an Assignment"
            );

            Console.WriteLine(
                "6. View Grades"
            );

            Console.WriteLine(
                "7. Unenroll From This Course"
            );

            Console.WriteLine(
                "8. Return to Student Menu"
            );

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
            else if (userChoice == "5")
            {
                SubmitAssignment(
                    selectedCourse,
                    selectedStudent
                );
            }
            else if (userChoice == "6")
            {
                DisplayGrades(
                    selectedCourse,
                    selectedStudent
                );
            }
            else if (userChoice == "7")
            {
                bool wasUnenrolled =
                    UnenrollFromCourse(
                        selectedCourse,
                        selectedStudent
                    );

                if (wasUnenrolled)
                {
                    return;
                }
            }
            else if (userChoice != "8")
            {
                Console.WriteLine(
                    "Invalid selection. " +
                    "Please enter 1, 2, 3, 4, 5, 6, 7, or 8."
                );
            }

        } while (userChoice != "8");
    }

    private bool UnenrollFromCourse(
        Course selectedCourse,
        Student selectedStudent)
    {
        Console.WriteLine();
        Console.Write(
            $"Unenroll from {selectedCourse.Name}? " +
            "Enter Y to confirm: "
        );

        string? confirmation = Console.ReadLine();

        if (!string.Equals(
            confirmation,
            "Y",
            StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine(
                "Unenrollment was cancelled."
            );

            return false;
        }

        bool studentWasRemoved =
            CourseServiceProxy.Current.UnenrollStudent(
                selectedCourse.Id,
                selectedStudent.Id
            );

        if (!studentWasRemoved)
        {
            Console.WriteLine(
                "The student could not be unenrolled."
            );

            return false;
        }

        Console.WriteLine(
            "You were unenrolled from the course."
        );

        return true;
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

        foreach (
            Module module
            in selectedCourse.Modules)
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

            foreach (
                ModuleItem item
                in module.Content)
            {
                Console.WriteLine(
                    $"- {item.DisplayText}"
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

    private void SubmitAssignment(
        Course selectedCourse,
        Student selectedStudent)
    {
        Console.WriteLine();
        Console.WriteLine("Submit an Assignment:");

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
                $"Due: {assignment.DueDate:MM/dd/yyyy}"
            );
        }

        Console.WriteLine();
        Console.Write(
            "Enter the assignment ID: "
        );

        string? assignmentIdText =
            Console.ReadLine();

        bool idIsValid = int.TryParse(
            assignmentIdText,
            out int assignmentId
        );

        if (!idIsValid)
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

        Console.Write(
            "Enter your submission content: "
        );

        string? submissionContent =
            Console.ReadLine();

        if (string.IsNullOrWhiteSpace(
            submissionContent))
        {
            Console.WriteLine(
                "Submission content is required."
            );

            return;
        }

        Submission? newSubmission =
            CourseServiceProxy.Current.AddSubmission(
                selectedCourse.Id,
                selectedAssignment.Id,
                selectedStudent.Id,
                submissionContent
            );

        if (newSubmission == null)
        {
            Console.WriteLine(
                "The submission could not be added."
            );

            return;
        }

        Console.WriteLine();
        Console.WriteLine(
            $"Submission {newSubmission.Id} was added."
        );

        Console.WriteLine(
            $"Submitted: " +
            $"{newSubmission.SubmissionDate:g}"
        );
    }

    private void DisplayGrades(
        Course selectedCourse,
        Student selectedStudent)
    {
        Console.WriteLine();
        Console.WriteLine("--=========================--");
        Console.WriteLine("Course Grades:");
        Console.WriteLine("--=========================--");

        Console.WriteLine(
            $"Course: {selectedCourse.Name}"
        );

        Console.WriteLine(
            $"Student: {selectedStudent.Name}"
        );

        if (selectedCourse.Assignments.Count == 0)
        {
            Console.WriteLine();
            Console.WriteLine(
                "This course has no assignments."
            );

            return;
        }

        bool atLeastOneGradeWasFound = false;

        foreach (
            Assignment assignment
            in selectedCourse.Assignments)
        {
            Submission? gradedSubmission =
                assignment.Submissions
                    .Where(
                        submission =>
                            submission.StudentId
                                == selectedStudent.Id
                            &&
                            submission.Grade.HasValue
                    )
                    .OrderByDescending(
                        submission =>
                            submission.SubmissionDate
                    )
                    .FirstOrDefault();

            Console.WriteLine();
            Console.WriteLine(
                $"Assignment: {assignment.Name}"
            );

            Console.WriteLine(
                $"Available Points: " +
                $"{assignment.AvailablePoints}"
            );

            if (gradedSubmission == null)
            {
                Console.WriteLine(
                    "Grade: Not graded"
                );

                Console.WriteLine(
                    "Feedback: No feedback"
                );

                continue;
            }

            atLeastOneGradeWasFound = true;

            double earnedPoints =
                gradedSubmission.Grade ?? 0;

            double percentage = 0;

            if (assignment.AvailablePoints > 0)
            {
                percentage =
                    earnedPoints
                    / assignment.AvailablePoints
                    * 100;
            }

            Console.WriteLine(
                $"Grade: {earnedPoints:0.##}/" +
                $"{assignment.AvailablePoints} " +
                $"({percentage:0.##}%)"
            );

            string feedbackDisplay =
                string.IsNullOrWhiteSpace(
                    gradedSubmission.Feedback
                )
                ? "No feedback"
                : gradedSubmission.Feedback;

            Console.WriteLine(
                $"Feedback: {feedbackDisplay}"
            );
        }

        Console.WriteLine();

        double? courseGrade =
            CourseServiceProxy.Current
                .CalculateCourseGrade(
                    selectedCourse.Id,
                    selectedStudent.Id
                );

        if (courseGrade.HasValue)
        {
            Console.WriteLine(
                $"Course Average: " +
                $"{courseGrade.Value:0.##}%"
            );
        }
        else if (atLeastOneGradeWasFound)
        {
            double totalEarnedPoints = 0;
            double totalAvailablePoints = 0;

            foreach (
                Assignment assignment
                in selectedCourse.Assignments)
            {
                Submission? gradedSubmission =
                    assignment.Submissions
                        .Where(
                            submission =>
                                submission.StudentId
                                    == selectedStudent.Id
                                &&
                                submission.Grade.HasValue
                        )
                        .OrderByDescending(
                            submission =>
                                submission.SubmissionDate
                        )
                        .FirstOrDefault();

                if (gradedSubmission == null)
                {
                    continue;
                }

                if (assignment.AvailablePoints <= 0)
                {
                    continue;
                }

                totalEarnedPoints +=
                    gradedSubmission.Grade ?? 0;

                totalAvailablePoints +=
                    assignment.AvailablePoints;
            }

            if (totalAvailablePoints > 0)
            {
                double unweightedAverage =
                    totalEarnedPoints
                    / totalAvailablePoints
                    * 100;

                Console.WriteLine(
                    $"Course Average: " +
                    $"{unweightedAverage:0.##}%"
                );
            }
        }
        else
        {
            Console.WriteLine(
                "Course Average: No graded work"
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

        foreach (
            Student student
            in otherStudents)
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