# Use the .NET 6 SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY HangfireDashboard.csproj ./
RUN dotnet restore

# Copy the remaining source files and build the application
COPY . .
RUN dotnet publish -c Release -o /app

# Use the ASP.NET Core runtime image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

# Copy the built application from the build stage
COPY --from=build /app ./

# Expose port 80 to access the Hangfire Dashboard
EXPOSE 80

# Run the application
ENTRYPOINT ["dotnet", "HangfireDashboard.dll"]
