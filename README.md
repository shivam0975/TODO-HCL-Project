# TODO App

[![Angular](https://img.shields.io/badge/Angular-21-DD0031?logo=angular&logoColor=white)](frontend/package.json)
[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](backend/backend.csproj)
[![MongoDB](https://img.shields.io/badge/MongoDB-Driver-47A248?logo=mongodb&logoColor=white)](backend/backend.csproj)

A full-stack task management application with JWT authentication, a .NET Web API backend, MongoDB persistence, an Angular frontend, and an ASP.NET Core MVC client.

## What This Project Does

This project provides a complete TODO workflow:

- User registration and login with JWT token authentication
- Create, read, update, and delete personal tasks
- Task status toggling (`Pending` / `Completed`)
- Priority, category, due date, and description support
- Route protection on the frontend and authorization checks on the backend
- Server-side MVC task management (login + create/read/update/delete)

### Stack

- Backend: ASP.NET Core (`net10.0`), MongoDB Driver, JWT Bearer auth, BCrypt password hashing
- Frontend: Angular 21 standalone components, RxJS, reactive forms
- MVC client: ASP.NET Core MVC (`net10.0`) using HttpClient + Razor views
- Database: MongoDB

## Why It Is Useful

- Production-style architecture split into controllers, services, and repositories
- End-to-end auth flow (register/login, token storage, guarded routes, secured APIs)
- Two client options: Angular SPA and server-rendered MVC
- Clean baseline for extending into team tasks, reminders, tags, or reporting
- Good learning reference for Angular + .NET + MongoDB integration

## How To Get Started

### Prerequisites

- Node.js 20+ and npm
- .NET SDK 10
- MongoDB instance (local or hosted)

### 1. Clone and install dependencies

```bash
git clone <your-repo-url>
cd TODO-App

cd frontend
npm install
cd ..
```

### 2. Configure backend settings

The backend reads configuration from `backend/appsettings.json` and environment overrides.

Set these values before running in your environment:

- `ConnectionStrings__MongoDbConnection`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`

Example (macOS/Linux):

```bash
export ConnectionStrings__MongoDbConnection="mongodb://localhost:27017"
export Jwt__Key="replace-with-a-long-random-secret"
export Jwt__Issuer="TodoApi"
export Jwt__Audience="TodoApiUsers"
```

### 3. Run the backend API

```bash
cd backend
dotnet restore
dotnet run
```

By default, the API runs on:

- `http://localhost:5137`
- `https://localhost:7185`

OpenAPI/Scalar docs are available in development mode.

### 4. Run the MVC client (optional)

In a second terminal:

```bash
cd backendMVC
dotnet restore
dotnet run
```

Then open one of the URLs shown in the startup output.

The MVC app calls the backend API using `ApiSettings:BaseUrl` from `backendMVC/appsettings.json`.
Default value:

- `http://localhost:5137/`

### 5. Run the frontend

In a second terminal:

```bash
cd frontend
npm start
```

Then open `http://localhost:4200`.

The frontend API URL defaults to `http://localhost:5137` in `frontend/src/environments/environment.ts`.

### 6. Quick usage example

1. Register a new account from the UI.
2. Sign in.
3. Create a task with title, priority, and optional category/due date.
4. Toggle status or edit/delete it from the task list.

You can also test requests with `backend/backend.http` or any API client.

### 7. MVC Features

- Login against `POST /api/auth/login`
- List tasks from `GET /api/tasks`
- Create task via `POST /api/tasks`
- Edit task via `PUT /api/tasks/{id}`
- Delete task via `DELETE /api/tasks/{id}`
- Connection-failure handling with user-friendly error messages

### 8. Troubleshooting

- If MVC shows "Cannot reach API...", ensure backend API is running first (`cd backend && dotnet run`).
- If login fails in MVC, verify the API base URL in `backendMVC/appsettings.json` points to the active backend URL.
- If app startup fails with "address already in use", change ports in `backendMVC/Properties/launchSettings.json`.

## Screenshots

### Login

![Login Screen](<public/LoginScreen.png>)

### Categories

![Register Screen](<public/Categories.png>)

### Task Dashboard

![Task Dashboard](<public/MyTaskDashboard.png>)

### Add Task

![Add Task](<public/AddTask.png>)

### Edit Task

![Edit Task](<public/EditTask.png>)

### MVC Login

![MVC Login](<public/MVCLogin.png>)

### MVC Dashboard

![MVC Dashboard](<public/MVCHome.png>)

## API Overview

Auth endpoints:

- `POST /api/auth/register`
- `POST /api/auth/login`

Task endpoints (JWT required):

- `GET /api/tasks`
- `GET /api/tasks/{id}`
- `POST /api/tasks`
- `PUT /api/tasks/{id}`
- `DELETE /api/tasks/{id}`

## Where To Get Help

- Check frontend notes: [frontend/README.md](frontend/README.md)
- Inspect sample HTTP requests: [backend/backend.http](backend/backend.http)
- Review backend startup/configuration: [backend/Program.cs](backend/Program.cs)
- Review MVC startup/configuration: [backendMVC/Program.cs](backendMVC/Program.cs)
- Open an issue in this repository for bugs, questions, or feature requests

## Maintainers And Contributions

This project is maintained by the repository owner and contributors.

### Contributors

- [Utkarsh (Vectorutkarsh9)](https://github.com/Vectorutkarsh9)
- [Sankalp Tripathi (tripathisankalp77)](https://github.com/tripathisankalp77)
- [Rohit (rohit5434)](https://github.com/rohit5434)

- Contribution guide: [CONTRIBUTING.md](CONTRIBUTING.md)
- Before contributing, run:

```bash
cd backend && dotnet build
cd ../backendMVC && dotnet build
cd ../frontend && npm run build
```

## Project Structure

```text
TODO-App/
├─ backend/   # ASP.NET Core API, auth, task CRUD, MongoDB data access
├─ backendMVC/ # ASP.NET Core MVC client (Razor) with task CRUD
└─ frontend/  # Angular UI, auth flow, task pages/components
```
