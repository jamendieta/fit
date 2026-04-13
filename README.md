# Task Management - Technical Exercise

Solution implemented with .NET 10, Clean Architecture style, SQLite (without Entity Framework, Dapper, or MediatR), and Vue 3 frontend.

## Informal User Story
As a team member, I want to authenticate in the platform and manage my own tasks (create, view, update, and delete) so I can track commitments and deadlines in one place.

## Tech Stack
- Backend: ASP.NET Core Web API (.NET 10)
- Data access: raw `Microsoft.Data.Sqlite` (ADO.NET style)
- Auth: JWT Bearer
- Frontend: Vue 3 + Vite
- Testing: xUnit (Application, Infrastructure, Presentation/API)

## Architecture
- `src/TaskManagement.Core`: entities and repository contracts
- `src/TaskManagement.Application`: business logic/services
- `src/TaskManagement.Infrastructure`: SQLite repositories + DB initializer/seed
- `src/TaskManagement.Presentation`: Web API controllers + auth wiring
- `src/TaskManagement.Presentation/frontend`: Vue 3 app
- `tests/*`: unit/integration tests by layer

## Seeded Demo Credentials
- Username: `admin`
- Password: `admin123`

## Functional Coverage
- Task CRUD endpoints
- User register/login
- Public endpoint: `GET /api/users/public-ping`
- Authorized endpoint: `GET /api/users/me`
- Task endpoints protected with JWT and scoped to authenticated user

## API Endpoints
- `POST /api/users/register`
- `POST /api/users/login`
- `GET /api/users/public-ping` (anonymous)
- `GET /api/users/me` (authorized)
- `GET /api/tasks` (authorized)
- `GET /api/tasks/{id}` (authorized)
- `POST /api/tasks` (authorized)
- `PUT /api/tasks/{id}` (authorized)
- `DELETE /api/tasks/{id}` (authorized)

## Run Backend
```bash
cd src/TaskManagement.Presentation
dotnet run --launch-profile http
```
API runs on `http://localhost:5082`.
Swagger UI runs on `http://localhost:5082/swagger`.

## Run Frontend + Backend Together (recommended)
```bash
cd /Users/jimmyandresmendietarivera/Documents/fit
npm install
npm run dev
```
Behavior of `npm run dev`:
- If backend is not running, it starts backend on `http://localhost:5082` and frontend on `http://localhost:5173`.
- If backend is already running on `http://localhost:5082`, it does not restart it and keeps frontend running on `http://localhost:5173`.

The terminal prints useful URLs on startup:
- API: `http://localhost:5082`
- Swagger: `http://localhost:5082/swagger`
- Frontend: `http://localhost:5173`

Frontend uses Vite proxy to call `/api/*` against the backend automatically.

## Run Frontend
```bash
cd src/TaskManagement.Presentation/frontend
npm install
npm run dev
```
Frontend runs on `http://localhost:5173`.

## Frontend Environment Variable (optional)
Create `.env` in `src/TaskManagement.Presentation/frontend`:
```bash
VITE_API_URL=http://localhost:5082
```
If not provided, frontend uses relative `/api` paths (recommended for local dev with Vite proxy).

## Run Tests
```bash
cd /Users/jimmyandresmendietarivera/Documents/fit
dotnet test
```

## Generative AI Section (requested by exercise)
See `GENAI_NOTES.md` for:
- Prompt used
- Representative generated output sample
- Validation and correction process
- Edge cases and security considerations
