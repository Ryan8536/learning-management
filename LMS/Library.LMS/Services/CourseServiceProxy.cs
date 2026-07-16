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
            : Courses.Max(c => c.Id) + 1;

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