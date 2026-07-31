using System.Net;
using System.Net.Http.Json;
using Library.LMS.Models;

namespace Library.LMS.Services;

public class StudentServiceProxy
{
    private static StudentServiceProxy? instance;

    private static readonly object instanceLock =
        new object();

    private readonly HttpClient httpClient;

    private const string StudentApiUrl =
        "http://localhost:5219/api/students";

    public List<Student> Students
    {
        get;
        private set;
    }

    private StudentServiceProxy()
    {
        httpClient =
            new HttpClient();

        Students =
            new List<Student>();
    }

    public static StudentServiceProxy Current
    {
        get
        {
            lock (instanceLock)
            {
                instance ??=
                    new StudentServiceProxy();

                return instance;
            }
        }
    }

    public async Task<bool> RefreshAsync()
    {
        try
        {
            List<Student>? studentsFromApi =
                await httpClient
                    .GetFromJsonAsync<List<Student>>(
                        StudentApiUrl
                    );

            Students =
                studentsFromApi
                ?? new List<Student>();

            return true;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }

    public bool Refresh()
    {
        try
        {
            List<Student>? studentsFromApi =
                httpClient
                    .GetFromJsonAsync<List<Student>>(
                        StudentApiUrl
                    )
                    .GetAwaiter()
                    .GetResult();

            Students =
                studentsFromApi
                ?? new List<Student>();

            return true;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }

    public Student? Add(
        string? name,
        string? code,
        string? classification)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        Student newStudent =
            new Student
            {
                Name = name.Trim(),
                Code = code.Trim(),
                Classification =
                    classification?.Trim()
            };

        try
        {
            HttpResponseMessage response =
                httpClient
                    .PostAsJsonAsync(
                        StudentApiUrl,
                        newStudent
                    )
                    .GetAwaiter()
                    .GetResult();

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            Student? addedStudent =
                response.Content
                    .ReadFromJsonAsync<Student>()
                    .GetAwaiter()
                    .GetResult();

            if (addedStudent == null)
            {
                return null;
            }

            Students.Add(addedStudent);

            return addedStudent;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (TaskCanceledException)
        {
            return null;
        }
    }

    public Student? GetById(
        int id)
    {
        Student? localStudent =
            Students.FirstOrDefault(
                student =>
                    student.Id == id
            );

        if (localStudent != null)
        {
            return localStudent;
        }

        try
        {
            HttpResponseMessage response =
                httpClient
                    .GetAsync(
                        $"{StudentApiUrl}/{id}"
                    )
                    .GetAwaiter()
                    .GetResult();

            if (
                response.StatusCode ==
                HttpStatusCode.NotFound
            )
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            Student? student =
                response.Content
                    .ReadFromJsonAsync<Student>()
                    .GetAwaiter()
                    .GetResult();

            if (student != null)
            {
                Students.Add(student);
            }

            return student;
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (TaskCanceledException)
        {
            return null;
        }
    }

    public bool Update(
        int studentId,
        string? name,
        string? code,
        string? classification)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        Student updatedStudent =
            new Student
            {
                Id = studentId,
                Name = name.Trim(),
                Code = code.Trim(),
                Classification =
                    classification?.Trim()
            };

        try
        {
            HttpResponseMessage response =
                httpClient
                    .PutAsJsonAsync(
                        $"{StudentApiUrl}/{studentId}",
                        updatedStudent
                    )
                    .GetAwaiter()
                    .GetResult();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            Student? savedStudent =
                response.Content
                    .ReadFromJsonAsync<Student>()
                    .GetAwaiter()
                    .GetResult();

            if (savedStudent == null)
            {
                return false;
            }

            Student? localStudent =
                Students.FirstOrDefault(
                    student =>
                        student.Id == studentId
                );

            if (localStudent == null)
            {
                Students.Add(savedStudent);
            }
            else
            {
                localStudent.Name =
                    savedStudent.Name;

                localStudent.Code =
                    savedStudent.Code;

                localStudent.Classification =
                    savedStudent.Classification;
            }

            return true;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }

    public bool Delete(
        int studentId)
    {
        try
        {
            HttpResponseMessage response =
                httpClient
                    .DeleteAsync(
                        $"{StudentApiUrl}/{studentId}"
                    )
                    .GetAwaiter()
                    .GetResult();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            Student? selectedStudent =
                Students.FirstOrDefault(
                    student =>
                        student.Id == studentId
                );

            if (selectedStudent != null)
            {
                Students.Remove(
                    selectedStudent
                );
            }

            RemoveStudentFromLocalCourses(
                studentId
            );

            return true;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }

    private static void RemoveStudentFromLocalCourses(
        int studentId)
    {
        foreach (
            Course course
            in CourseServiceProxy.Current.Courses)
        {
            course.Roster.RemoveAll(
                student =>
                    student.Id == studentId
            );

            foreach (
                Assignment assignment
                in course.Assignments)
            {
                assignment.Submissions.RemoveAll(
                    submission =>
                        submission.StudentId ==
                        studentId
                );
            }
        }
    }
}