# TODO App

[![Angular](https://img.shields.io/badge/Angular-21-DD0031?logo=angular&logoColor=white)](frontend/package.json)
[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](backend/backend.csproj)
[![MongoDB](https://img.shields.io/badge/MongoDB-Driver-47A248?logo=mongodb&logoColor=white)](backend/backend.csproj)

A full-stack task management application with JWT authentication, a .NET Web API backend, MongoDB persistence, and an Angular frontend.

## What This Project Does

This project provides a complete TODO workflow:

- User registration and login with JWT token authentication
- Create, read, update, and delete personal tasks
- Task status toggling (`Pending` / `Completed`)
- Priority, category, due date, and description support
- Route protection on the frontend and authorization checks on the backend

### Stack

- Backend: ASP.NET Core (`net10.0`), MongoDB Driver, JWT Bearer auth, BCrypt password hashing
- Frontend: Angular 21 standalone components, RxJS, reactive forms
- Database: MongoDB

## Why It Is Useful

- Production-style architecture split into controllers, services, and repositories
- End-to-end auth flow (register/login, token storage, guarded routes, secured APIs)
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

### 4. Run the frontend

In a second terminal:

```bash
cd frontend
npm start
```

Then open `http://localhost:4200`.

The frontend API URL defaults to `http://localhost:5137` in `frontend/src/environments/environment.ts`.

### 5. Quick usage example

1. Register a new account from the UI.
2. Sign in.
3. Create a task with title, priority, and optional category/due date.
4. Toggle status or edit/delete it from the task list.

You can also test requests with `backend/backend.http` or any API client.

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
- Open an issue in this repository for bugs, questions, or feature requests

## Maintainers And Contributions

This project is maintained by the repository owner and contributors.

- Contribution guide: [CONTRIBUTING.md](CONTRIBUTING.md)
- Before contributing, run:

```bash
cd backend && dotnet build
cd ../frontend && npm run build
```

## Project Structure

```text
TODO-App/
├─ backend/   # ASP.NET Core API, auth, task CRUD, MongoDB data access
└─ frontend/  # Angular UI, auth flow, task pages/components
```
