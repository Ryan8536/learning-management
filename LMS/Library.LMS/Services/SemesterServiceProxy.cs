using Library.LMS.Models;

namespace Library.LMS.Services;

public class SemesterServiceProxy
{
    private static SemesterServiceProxy? instance;

    private static readonly object instanceLock =
        new object();

    public static SemesterServiceProxy Current
    {
        get
        {
            lock (instanceLock)
            {
                instance ??=
                    new SemesterServiceProxy();

                return instance;
            }
        }
    }

    public List<Semester> Semesters { get; }

    private SemesterServiceProxy()
    {
        Semesters =
            new List<Semester>();
    }

    public Semester? AddSemester(
        string? name,
        DateTime startDate,
        DateTime endDate)
    {
        if (
            string.IsNullOrWhiteSpace(name)
            ||
            endDate.Date < startDate.Date
        )
        {
            return null;
        }

        int newSemesterId =
            Semesters.Count == 0
                ? 1
                : Semesters.Max(
                    semester =>
                        semester.Id
                ) + 1;

        Semester semester =
            new Semester
            {
                Id = newSemesterId,
                Name = name.Trim(),
                StartDate = startDate.Date,
                EndDate = endDate.Date
            };

        Semesters.Add(semester);

        return semester;
    }

    public bool UpdateSemester(
        int semesterId,
        string? name,
        DateTime startDate,
        DateTime endDate)
    {
        if (
            string.IsNullOrWhiteSpace(name)
            ||
            endDate.Date < startDate.Date
        )
        {
            return false;
        }

        Semester? semester =
            Semesters.FirstOrDefault(
                existingSemester =>
                    existingSemester.Id ==
                        semesterId
            );

        if (semester == null)
        {
            return false;
        }

        semester.Name =
            name.Trim();

        semester.StartDate =
            startDate.Date;

        semester.EndDate =
            endDate.Date;

        return true;
    }

    public bool DeleteSemester(
        int semesterId)
    {
        Semester? semester =
            Semesters.FirstOrDefault(
                existingSemester =>
                    existingSemester.Id ==
                        semesterId
            );

        if (semester == null)
        {
            return false;
        }

        Semesters.Remove(semester);

        return true;
    }
}