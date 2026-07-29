using Api.LMS.Services;
using Library.LMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.LMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController :
    ControllerBase
{
    private static readonly object StudentsLock =
        new object();

    private static readonly List<Student> Students =
        StudentFileStore.LoadStudents();

    [HttpGet]
    public ActionResult<IEnumerable<Student>>
        GetStudents()
    {
        lock (StudentsLock)
        {
            List<Student> studentCopies =
                Students
                    .OrderBy(
                        student =>
                            student.Id
                    )
                    .ToList();

            return Ok(studentCopies);
        }
    }

    [HttpGet("{id:int}")]
    public ActionResult<Student> GetStudent(
        int id)
    {
        lock (StudentsLock)
        {
            Student? student =
                Students.FirstOrDefault(
                    existingStudent =>
                        existingStudent.Id == id
                );

            if (student == null)
            {
                return NotFound(
                    $"No student was found with ID {id}."
                );
            }

            return Ok(student);
        }
    }

    [HttpPost]
    public ActionResult<Student> AddStudent(
        Student student)
    {
        if (string.IsNullOrWhiteSpace(
            student.Name))
        {
            return BadRequest(
                "The student name is required."
            );
        }

        if (string.IsNullOrWhiteSpace(
            student.Code))
        {
            return BadRequest(
                "The student code is required."
            );
        }

        lock (StudentsLock)
        {
            bool codeAlreadyExists =
                Students.Any(
                    existingStudent =>
                        string.Equals(
                            existingStudent.Code,
                            student.Code.Trim(),
                            StringComparison
                                .OrdinalIgnoreCase
                        )
                );

            if (codeAlreadyExists)
            {
                return Conflict(
                    "A student already uses that code."
                );
            }

            student.Id =
                Students.Count == 0
                    ? 1
                    : Students.Max(
                        existingStudent =>
                            existingStudent.Id
                    ) + 1;

            student.Name =
                student.Name.Trim();

            student.Code =
                student.Code.Trim();

            student.Classification =
                student.Classification?.Trim();

            Students.Add(student);

            bool saved =
                StudentFileStore.SaveStudents(
                    Students
                );

            if (!saved)
            {
                Students.Remove(student);

                return StatusCode(
                    StatusCodes
                        .Status500InternalServerError,
                    "The student could not be saved."
                );
            }

            return CreatedAtAction(
                nameof(GetStudent),
                new
                {
                    id = student.Id
                },
                student
            );
        }
    }

    [HttpPut("{id:int}")]
    public ActionResult<Student> UpdateStudent(
        int id,
        Student updatedStudent)
    {
        if (string.IsNullOrWhiteSpace(
            updatedStudent.Name))
        {
            return BadRequest(
                "The student name is required."
            );
        }

        if (string.IsNullOrWhiteSpace(
            updatedStudent.Code))
        {
            return BadRequest(
                "The student code is required."
            );
        }

        lock (StudentsLock)
        {
            Student? existingStudent =
                Students.FirstOrDefault(
                    student =>
                        student.Id == id
                );

            if (existingStudent == null)
            {
                return NotFound(
                    $"No student was found with ID {id}."
                );
            }

            bool codeAlreadyExists =
                Students.Any(
                    student =>
                        student.Id != id
                        &&
                        string.Equals(
                            student.Code,
                            updatedStudent.Code.Trim(),
                            StringComparison
                                .OrdinalIgnoreCase
                        )
                );

            if (codeAlreadyExists)
            {
                return Conflict(
                    "A student already uses that code."
                );
            }

            string? originalName =
                existingStudent.Name;

            string? originalCode =
                existingStudent.Code;

            string? originalClassification =
                existingStudent.Classification;

            existingStudent.Name =
                updatedStudent.Name.Trim();

            existingStudent.Code =
                updatedStudent.Code.Trim();

            existingStudent.Classification =
                updatedStudent
                    .Classification?
                    .Trim();

            bool saved =
                StudentFileStore.SaveStudents(
                    Students
                );

            if (!saved)
            {
                existingStudent.Name =
                    originalName;

                existingStudent.Code =
                    originalCode;

                existingStudent.Classification =
                    originalClassification;

                return StatusCode(
                    StatusCodes
                        .Status500InternalServerError,
                    "The student changes could not be saved."
                );
            }

            return Ok(existingStudent);
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteStudent(
        int id)
    {
        lock (StudentsLock)
        {
            Student? student =
                Students.FirstOrDefault(
                    existingStudent =>
                        existingStudent.Id == id
                );

            if (student == null)
            {
                return NotFound(
                    $"No student was found with ID {id}."
                );
            }

            int originalIndex =
                Students.IndexOf(student);

            Students.Remove(student);

            bool saved =
                StudentFileStore.SaveStudents(
                    Students
                );

            if (!saved)
            {
                Students.Insert(
                    originalIndex,
                    student
                );

                return StatusCode(
                    StatusCodes
                        .Status500InternalServerError,
                    "The student deletion could not be saved."
                );
            }

            return NoContent();
        }
    }
}