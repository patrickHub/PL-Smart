# ------------ Build stage ------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the csproj files needed to restore the API dependency graph
COPY src/PristaneLaverieSmart.API/PristaneLaverieSmart.API.csproj src/PristaneLaverieSmart.API/
COPY src/PristaneLaverieSmart.Application/PristaneLaverieSmart.Application.csproj src/PristaneLaverieSmart.Application/
COPY src/PristaneLaverieSmart.Domain/PristaneLaverieSmart.Domain.csproj src/PristaneLaverieSmart.Domain/
COPY src/PristaneLaverieSmart.Infrastructure/PristaneLaverieSmart.Infrastructure.csproj src/PristaneLaverieSmart.Infrastructure/

# If you have NuGet.config, copy it too:
# COPY NuGet.config ./

# Restore ONLY the API project (avoids missing UI/tests projects)
RUN dotnet restore src/PristaneLaverieSmart.API/PristaneLaverieSmart.API.csproj -v minimal

# Copy the rest of the repo and publish
COPY . .
RUN dotnet publish src/PristaneLaverieSmart.API/PristaneLaverieSmart.API.csproj -c Release -o /app/publish --no-restore

# ------------ Runtime stage ------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PristaneLaverieSmart.API.dll"]