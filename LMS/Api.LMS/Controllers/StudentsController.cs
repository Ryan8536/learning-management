using Library.LMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.LMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private static readonly List<Student> Students =
        new List<Student>();

    [HttpGet]
    public ActionResult<IEnumerable<Student>> GetStudents()
    {
        return Ok(Students);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Student> GetStudent(
        int id)
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

    [HttpPost]
    public ActionResult<Student> AddStudent(
        Student student)
    {
        if (string.IsNullOrWhiteSpace(student.Name))
        {
            return BadRequest(
                "The student name is required."
            );
        }

        student.Id =
            Students.Count == 0
                ? 1
                : Students.Max(
                    existingStudent =>
                        existingStudent.Id
                ) + 1;

        Students.Add(student);

        return CreatedAtAction(
            nameof(GetStudent),
            new
            {
                id = student.Id
            },
            student
        );
    }

    [HttpPut("{id:int}")]
    public ActionResult<Student> UpdateStudent(
        int id,
        Student updatedStudent)
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

        if (string.IsNullOrWhiteSpace(
            updatedStudent.Name))
        {
            return BadRequest(
                "The student name is required."
            );
        }

        existingStudent.Name =
            updatedStudent.Name;

        existingStudent.Code =
            updatedStudent.Code;

        existingStudent.Classification =
            updatedStudent.Classification;

        return Ok(existingStudent);
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteStudent(
        int id)
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

        Students.Remove(student);

        return NoContent();
    }
}