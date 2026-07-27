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

        course.Id = Courses.Count == 0
            ? 1
            : Courses.Max(course => course.Id) + 1;

        Courses.Add(course);
    }

    public Course? GetById(int id)
    {
        return Courses.FirstOrDefault(
            course => course.Id == id
        );
    }

    public void Delete(int courseId)
   {
    Course? courseToDelete = GetById(courseId);

    if (courseToDelete != null)
    {
        Courses.Remove(courseToDelete);
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
}    public void AddAssignment(
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

        Course? course = GetById(courseId);

        if (course == null)
        {
            return;
        }

        int newAssignmentId =
            course.Assignments.Count == 0
            ? 1
            : course.Assignments.Max(
                assignment => assignment.Id
            ) + 1;

        Assignment newAssignment = new Assignment
        {
            Id = newAssignmentId,
            Name = name,
            Description = description,
            AvailablePoints = availablePoints,
            DueDate = dueDate
        };

        course.Assignments.Add(newAssignment);
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
        string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

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

        Assignment? assignment =
            course.Assignments.FirstOrDefault(
                assignment =>
                    assignment.Id
                    == assignmentId
            );

        if (assignment == null)
        {
            return null;
        }

        int newSubmissionId =
            assignment.Submissions.Count == 0
            ? 1
            : assignment.Submissions.Max(
                submission => submission.Id
            ) + 1;

        Submission newSubmission =
            new Submission
            {
                Id = newSubmissionId,
                StudentId = studentId,
                AssignmentId = assignmentId,
                Content = content,
                SubmissionDate = DateTime.Now,
                Grade = null
            };

        assignment.Submissions.Add(
            newSubmission
        );

        return newSubmission;
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