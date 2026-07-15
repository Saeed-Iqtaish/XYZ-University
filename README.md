# XYZ University

A university management system built with **ASP.NET Core 8 MVC** — role-based portals for administering departments, courses, instructor offices, and student enrollments. Built as coursework for my ASP.NET Core MVC class at Al-Hussein Technical University (2026).

## Features

- **Role-based access control** — three roles (Admin, Instructor, Student) seeded on startup via ASP.NET Core Identity, with `[Authorize]` policies across controllers and a default admin account bootstrapped on first run
- **Admin panel** — list all users with their departments and assign roles, built on `UserManager` / `RoleManager`
- **Course catalog** — full CRUD for courses, departments, and instructor offices, with data-annotation validation (required fields, credit range 1–6, length constraints)
- **Course specializations** — `Lab` and `OnlineCourse` types inherit from `Course` and are mapped through EF Core inheritance
- **Enrollments** — many-to-many student ↔ course relationship modelled as a join entity carrying a letter grade
- **Extended Identity user** — `ApplicationUser` adds name, department, office assignment, and enrollment navigation to the stock `IdentityUser`
- **Custom Razor tag helper** — `<submit-btn>` renders consistently styled form submit buttons across views

## Tech stack

ASP.NET Core 8 MVC · C# · Entity Framework Core 8 (SQL Server; SQLite provider included) · ASP.NET Core Identity · Razor views

## Data model

| Entity | Notes |
|---|---|
| `ApplicationUser` | Extends `IdentityUser` — first/last name, optional `Department`, optional `Office`, enrollments |
| `Course` | Title, credits (1–6), belongs to a `Department`, optional instructor; subtypes `Lab` (lab software) and `OnlineCourse` (platform) |
| `Department` | Name and code; owns courses and users |
| `Office` | Location with a one-to-one instructor assignment |
| `Enrollment` | Join entity: student + course + grade |

## Getting started

**Prerequisites:** [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and SQL Server (LocalDB is fine).

```bash
git clone https://github.com/Saeed-Iqtaish/XYZ-University.git
cd XYZ-University/XYZ-University
```

1. Update `DefaultConnection` in `appsettings.json` to point at your SQL Server instance, e.g. `Server=(localdb)\\MSSQLLocalDB;Database=XYZ-University-DB;Trusted_Connection=True;TrustServerCertificate=True`
2. Apply the migrations:

```bash
dotnet tool install --global dotnet-ef   # if not already installed
dotnet ef database update
```

3. Run:

```bash
dotnet run
```

On first run, the role seeder creates the Admin / Instructor / Student roles and a default admin account (see `Data/RoleSeeder.cs` for the credentials), so you can sign in and explore the admin panel immediately.

## Project structure

```
XYZ-University/
├── Controllers/      # Admin, Courses, Departments, Enrollments, Offices, Home
├── Models/           # Domain entities + ApplicationUser
├── Data/             # DbContext, migrations, role seeder
├── TagHelper/        # Custom <submit-btn> tag helper
├── Areas/Identity/   # Identity UI
├── Views/            # Razor views
└── wwwroot/          # Static assets
```

<!-- ## Screenshots
Add 2–3 screenshots here (admin panel, course list, enrollment view) — drop them in a /docs folder and reference:
![Admin panel](docs/admin.png)
-->
