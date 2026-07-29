using System.Text.Json;
using Library.LMS.Models;

namespace Api.LMS.Services;

public static class CourseFileStore
{
    private static readonly object FileLock =
        new object();

    private static readonly JsonSerializerOptions
        JsonOptions =
            new JsonSerializerOptions
            {
                WriteIndented = true
            };

    private static readonly string DataDirectory =
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder
                    .LocalApplicationData
            ),
            "LMSApi"
        );

    private static readonly string CourseFilePath =
        Path.Combine(
            DataDirectory,
            "courses.json"
        );

    public static List<Course> LoadCourses()
    {
        lock (FileLock)
        {
            Directory.CreateDirectory(
                DataDirectory
            );

            if (!File.Exists(CourseFilePath))
            {
                return new List<Course>();
            }

            try
            {
                string json =
                    File.ReadAllText(
                        CourseFilePath
                    );

                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<Course>();
                }

                List<Course> courses =
                    JsonSerializer.Deserialize<
                        List<Course>
                    >(
                        json,
                        JsonOptions
                    )
                    ?? new List<Course>();

                foreach (Course course in courses)
                {
                    InitializeCourse(course);
                    RelinkCourseAssignments(course);
                }

                return courses;
            }
            catch (IOException)
            {
                return new List<Course>();
            }
            catch (JsonException)
            {
                return new List<Course>();
            }
            catch (NotSupportedException)
            {
                return new List<Course>();
            }
        }
    }

    public static bool SaveCourses(
        IEnumerable<Course> courses)
    {
        lock (FileLock)
        {
            try
            {
                Directory.CreateDirectory(
                    DataDirectory
                );

                string json =
                    JsonSerializer.Serialize(
                        courses,
                        JsonOptions
                    );

                File.WriteAllText(
                    CourseFilePath,
                    json
                );

                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (JsonException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }
    }

    public static void InitializeCourse(
        Course course)
    {
        course.Roster ??=
            new List<Student>();

        course.Modules ??=
            new List<Module>();

        course.Assignments ??=
            new List<Assignment>();

        course.AssignmentGroups ??=
            new List<AssignmentGroup>();

        course.Announcements ??=
            new List<Announcement>();

        foreach (Module module in course.Modules)
        {
            module.Content ??=
                new List<ModuleItem>();
        }

        foreach (
            Assignment assignment
            in course.Assignments
        )
        {
            assignment.Submissions ??=
                new List<Submission>();
        }

        foreach (
            AssignmentGroup group
            in course.AssignmentGroups
        )
        {
            group.Assignments ??=
                new List<Assignment>();
        }
    }

    private static void RelinkCourseAssignments(
        Course course)
    {
        Dictionary<int, Assignment>
            assignmentsById =
                course.Assignments
                    .GroupBy(
                        assignment =>
                            assignment.Id
                    )
                    .ToDictionary(
                        group =>
                            group.Key,
                        group =>
                            group.First()
                    );

        foreach (Module module in course.Modules)
        {
            for (
                int index = 0;
                index < module.Content.Count;
                index++
            )
            {
                if (
                    module.Content[index]
                    is not Assignment assignment
                )
                {
                    continue;
                }

                if (
                    assignmentsById.TryGetValue(
                        assignment.Id,
                        out Assignment?
                            canonicalAssignment
                    )
                )
                {
                    module.Content[index] =
                        canonicalAssignment;
                }
            }
        }

        foreach (
            AssignmentGroup group
            in course.AssignmentGroups
        )
        {
            List<Assignment>
                linkedAssignments =
                    new List<Assignment>();

            foreach (
                Assignment assignment
                in group.Assignments
            )
            {
                if (
                    assignmentsById.TryGetValue(
                        assignment.Id,
                        out Assignment?
                            canonicalAssignment
                    )
                )
                {
                    linkedAssignments.Add(
                        canonicalAssignment
                    );
                }
            }

            group.Assignments =
                linkedAssignments;
        }
    }
}