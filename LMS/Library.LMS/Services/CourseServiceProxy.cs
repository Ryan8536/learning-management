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
            : course.Modules.Max(module => module.Id) + 1;

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

        int contentIndex = module.Content.IndexOf(oldContent);

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

        int newAssignmentId = course.Assignments.Count == 0
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