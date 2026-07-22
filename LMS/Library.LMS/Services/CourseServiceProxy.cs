using Library.LMS.Models;

namespace Library.LMS.Services;

public class CourseServiceProxy
{
    private static CourseServiceProxy? instance;
    private static readonly object instanceLock = new object();

    public List<Course> Courses { get; private set; }

    private CourseServiceProxy()
    {
        Courses = new List<Course>();
    }

    public static CourseServiceProxy Current
    {
        get
        {
            lock (instanceLock)
            {
                if (instance == null)
                {
                    instance = new CourseServiceProxy();
                }
            }

            return instance;
        }
    }

    public void Add(Course? course)
    {
        if (course == null)
        {
            return;
        }

        course.Id = Courses.Count == 0
            ? 1
            : Courses.Max(course => course.Id) + 1;

        Courses.Add(course);
    }

    public Course? GetById(int id)
    {
        return Courses.FirstOrDefault(
            course => course.Id == id
        );
    }

    public void UpdateDescription(
        int id,
        string? newDescription)
    {
        Course? course = GetById(id);

        if (course == null)
        {
            return;
        }

        course.Description = newDescription;
    }

    public void AddModule(int courseId)
    {
        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        int newModuleId = course.Modules.Count == 0
            ? 1
            : course.Modules.Max(
                module => module.Id
            ) + 1;

        Module newModule = new Module
        {
            Id = newModuleId
        };

        course.Modules.Add(newModule);
    }

    public void AddModuleContent(
        int courseId,
        int moduleId,
        string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        Module? module = course.Modules.FirstOrDefault(
            module => module.Id == moduleId
        );

        if (module == null)
        {
            return;
        }

        module.Content.Add(content);
    }

    public void UpdateModuleContent(
        int courseId,
        int moduleId,
        string? oldContent,
        string? newContent)
    {
        if (string.IsNullOrWhiteSpace(oldContent))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(newContent))
        {
            return;
        }

        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        Module? module = course.Modules.FirstOrDefault(
            module => module.Id == moduleId
        );

        if (module == null)
        {
            return;
        }

        int contentIndex =
            module.Content.IndexOf(oldContent);

        if (contentIndex == -1)
        {
            return;
        }

        module.Content[contentIndex] = newContent;
    }

    public void RemoveModuleContent(
        int courseId,
        int moduleId,
        string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        Module? module = course.Modules.FirstOrDefault(
            module => module.Id == moduleId
        );

        if (module == null)
        {
            return;
        }

        module.Content.Remove(content);
    }

    public void AddAssignment(
        int courseId,
        string? name,
        string? description,
        int availablePoints,
        DateTime dueDate)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        int newAssignmentId =
            course.Assignments.Count == 0
            ? 1
            : course.Assignments.Max(
                assignment => assignment.Id
            ) + 1;

        Assignment newAssignment = new Assignment
        {
            Id = newAssignmentId,
            Name = name,
            Description = description,
            AvailablePoints = availablePoints,
            DueDate = dueDate
        };

        course.Assignments.Add(newAssignment);
    }

    public void UpdateAssignment(
        int courseId,
        int assignmentId,
        string? name,
        string? description,
        int availablePoints,
        DateTime dueDate)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        Assignment? assignment =
            course.Assignments.FirstOrDefault(
                assignment =>
                    assignment.Id == assignmentId
            );

        if (assignment == null)
        {
            return;
        }

        assignment.Name = name;
        assignment.Description = description;
        assignment.AvailablePoints = availablePoints;
        assignment.DueDate = dueDate;
    }

    public void DeleteAssignment(
        int courseId,
        int assignmentId)
    {
        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        Assignment? assignment =
            course.Assignments.FirstOrDefault(
                assignment =>
                    assignment.Id == assignmentId
            );

        if (assignment == null)
        {
            return;
        }

        assignment.Submissions.Clear();
        course.Assignments.Remove(assignment);
    }

    public void EnrollStudent(
        int courseId,
        Student? student)
    {
        if (student == null)
        {
            return;
        }

        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        bool studentIsAlreadyEnrolled =
            course.Roster.Any(
                enrolledStudent =>
                    enrolledStudent.Id == student.Id
            );

        if (studentIsAlreadyEnrolled)
        {
            return;
        }

        course.Roster.Add(student);
    }

    public Submission? AddSubmission(
        int courseId,
        int assignmentId,
        int studentId,
        string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        Course? course = GetById(courseId);

        if (course == null)
        {
            return null;
        }

        bool studentIsEnrolled =
            course.Roster.Any(
                student => student.Id == studentId
            );

        if (!studentIsEnrolled)
        {
            return null;
        }

        Assignment? assignment =
            course.Assignments.FirstOrDefault(
                assignment =>
                    assignment.Id == assignmentId
            );

        if (assignment == null)
        {
            return null;
        }

        int newSubmissionId =
            assignment.Submissions.Count == 0
            ? 1
            : assignment.Submissions.Max(
                submission => submission.Id
            ) + 1;

        Submission newSubmission = new Submission
        {
            Id = newSubmissionId,
            StudentId = studentId,
            AssignmentId = assignmentId,
            Content = content,
            SubmissionDate = DateTime.Now,
            Grade = null
        };

        assignment.Submissions.Add(newSubmission);

        return newSubmission;
    }

    public bool GradeSubmission(
        int courseId,
        int assignmentId,
        int submissionId,
        int grade)
    {
        Course? course = GetById(courseId);

        if (course == null)
        {
            return false;
        }

        Assignment? assignment =
            course.Assignments.FirstOrDefault(
                assignment =>
                    assignment.Id == assignmentId
            );

        if (assignment == null)
        {
            return false;
        }

        if (grade < 0 ||
            grade > assignment.AvailablePoints)
        {
            return false;
        }

        Submission? submission =
            assignment.Submissions.FirstOrDefault(
                submission =>
                    submission.Id == submissionId
            );

        if (submission == null)
        {
            return false;
        }

        submission.Grade = grade;

        return true;
    }

    public Course? Delete(int id)
    {
        Course? course = GetById(id);

        if (course != null)
        {
            Courses.Remove(course);
        }

        return course;
    }
}