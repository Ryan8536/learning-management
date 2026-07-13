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

        Courses.Add(course);
    }
}