using System.Text.Json;
using Library.LMS.Models;

namespace Api.LMS.Services;

public static class StudentFileStore
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

    private static readonly string StudentFilePath =
        Path.Combine(
            DataDirectory,
            "students.json"
        );

    public static List<Student> LoadStudents()
    {
        lock (FileLock)
        {
            Directory.CreateDirectory(
                DataDirectory
            );

            if (!File.Exists(StudentFilePath))
            {
                return new List<Student>();
            }

            try
            {
                string json =
                    File.ReadAllText(
                        StudentFilePath
                    );

                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<Student>();
                }

                return JsonSerializer.Deserialize<
                    List<Student>
                >(
                    json,
                    JsonOptions
                )
                ?? new List<Student>();
            }
            catch (
                IOException
            )
            {
                return new List<Student>();
            }
            catch (
                JsonException
            )
            {
                return new List<Student>();
            }
        }
    }

    public static bool SaveStudents(
        IEnumerable<Student> students)
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
                        students,
                        JsonOptions
                    );

                File.WriteAllText(
                    StudentFilePath,
                    json
                );

                return true;
            }
            catch (
                IOException
            )
            {
                return false;
            }

            catch (
                UnauthorizedAccessException
            )
            {
                return false;
            }
        }
    }
}