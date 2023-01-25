dotnet ef database drop --project "Bumbo.Data\Bumbo.Data.csproj" --startup-project "Bumbo.Web\Bumbo.Web.csproj" --force
rmdir /s /q .\Bumbo.Data\Migrations\
dotnet ef migrations add --project "Bumbo.Data\Bumbo.Data.csproj" --startup-project "Bumbo.Web\Bumbo.Web.csproj" --context "Bumbo.Data.BumboDbContext" --configuration Debug Initial --output-dir "Migrations"
dotnet ef database update --project "Bumbo.Data\Bumbo.Data.csproj" --startup-project "Bumbo.Web\Bumbo.Web.csproj" --context "Bumbo.Data.BumboDbContext" --configuration Debug
pause