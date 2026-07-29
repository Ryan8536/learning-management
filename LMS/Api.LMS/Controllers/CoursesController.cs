using Library.LMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.LMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private static readonly List<Course> Courses =
        new List<Course>();

    [HttpGet]
    public ActionResult<IEnumerable<Course>> GetCourses()
    {
        return Ok(Courses);
    }

    [HttpGet("{id:int}")]
    public ActionResult<Course> GetCourse(
        int id)
    {
        Course? course =
            Courses.FirstOrDefault(
                existingCourse =>
                    existingCourse.Id == id
            );

        if (course == null)
        {
            return NotFound(
                $"No course was found with ID {id}."
            );
        }

        return Ok(course);
    }

    [HttpPost]
    public ActionResult<Course> AddCourse(
        Course course)
    {
        if (string.IsNullOrWhiteSpace(course.Name))
        {
            return BadRequest(
                "The course name is required."
            );
        }

        if (string.IsNullOrWhiteSpace(course.Code))
        {
            return BadRequest(
                "The course code is required."
            );
        }

        course.Id =
            Courses.Count == 0
                ? 1
                : Courses.Max(
                    existingCourse =>
                        existingCourse.Id
                ) + 1;

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

        Courses.Add(course);

        return CreatedAtAction(
            nameof(GetCourse),
            new
            {
                id = course.Id
            },
            course
        );
    }

    [HttpPut("{id:int}")]
    public ActionResult<Course> UpdateCourse(
        int id,
        Course updatedCourse)
    {
        Course? existingCourse =
            Courses.FirstOrDefault(
                course =>
                    course.Id == id
            );

        if (existingCourse == null)
        {
            return NotFound(
                $"No course was found with ID {id}."
            );
        }

        if (string.IsNullOrWhiteSpace(
            updatedCourse.Name))
        {
            return BadRequest(
                "The course name is required."
            );
        }

        if (string.IsNullOrWhiteSpace(
            updatedCourse.Code))
        {
            return BadRequest(
                "The course code is required."
            );
        }

        existingCourse.Name =
            updatedCourse.Name;

        existingCourse.Code =
            updatedCourse.Code;

        existingCourse.Description =
            updatedCourse.Description;

        existingCourse.Semester =
            updatedCourse.Semester;

        existingCourse.Section =
            updatedCourse.Section;

        existingCourse.MinimumAPercentage =
            updatedCourse.MinimumAPercentage;

        existingCourse.MinimumBPercentage =
            updatedCourse.MinimumBPercentage;

        existingCourse.MinimumCPercentage =
            updatedCourse.MinimumCPercentage;

        existingCourse.MinimumDPercentage =
            updatedCourse.MinimumDPercentage;

        return Ok(existingCourse);
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteCourse(
        int id)
    {
        Course? course =
            Courses.FirstOrDefault(
                existingCourse =>
                    existingCourse.Id == id
            );

        if (course == null)
        {
            return NotFound(
                $"No course was found with ID {id}."
            );
        }

        Courses.Remove(course);

        return NoContent();
    }
}