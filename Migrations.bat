set /p id=Enter MigrationName:
dotnet ef migrations add %id% --project data --startup-project api --output-dir Database/Migrations
cmd/k