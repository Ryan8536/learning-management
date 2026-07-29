using Api.LMS.Services;
using Library.LMS.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.LMS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController :
    ControllerBase
{
    private static readonly object CoursesLock =
        new object();

    private static readonly List<Course> Courses =
        CourseFileStore.LoadCourses();

    [HttpGet]
    public ActionResult<IEnumerable<Course>>
        GetCourses()
    {
        lock (CoursesLock)
        {
            return Ok(
                Courses
                    .OrderBy(
                        course =>
                            course.Id
                    )
                    .ToList()
            );
        }
    }

    [HttpGet("{id:int}")]
    public ActionResult<Course> GetCourse(
        int id)
    {
        lock (CoursesLock)
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
    }

    [HttpPost]
    public ActionResult<Course> AddCourse(
        Course course)
    {
        if (string.IsNullOrWhiteSpace(
            course.Name))
        {
            return BadRequest(
                "The course name is required."
            );
        }

        if (string.IsNullOrWhiteSpace(
            course.Code))
        {
            return BadRequest(
                "The course code is required."
            );
        }

        lock (CoursesLock)
        {
            course.Id =
                Courses.Count == 0
                    ? 1
                    : Courses.Max(
                        existingCourse =>
                            existingCourse.Id
                    ) + 1;

            course.Name =
                course.Name.Trim();

            course.Code =
                course.Code.Trim();

            course.Description =
                course.Description?.Trim();

            course.Semester =
                course.Semester?.Trim();

            course.Section =
                course.Section?.Trim();

            CourseFileStore.InitializeCourse(
                course
            );

            Courses.Add(course);

            bool saved =
                CourseFileStore.SaveCourses(
                    Courses
                );

            if (!saved)
            {
                Courses.Remove(course);

                return StatusCode(
                    StatusCodes
                        .Status500InternalServerError,
                    "The course could not be saved."
                );
            }

            return CreatedAtAction(
                nameof(GetCourse),
                new
                {
                    id = course.Id
                },
                course
            );
        }
    }

    [HttpPut("{id:int}")]
    public ActionResult<Course> UpdateCourse(
        int id,
        Course updatedCourse)
    {
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

        lock (CoursesLock)
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

            CourseFileStore.InitializeCourse(
                updatedCourse
            );

            int courseIndex =
                Courses.IndexOf(existingCourse);

            updatedCourse.Id =
                id;

            updatedCourse.Name =
                updatedCourse.Name.Trim();

            updatedCourse.Code =
                updatedCourse.Code.Trim();

            updatedCourse.Description =
                updatedCourse.Description?.Trim();

            updatedCourse.Semester =
                updatedCourse.Semester?.Trim();

            updatedCourse.Section =
                updatedCourse.Section?.Trim();

            Courses[courseIndex] =
                updatedCourse;

            bool saved =
                CourseFileStore.SaveCourses(
                    Courses
                );

            if (!saved)
            {
                Courses[courseIndex] =
                    existingCourse;

                return StatusCode(
                    StatusCodes
                        .Status500InternalServerError,
                    "The course changes could not be saved."
                );
            }

            return Ok(updatedCourse);
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteCourse(
        int id)
    {
        lock (CoursesLock)
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

            int originalIndex =
                Courses.IndexOf(course);

            Courses.Remove(course);

            bool saved =
                CourseFileStore.SaveCourses(
                    Courses
                );

            if (!saved)
            {
                Courses.Insert(
                    originalIndex,
                    course
                );

                return StatusCode(
                    StatusCodes
                        .Status500InternalServerError,
                    "The course deletion could not be saved."
                );
            }

            return NoContent();
        }
    }
}