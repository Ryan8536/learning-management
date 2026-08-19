# Learning Management System (LMS)

A full-stack Learning Management System built as a class project, made up of a shared C# domain library, a REST API, a command-line client, and a cross-platform .NET MAUI app for students and teachers.

## Overview

The system models the core of a real LMS: courses, semesters, modules, assignments, submissions, announcements, and role-based views for students, teachers, and admins. Rather than one monolithic app, the project is split into independent layers that all build on a shared domain library — closer to how a real production system would be structured.

## Architecture

```
LMS/
├── Library.LMS/    # Shared domain models + service layer (Course, Student, Assignment, Module, Submission, etc.)
├── Api.LMS/        # ASP.NET Core Web API — REST endpoints over the domain models
├── CLI.LMS/        # Command-line client for interacting with the system
└── Maui.LMS/        # .NET MAUI cross-platform app (student, teacher, and course views)
```

- **Library.LMS** — the shared core: domain models (`Course`, `Student`, `Instructor`, `Assignment`, `AssignmentGroup`, `Module`, `ModuleItem`, `ModulePage`, `Submission`, `SubmissionComment`, `Announcement`, `Semester`, `User`) plus service classes that both the API and CLI build on top of.
- **Api.LMS** — an ASP.NET Core Web API exposing courses and students as REST resources (`CoursesController`, `StudentsController`), backed by a simple file-based store.
- **CLI.LMS** — a terminal client for exercising the system without the UI.
- **Maui.LMS** — the front-end app, with separate page flows for students (`StudentMenuPage`, `StudentCoursePage`, `StudentCourseDetailPage`) and teachers (`TeacherMenuPage`, `CourseDetailPage`, `CourseMenuPage`, `CourseSettingsPage`), built with the MVVM pattern (dedicated `ViewModels/` alongside `Views/`).

## Features

- Course and semester management
- Assignments, assignment groups, and student submissions (with comments)
- Modules, module pages, and module items/files for organizing course content
- Announcements
- Distinct student and teacher/instructor views and workflows
- REST API layer decoupled from the client apps, so the same domain logic drives both the CLI and the MAUI app

## Tech stack

- **C# / .NET**
- **ASP.NET Core** — Web API
- **.NET MAUI** — cross-platform client (Views + ViewModels, MVVM)
- Solution managed via `LMS.slnx`

## Getting started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (matching the version targeted by the `.csproj` files)
- For the MAUI app: the .NET MAUI workload (`dotnet workload install maui`)

### Setup

```bash
git clone https://github.com/Ryan8536/learning-management.git
cd learning-management/LMS
```

### Run the API

```bash
cd Api.LMS
dotnet run
```

The API exposes course and student endpoints under `api/`. See `Api.LMS.http` for example requests.

### Run the CLI

```bash
cd CLI.LMS
dotnet run
```

### Run the MAUI app

Open `LMS.slnx` in Visual Studio (with the .NET MAUI workload installed) and run the `Maui.LMS` project, or:

```bash
cd Maui.LMS
dotnet build -t:Run -f net8.0-windows10.0.19041.0
```
*(adjust the target framework to whichever platform you're building for)*

## Project management

This repo also tracks the project's own development process — the `backlog/` folder contains sprint-by-sprint issue definitions (`issues__Sprint_1.yml` through `Sprint_7.yml`), and `scripts/` has the automation (`populate_issues.py`, `export_backlog.py`) used to sync that backlog with GitHub Issues via a GitHub Actions workflow.

## Contributors

Built with [crmillfsu](https://github.com/crmillfsu) as a class project.

## License

MIT — see [LICENSE](LICENSE).
