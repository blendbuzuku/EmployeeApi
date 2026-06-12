# Employee Management API

A robust RESTful API for employee management with file upload capabilities. Built with ASP.NET Core 8, Entity Framework Core, and SQL Server.

## Related Frontend Repository

This API powers the PeopleFlow employee management dashboard.

Frontend Repository: [employee-ui](https://github.com/YOUR_USERNAME/employee-ui)

The frontend provides a modern React interface for employee directory management, profile picture uploads, document attachments, and real-time analytics dashboard.

## API Base URL

Development: http://localhost:5199
Production: https://your-api-domain.com

## Available Endpoints

GET /api/employees - Get all employees
GET /api/employees/{id} - Get employee by ID
POST /api/employees - Create new employee
PUT /api/employees/{id} - Update employee
DELETE /api/employees/{id} - Delete employee
POST /api/fileupload/upload-profile/{id} - Upload profile picture
POST /api/fileupload/upload-attachment/{id} - Upload document

## CORS Configuration

To connect with the frontend, ensure CORS is configured in Program.cs:

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://your-frontend-domain.com")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

## Environment Variables

Create appsettings.json:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=;Database=;Trusted_Connection=True;"
  }
}

## Getting Started

git clone https://github.com/YOUR_USERNAME/employee-api.git
cd employee-api/EmployeeApi
dotnet restore
dotnet ef database update
dotnet run

## Full Stack Application

This API is part of a complete employee management system:

Backend API: employee-api - ASP.NET Core 8, EF Core
Frontend UI: employee-ui - React, Vite, Axios

## License

MIT
