# Code-Sync
tests use of blazor in c#

# FileSync

Built this to learn Blazor Server and Clean Architecture. Nothing fancy just a file sharing app with two modes

- **Spaces** — shared text + file upload with a join code (like `BLUE-TIGER-42`). Expires after 12 hours.
- **FileDrops** — upload files, get a link, send it to someone. Optional passcode. Expires in 1-7 days.

## Tech

- .NET 9, Blazor Server
- EF Core + SQLite
- SignalR for real-time sync in Spaces
- BCrypt for passcode hashing
- Docker for deployment

## Architecture

Clean Architecture with 4 projects:

```
Domain          → Entities, interfaces (no dependencies)
Infrastructure  → Database, file storage, email
Application     → Services, DTOs, business logic
Web             → Blazor pages, SignalR hub
```

## Run it

```bash
dotnet ef migrations add Init --project FileSync.Infrastructure --startup-project FileSync.Web
dotnet ef database update --project FileSync.Infrastructure --startup-project FileSync.Web
cd FileSync.Web
dotnet run
```

i was using it to learn clean architectuere in dotnet 
