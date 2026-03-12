ThreadSpace – Project Commands Reference


=== FRONTEND (Run Locally without Docker) ===

Navigate to frontend folder
  cd frontend

Install dependencies (first time only)
  npm install

Compile TypeScript to JavaScript
  npm run build

Watch mode (auto-recompile on file change)
  npm run dev

Note: After building, open index.html directly in a browser OR serve it
      with a local server. With Docker, it runs on http://localhost:3000


=== FRONTEND (Docker) ===

Rebuild frontend only (after TypeScript/CSS/HTML changes)
  docker compose build frontend
  docker compose up frontend

Frontend URL
  http://localhost:3000


=== BACKEND (Run Locally without Docker) ===

Navigate to backend folder
  cd backend

Restore NuGet packages
  dotnet restore

Run the backend (development mode)
  dotnet run

Run with hot reload (auto-restart on file change)
  dotnet watch run

Build the project (check for errors)
  dotnet build

Publish for production
  dotnet publish -c Release -o ./publish

Note: When running locally, the API starts at http://localhost:5000
      Make sure PostgreSQL is running and the connection string in
      appsettings.json matches your local DB before running.


=== DOCKER ===

Start all services (build + run)
  docker compose up --build

Start in detached/background mode
  docker compose up --build -d

Stop all services
  docker compose down

Stop and delete volumes (wipes database)
  docker compose down -v

Rebuild only the API container
  docker compose build api

View live logs (all services)
  docker compose logs -f

View logs for API only
  docker compose logs -f api

View logs for DB only
  docker compose logs -f db

Restart API only (after code change)
  docker compose restart api


=== DATABASE (TablePlus Connection) ===

Host     : 127.0.0.1
Port     : 5434
User     : postgres
Password : postgres  (update in .env if different)
Database : ThreadSpace


=== API ===

Base URL — via Docker
  http://localhost:5001

Base URL — running locally (dotnet run)
  http://localhost:5000

Health check (quick test)
  curl http://localhost:5001/api/tags


=== QUICK API TEST (curl) ===

Register a user
  curl -X POST http://localhost:5001/api/auth/register \
    -H "Content-Type: application/json" \
    -d '{"name":"John Doe","email":"john@example.com","username":"johndoe","password":"password123","userLevel":"Mid-Level"}'

Login
  curl -X POST http://localhost:5001/api/auth/login \
    -H "Content-Type: application/json" \
    -d '{"email":"john@example.com","password":"password123"}'

Get all posts (public)
  curl http://localhost:5001/api/posts

Get all tags (public)
  curl http://localhost:5001/api/tags

Get AI posts (public)
  curl http://localhost:5001/api/ai-posts

Create a post (replace TOKEN)
  curl -X POST http://localhost:5001/api/posts \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer TOKEN" \
    -d '{"tagId":1,"title":"My First Post","content":"Hello ThreadSpace!"}'

Upvote a post (replace TOKEN and POST_ID)
  curl -X POST http://localhost:5001/api/posts/POST_ID/vote \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer TOKEN" \
    -d '{"isLike":true}'


=== USER LEVEL VALUES ===

Valid values for userLevel field:
  Student | Fresher | Mid-Level | Pro


=== GIT BRANCHES ===

main      → stable / production-ready
dev       → active development
feat/*    → feature branches
