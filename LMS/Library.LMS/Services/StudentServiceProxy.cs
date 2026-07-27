using Library.LMS.Models;

namespace Library.LMS.Services;

public class StudentServiceProxy
{
    private static StudentServiceProxy? instance;

    private static readonly object instanceLock =
        new object();

    public List<Student> Students
    {
        get;
        private set;
    }

    private StudentServiceProxy()
    {
        Students = new List<Student>();
    }

    public static StudentServiceProxy Current
    {
        get
        {
            lock (instanceLock)
            {
                if (instance == null)
                {
                    instance =
                        new StudentServiceProxy();
                }
            }

            return instance;
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

        Student? existingStudent =
            Students.FirstOrDefault(
                student =>
                    student.Code == code
            );

        if (existingStudent != null)
        {
            return existingStudent;
        }

        int newStudentId =
            Students.Count == 0
                ? 1
                : Students.Max(
                    student => student.Id
                ) + 1;

        Student newStudent =
            new Student
            {
                Id = newStudentId,
                Name = name.Trim(),
                Code = code.Trim(),
                Classification =
                    classification?.Trim()
            };

        Students.Add(newStudent);

        return newStudent;
    }

    public Student? GetById(int id)
    {
        return Students.FirstOrDefault(
            student =>
                student.Id == id
        );
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

        Student? selectedStudent =
            GetById(studentId);

        if (selectedStudent == null)
        {
            return false;
        }

        bool codeBelongsToAnotherStudent =
            Students.Any(
                student =>
                    student.Id != studentId
                    &&
                    student.Code == code.Trim()
            );

        if (codeBelongsToAnotherStudent)
        {
            return false;
        }

        selectedStudent.Name =
            name.Trim();

        selectedStudent.Code =
            code.Trim();

        selectedStudent.Classification =
            classification?.Trim();

        return true;
    }

    public bool Delete(int studentId)
    {
        Student? selectedStudent =
            GetById(studentId);

        if (selectedStudent == null)
        {
            return false;
        }

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

        Students.Remove(selectedStudent);

        return true;
    }
}