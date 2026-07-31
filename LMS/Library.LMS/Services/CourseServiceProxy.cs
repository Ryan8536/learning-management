using System.Net;
using System.Net.Http.Json;
using Library.LMS.Models;

namespace Library.LMS.Services;

public class CourseServiceProxy
{
    private static CourseServiceProxy? instance;
    private static readonly object instanceLock = new object();

    private readonly HttpClient httpClient;

    private const string CourseApiUrl =
        "http://localhost:5219/api/courses";

    public List<Course> Courses { get; private set; }

    private CourseServiceProxy()
    {
        httpClient =
            new HttpClient();

        Courses =
            new List<Course>();

        Refresh();
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


    public async Task<bool> RefreshAsync()
{
    try
    {
        List<Course>? coursesFromApi =
            await httpClient
                .GetFromJsonAsync<List<Course>>(
                    CourseApiUrl
                );

        Courses =
            coursesFromApi
            ?? new List<Course>();

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
            List<Course>? coursesFromApi =
                httpClient
                    .GetFromJsonAsync<List<Course>>(
                        CourseApiUrl
                    )
                    .GetAwaiter()
                    .GetResult();

            Courses =
                coursesFromApi
                ?? new List<Course>();

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
        catch (NotSupportedException)
        {
            return false;
        }
    }

    public void Add(Course? course)
    {
        if (course == null)
        {
            return;
        }

        try
        {
            HttpResponseMessage response =
                httpClient
                    .PostAsJsonAsync(
                        CourseApiUrl,
                        course
                    )
                    .GetAwaiter()
                    .GetResult();

            if (!response.IsSuccessStatusCode)
            {
                return;
            }

            Course? savedCourse =
                response.Content
                    .ReadFromJsonAsync<Course>()
                    .GetAwaiter()
                    .GetResult();

            if (savedCourse == null)
            {
                return;
            }

            Courses.Add(savedCourse);
        }
        catch (HttpRequestException)
        {
        }
        catch (TaskCanceledException)
        {
        }
        catch (NotSupportedException)
        {
        }
    }

    public Course? GetById(int id)
    {
        return Courses.FirstOrDefault(
            course => course.Id == id
        );
    }

    public bool SaveCourse(
        int courseId)
    {
        Course? course =
            GetById(courseId);

        if (course == null)
        {
            return false;
        }

        try
        {
            HttpResponseMessage response =
                httpClient
                    .PutAsJsonAsync(
                        $"{CourseApiUrl}/{courseId}",
                        course
                    )
                    .GetAwaiter()
                    .GetResult();

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            Course? savedCourse =
                response.Content
                    .ReadFromJsonAsync<Course>()
                    .GetAwaiter()
                    .GetResult();

            if (savedCourse == null)
            {
                return false;
            }

            int existingIndex =
                Courses.FindIndex(
                    existingCourse =>
                        existingCourse.Id ==
                            courseId
                );

            if (existingIndex >= 0)
            {
                Courses[existingIndex] =
                    savedCourse;
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
        catch (NotSupportedException)
        {
            return false;
        }
    }

    public void Delete(
        int courseId)
    {
        try
        {
            HttpResponseMessage response =
                httpClient
                    .DeleteAsync(
                        $"{CourseApiUrl}/{courseId}"
                    )
                    .GetAwaiter()
                    .GetResult();

            if (
                !response.IsSuccessStatusCode
                &&
                response.StatusCode !=
                    HttpStatusCode.NotFound
            )
            {
                return;
            }

            Course? courseToDelete =
                GetById(courseId);

            if (courseToDelete != null)
            {
                Courses.Remove(
                    courseToDelete
                );
            }
        }
        catch (HttpRequestException)
        {
        }
        catch (TaskCanceledException)
        {
        }
    }

public Course? CopyCourse(int courseId)
{
    Course? originalCourse =
        GetById(courseId);

    if (originalCourse == null)
    {
        return null;
    }

    Course copiedCourse = new Course
    {
        Name = $"{originalCourse.Name} - Copy",
        Code = originalCourse.Code,
        Description = originalCourse.Description,
        Semester = originalCourse.Semester,
        Section = originalCourse.Section
    };

    Dictionary<int, Assignment>
        copiedAssignmentsById =
            new Dictionary<int, Assignment>();

    foreach (
        Assignment originalAssignment
        in originalCourse.Assignments)
    {
        Assignment copiedAssignment =
            new Assignment
            {
                Id = originalAssignment.Id,
                Name = originalAssignment.Name,
                Description =
                    originalAssignment.Description,
                AvailablePoints =
                    originalAssignment.AvailablePoints,
                DueDate =
                    originalAssignment.DueDate
            };

        copiedCourse.Assignments.Add(
            copiedAssignment
        );

        copiedAssignmentsById.Add(
            originalAssignment.Id,
            copiedAssignment
        );
    }

    foreach (
        Module originalModule
        in originalCourse.Modules)
    {
        Module copiedModule = new Module
        {
            Id = originalModule.Id
        };

        foreach (
            ModuleItem originalItem
            in originalModule.Content)
        {
            if (
                originalItem
                is ModulePage originalPage
            )
            {
                ModulePage copiedPage =
                    new ModulePage
                    {
                        Id = originalPage.Id,
                        Name = originalPage.Name,
                        Body = originalPage.Body
                    };

                copiedModule.Content.Add(
                    copiedPage
                );
            }
            else if (
                originalItem
                is ModuleFile originalFile
            )
            {
                ModuleFile copiedFile =
                    new ModuleFile
                    {
                        Id = originalFile.Id,
                        Name = originalFile.Name,
                        FilePath =
                            originalFile.FilePath
                    };

                copiedModule.Content.Add(
                    copiedFile
                );
            }
            else if (
                originalItem
                is Assignment originalAssignment
            )
            {
                if (
                    copiedAssignmentsById.TryGetValue(
                        originalAssignment.Id,
                        out Assignment? copiedAssignment
                    )
                )
                {
                    copiedModule.Content.Add(
                        copiedAssignment
                    );
                }
            }
        }

        copiedCourse.Modules.Add(
            copiedModule
        );
    }

    foreach (
        AssignmentGroup originalGroup
        in originalCourse.AssignmentGroups)
    {
        AssignmentGroup copiedGroup =
            new AssignmentGroup
            {
                Id = originalGroup.Id,
                Name = originalGroup.Name,
                Weight = originalGroup.Weight
            };

        foreach (
            Assignment originalAssignment
            in originalGroup.Assignments)
        {
            if (
                copiedAssignmentsById.TryGetValue(
                    originalAssignment.Id,
                    out Assignment? copiedAssignment
                )
            )
            {
                copiedGroup.Assignments.Add(
                    copiedAssignment
                );
            }
        }

        copiedCourse.AssignmentGroups.Add(
            copiedGroup
        );
    }

    Add(copiedCourse);

    return copiedCourse;
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

    public void AddModule(
    int courseId,
    string? name)
{
    Course? course = GetById(courseId);

    if (course == null)
    {
        return;
    }

    int newModuleId =
        course.Modules.Count == 0
            ? 1
            : course.Modules.Max(
                module => module.Id
            ) + 1;

    Module newModule =
        new Module
        {
            Id = newModuleId,
            Name = string.IsNullOrWhiteSpace(name)
                ? $"Module {newModuleId}"
                : name.Trim()
        };

    course.Modules.Add(newModule);
}

public void UpdateModule(
    int courseId,
    int moduleId,
    string? name)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return;
    }

    Course? course =
        GetById(courseId);

    if (course == null)
    {
        return;
    }

    Module? module =
        GetModule(
            course,
            moduleId
        );

    if (module == null)
    {
        return;
    }

    module.Name =
        name.Trim();
}

public void DeleteModule(
    int courseId,
    int moduleId)
{
    Course? course =
        GetById(courseId);

    if (course == null)
    {
        return;
    }

    Module? module =
        GetModule(
            course,
            moduleId
        );

    if (module == null)
    {
        return;
    }

    course.Modules.Remove(module);
}

private Module? GetModule(
    Course course,
    int moduleId)
{
    return course.Modules.FirstOrDefault(
        module =>
            module.Id == moduleId
    );
}

private int GetNextModuleItemId(
    Module module)
{
    if (module.Content.Count == 0)
    {
        return 1;
    }

    return module.Content.Max(
        item => item.Id
    ) + 1;
}

public void AddModulePage(
    int courseId,
    int moduleId,
    string? name,
    string? body)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return;
    }

    Course? course =
        GetById(courseId);

    if (course == null)
    {
        return;
    }

    Module? module =
        GetModule(
            course,
            moduleId
        );

    if (module == null)
    {
        return;
    }

    ModulePage page =
        new ModulePage
        {
            Id = GetNextModuleItemId(module),
            Name = name.Trim(),
            Body = body
        };

    module.Content.Add(page);
}

public void AddModuleFile(
    int courseId,
    int moduleId,
    string? name,
    string? filePath)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return;
    }

    if (string.IsNullOrWhiteSpace(filePath))
    {
        return;
    }

    Course? course =
        GetById(courseId);

    if (course == null)
    {
        return;
    }

    Module? module =
        GetModule(
            course,
            moduleId
        );

    if (module == null)
    {
        return;
    }

    ModuleFile file =
        new ModuleFile
        {
            Id = GetNextModuleItemId(module),
            Name = name.Trim(),
            FilePath = filePath.Trim()
        };

    module.Content.Add(file);
}

public void AddAssignmentToModule(
    int courseId,
    int moduleId,
    int assignmentId)
{
    Course? course =
        GetById(courseId);

    if (course == null)
    {
        return;
    }

    Module? module =
        GetModule(
            course,
            moduleId
        );

    if (module == null)
    {
        return;
    }

    Assignment? assignment =
        course.Assignments.FirstOrDefault(
            assignment =>
                assignment.Id == assignmentId
        );

    if (assignment == null)
    {
        return;
    }

    bool assignmentAlreadyExists =
        module.Content.Any(
            item =>
                item == assignment
        );

    if (assignmentAlreadyExists)
    {
        return;
    }

    module.Content.Add(assignment);
}

public void UpdateModuleItem(
    int courseId,
    int moduleId,
    ModuleItem? item,
    string? name,
    string? details)
{
    if (item == null)
    {
        return;
    }

    if (string.IsNullOrWhiteSpace(name))
    {
        return;
    }

    Course? course =
        GetById(courseId);

    if (course == null)
    {
        return;
    }

    Module? module =
        GetModule(
            course,
            moduleId
        );

    if (module == null)
    {
        return;
    }

    if (!module.Content.Contains(item))
    {
        return;
    }

    if (item is ModulePage page)
    {
        page.Name =
            name.Trim();

        page.Body =
            details;
    }
    else if (item is ModuleFile file)
    {
        if (string.IsNullOrWhiteSpace(details))
        {
            return;
        }

        file.Name =
            name.Trim();

        file.FilePath =
            details.Trim();
    }
}

public void RemoveModuleItem(
    int courseId,
    int moduleId,
    ModuleItem? item)
{
    if (item == null)
    {
        return;
    }

    Course? course =
        GetById(courseId);

    if (course == null)
    {
        return;
    }

    Module? module =
        GetModule(
            course,
            moduleId
        );

    if (module == null)
    {
        return;
    }

    module.Content.Remove(item);
} 

public Announcement? AddAnnouncement(
    int courseId,
    string? title,
    string? message)
{
    if (string.IsNullOrWhiteSpace(title))
    {
        return null;
    }

    if (string.IsNullOrWhiteSpace(message))
    {
        return null;
    }

    Course? course =
        GetById(courseId);

    if (course == null)
    {
        return null;
    }

    int newAnnouncementId =
        course.Announcements.Count == 0
            ? 1
            : course.Announcements.Max(
                announcement =>
                    announcement.Id
            ) + 1;

    Announcement announcement =
        new Announcement
        {
            Id = newAnnouncementId,
            Title = title.Trim(),
            Message = message.Trim(),
            PostedDate = DateTime.Now
        };

    course.Announcements.Add(
        announcement
    );

    return announcement;
}

public bool UpdateAnnouncement(
    int courseId,
    int announcementId,
    string? title,
    string? message)
{
    if (string.IsNullOrWhiteSpace(title))
    {
        return false;
    }

    if (string.IsNullOrWhiteSpace(message))
    {
        return false;
    }

    Course? course =
        GetById(courseId);

    if (course == null)
    {
        return false;
    }

    Announcement? announcement =
        course.Announcements.FirstOrDefault(
            announcement =>
                announcement.Id ==
                announcementId
        );

    if (announcement == null)
    {
        return false;
    }

    announcement.Title =
        title.Trim();

    announcement.Message =
        message.Trim();

    announcement.PostedDate =
        DateTime.Now;

    return true;
}

public bool DeleteAnnouncement(
    int courseId,
    int announcementId)
{
    Course? course =
        GetById(courseId);

    if (course == null)
    {
        return false;
    }

    Announcement? announcement =
        course.Announcements.FirstOrDefault(
            announcement =>
                announcement.Id ==
                announcementId
        );

    if (announcement == null)
    {
        return false;
    }

    course.Announcements.Remove(
        announcement
    );

    return true;
}


   public void AddAssignment(
    int courseId,
    string? name,
    string? description,
    int availablePoints,
    DateTime dueDate)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return;
    }

    Course? course =
        GetById(courseId);

    if (course == null)
    {
        return;
    }

    int newAssignmentId =
        course.Assignments.Count == 0
            ? 1
            : course.Assignments.Max(
                assignment =>
                    assignment.Id
            ) + 1;

    Assignment newAssignment =
        new Assignment
        {
            Id = newAssignmentId,
            Name = name,
            Description = description,
            AvailablePoints = availablePoints,
            DueDate = dueDate
        };

    course.Assignments.Add(
        newAssignment
    );

    SaveCourse(courseId);
}
    public void AddQuiz(
    int courseId,
    string? name,
    string? description,
    string? question,
    int availablePoints,
    DateTime dueDate)
{
    if (
        string.IsNullOrWhiteSpace(name)
        ||
        string.IsNullOrWhiteSpace(question)
    )
    {
        return;
    }

    Course? course =
        GetById(courseId);

    if (course == null)
    {
        return;
    }

    int newAssignmentId =
        course.Assignments.Count == 0
            ? 1
            : course.Assignments.Max(
                assignment =>
                    assignment.Id
            ) + 1;

    Assignment newQuiz =
        new Assignment
        {
            Id = newAssignmentId,
            Name = name,
            Description = description,
            QuizQuestion = question,
            IsQuiz = true,
            AvailablePoints = availablePoints,
            DueDate = dueDate
        };

    course.Assignments.Add(
        newQuiz
    );

    SaveCourse(courseId);
}
    public void UpdateAssignment(
        int courseId,
        int assignmentId,
        string? name,
        string? description,
        int availablePoints,
        DateTime dueDate)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        Assignment? assignment =
            course.Assignments.FirstOrDefault(
                assignment =>
                    assignment.Id == assignmentId
            );

        if (assignment == null)
        {
            return;
        }

        assignment.Name = name;
        assignment.Description = description;
        assignment.AvailablePoints =
            availablePoints;
        assignment.DueDate = dueDate;
    }

    public void DeleteAssignment(
        int courseId,
        int assignmentId)
    {
        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        Assignment? assignment =
            course.Assignments.FirstOrDefault(
                assignment =>
                    assignment.Id == assignmentId
            );

        if (assignment == null)
        {
            return;
        }

        foreach (
            Module module
            in course.Modules)
        {
            module.Content.Remove(assignment);
        }

        foreach (
            AssignmentGroup group
            in course.AssignmentGroups)
        {
            group.Assignments.Remove(
                assignment
            );
        }

        assignment.Submissions.Clear();

        course.Assignments.Remove(assignment);
    }

    public Assignment? CopyAssignment(
    int sourceCourseId,
    int assignmentId,
    int destinationCourseId)
{
    if (sourceCourseId == destinationCourseId)
    {
        return null;
    }

    Course? sourceCourse =
        GetById(sourceCourseId);

    Course? destinationCourse =
        GetById(destinationCourseId);

    if (
        sourceCourse == null
        ||
        destinationCourse == null
    )
    {
        return null;
    }

    Assignment? originalAssignment =
        sourceCourse.Assignments.FirstOrDefault(
            assignment =>
                assignment.Id == assignmentId
        );

    if (originalAssignment == null)
    {
        return null;
    }

    int newAssignmentId =
        destinationCourse.Assignments.Count == 0
            ? 1
            : destinationCourse.Assignments.Max(
                assignment => assignment.Id
            ) + 1;

    Assignment copiedAssignment =
        new Assignment
        {
            Id = newAssignmentId,
            Name = originalAssignment.Name,
            Description =
                originalAssignment.Description,
            AvailablePoints =
                originalAssignment.AvailablePoints,
            DueDate =
                originalAssignment.DueDate
        };

    destinationCourse.Assignments.Add(
        copiedAssignment
    );

    return copiedAssignment;
}

    public void AddAssignmentGroup(
        int courseId,
        string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        int newGroupId =
            course.AssignmentGroups.Count == 0
            ? 1
            : course.AssignmentGroups.Max(
                group => group.Id
            ) + 1;

        AssignmentGroup newGroup =
            new AssignmentGroup
            {
                Id = newGroupId,
                Name = name
            };

        course.AssignmentGroups.Add(newGroup);
    }

    public void UpdateAssignmentGroup(
        int courseId,
        int groupId,
        string? newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            return;
        }

        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        AssignmentGroup? group =
            course.AssignmentGroups.FirstOrDefault(
                group => group.Id == groupId
            );

        if (group == null)
        {
            return;
        }

        group.Name = newName;
    }

    public bool UpdateAssignmentGroupWeight(
        int courseId,
        int groupId,
        double weight)
    {
        if (weight < 0)
        {
            return false;
        }

        Course? course = GetById(courseId);

        if (course == null)
        {
            return false;
        }

        AssignmentGroup? group =
            course.AssignmentGroups.FirstOrDefault(
                group => group.Id == groupId
            );

        if (group == null)
        {
            return false;
        }

        group.Weight = weight;

        return true;
    }

    public bool UpdateGradeRanges(
    int courseId,
    double minimumA,
    double minimumB,
    double minimumC,
    double minimumD)
{
    Course? course =
        GetById(courseId);

    if (course == null)
    {
        return false;
    }

    bool percentagesAreValid =
        minimumA >= 0
        &&
        minimumA <= 100
        &&
        minimumB >= 0
        &&
        minimumB <= 100
        &&
        minimumC >= 0
        &&
        minimumC <= 100
        &&
        minimumD >= 0
        &&
        minimumD <= 100;

    if (!percentagesAreValid)
    {
        return false;
    }

    bool rangesAreOrdered =
        minimumA > minimumB
        &&
        minimumB > minimumC
        &&
        minimumC > minimumD;

    if (!rangesAreOrdered)
    {
        return false;
    }

    course.MinimumAPercentage =
        minimumA;

    course.MinimumBPercentage =
        minimumB;

    course.MinimumCPercentage =
        minimumC;

    course.MinimumDPercentage =
        minimumD;

    return true;
}

    public double? CalculateCourseGrade(
        int courseId,
        int studentId)
    {
        Course? course = GetById(courseId);

        if (course == null)
        {
            return null;
        }

        bool studentIsEnrolled =
            course.Roster.Any(
                student =>
                    student.Id == studentId
            );

        if (!studentIsEnrolled)
        {
            return null;
        }

        double weightedGradeTotal = 0;
        double totalUsedWeight = 0;

        foreach (
            AssignmentGroup group
            in course.AssignmentGroups)
        {
            if (group.Weight <= 0)
            {
                continue;
            }

            double earnedPoints = 0;
            double availablePoints = 0;

            foreach (
                Assignment assignment
                in group.Assignments)
            {
                Submission? gradedSubmission =
                    assignment.Submissions
                        .Where(
                            submission =>
                                submission.StudentId
                                    == studentId
                                &&
                                submission.Grade.HasValue
                        )
                        .OrderByDescending(
                            submission =>
                                submission.SubmissionDate
                        )
                        .FirstOrDefault();

                if (gradedSubmission == null)
                {
                    continue;
                }

                if (
                    assignment.AvailablePoints
                    <= 0
                )
                {
                    continue;
                }

                earnedPoints +=
                    gradedSubmission.Grade
                    ?? 0;

                availablePoints +=
                    assignment.AvailablePoints;
            }

            if (availablePoints == 0)
            {
                continue;
            }

            double groupGrade =
                earnedPoints
                / availablePoints
                * 100;

            weightedGradeTotal +=
                groupGrade
                * group.Weight;

            totalUsedWeight +=
                group.Weight;
        }

        if (totalUsedWeight == 0)
        {
            return null;
        }

        return weightedGradeTotal
            / totalUsedWeight;
    }

    public void DeleteAssignmentGroup(
        int courseId,
        int groupId)
    {
        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        AssignmentGroup? group =
            course.AssignmentGroups.FirstOrDefault(
                group => group.Id == groupId
            );

        if (group == null)
        {
            return;
        }

        group.Assignments.Clear();

        course.AssignmentGroups.Remove(group);
    }

    public void AddAssignmentToGroup(
        int courseId,
        int groupId,
        int assignmentId)
    {
        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        AssignmentGroup? selectedGroup =
            course.AssignmentGroups
                .FirstOrDefault(
                    group =>
                        group.Id == groupId
                );

        if (selectedGroup == null)
        {
            return;
        }

        Assignment? assignment =
            course.Assignments.FirstOrDefault(
                assignment =>
                    assignment.Id == assignmentId
            );

        if (assignment == null)
        {
            return;
        }

        foreach (
            AssignmentGroup group
            in course.AssignmentGroups)
        {
            group.Assignments.Remove(
                assignment
            );
        }

        selectedGroup.Assignments.Add(
            assignment
        );
    }

    public void RemoveAssignmentFromGroup(
        int courseId,
        int groupId,
        int assignmentId)
    {
        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        AssignmentGroup? group =
            course.AssignmentGroups.FirstOrDefault(
                group => group.Id == groupId
            );

        if (group == null)
        {
            return;
        }

        Assignment? assignment =
            group.Assignments.FirstOrDefault(
                assignment =>
                    assignment.Id == assignmentId
            );

        if (assignment == null)
        {
            return;
        }

        group.Assignments.Remove(assignment);
    }

    public void EnrollStudent(
        int courseId,
        Student? student)
    {
        if (student == null)
        {
            return;
        }

        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        bool studentIsAlreadyEnrolled =
            course.Roster.Any(
                enrolledStudent =>
                    enrolledStudent.Id
                    == student.Id
            );

        if (studentIsAlreadyEnrolled)
        {
            return;
        }

        course.Roster.Add(student);

        SaveCourse(courseId);
    }

    public bool UnenrollStudent(
        int courseId,
        int studentId)
    {
        Course? course = GetById(courseId);

        if (course == null)
        {
            return false;
        }

        Student? student =
            course.Roster.FirstOrDefault(
                student =>
                    student.Id == studentId
            );

        if (student == null)
        {
            return false;
        }

        course.Roster.Remove(student);

        return true;
    }

    public Submission? AddSubmission(
    int courseId,
    int assignmentId,
    int studentId,
    string? content,
    string? attachedFileName = null,
    string? attachedFilePath = null)
{
    bool hasContent =
        !string.IsNullOrWhiteSpace(content);

    bool hasAttachedFile =
        !string.IsNullOrWhiteSpace(
            attachedFileName
        )
        &&
        !string.IsNullOrWhiteSpace(
            attachedFilePath
        );

    if (
        !hasContent
        &&
        !hasAttachedFile
    )
    {
        return null;
    }

    Course? course =
        GetById(courseId);

    if (course == null)
    {
        return null;
    }

    bool studentIsEnrolled =
        course.Roster.Any(
            student =>
                student.Id == studentId
        );

    if (!studentIsEnrolled)
    {
        return null;
    }

    Assignment? assignment =
        course.Assignments.FirstOrDefault(
            assignment =>
                assignment.Id ==
                    assignmentId
        );

    if (assignment == null)
    {
        return null;
    }

    int newSubmissionId =
        assignment.Submissions.Count == 0
        ? 1
        : assignment.Submissions.Max(
            submission =>
                submission.Id
        ) + 1;

    Submission newSubmission =
        new Submission
        {
            Id = newSubmissionId,
            StudentId = studentId,
            AssignmentId = assignmentId,
            Content = content,
            SubmissionDate = DateTime.Now,
            Grade = null,
            AttachedFileName =
                attachedFileName,
            AttachedFilePath =
                attachedFilePath
        };

    assignment.Submissions.Add(
        newSubmission
    );

    return newSubmission;
}

public SubmissionComment? AddSubmissionComment(
    int courseId,
    int assignmentId,
    int submissionId,
    int authorId,
    string? authorName,
    string? authorRole,
    string? message)
{
    if (string.IsNullOrWhiteSpace(message))
    {
        return null;
    }

    Course? course =
        GetById(courseId);

    if (course == null)
    {
        return null;
    }

    Assignment? assignment =
        course.Assignments.FirstOrDefault(
            assignment =>
                assignment.Id == assignmentId
        );

    if (assignment == null)
    {
        return null;
    }

    Submission? submission =
        assignment.Submissions.FirstOrDefault(
            submission =>
                submission.Id == submissionId
        );

    if (submission == null)
    {
        return null;
    }

    submission.Comments ??=
        new List<SubmissionComment>();

    int newCommentId =
        submission.Comments.Count == 0
            ? 1
            : submission.Comments.Max(
                comment => comment.Id
            ) + 1;

    SubmissionComment comment =
        new SubmissionComment
        {
            Id = newCommentId,
            SubmissionId = submissionId,
            AuthorId = authorId,
            AuthorName =
                string.IsNullOrWhiteSpace(authorName)
                    ? "Unknown User"
                    : authorName.Trim(),
            AuthorRole =
                string.IsNullOrWhiteSpace(authorRole)
                    ? "User"
                    : authorRole.Trim(),
            Message = message.Trim(),
            PostedDate = DateTime.Now
        };

    submission.Comments.Add(comment);

    bool saved =
        SaveCourse(courseId);

    if (!saved)
    {
        submission.Comments.Remove(comment);

        return null;
    }

    return comment;
}

    public bool GradeSubmission(
    int courseId,
    int assignmentId,
    int submissionId,
    double grade,
    string? feedback)
{
    Course? course = GetById(courseId);

    if (course == null)
    {
        return false;
    }

    Assignment? assignment =
        course.Assignments.FirstOrDefault(
            assignment =>
                assignment.Id == assignmentId
        );

    if (assignment == null)
    {
        return false;
    }

    if (
        grade < 0
        ||
        grade > assignment.AvailablePoints
    )
    {
        return false;
    }

    Submission? submission =
        assignment.Submissions.FirstOrDefault(
            submission =>
                submission.Id == submissionId
        );

    if (submission == null)
    {
        return false;
    }

    submission.Grade = grade;
    submission.Feedback = feedback;

    return true;
}}