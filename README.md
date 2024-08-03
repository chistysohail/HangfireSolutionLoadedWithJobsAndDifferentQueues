
# Hangfire Demo Project

This project demonstrates the use of Hangfire with a SQL Server backend to schedule and execute background jobs in a .NET application. The application uses Hangfire's dashboard to monitor and manage job queues and processing.

## Prerequisites

- .NET 6 SDK
- SQL Server (for Hangfire storage)
- Docker and Docker Compose (for running in containers)
- Hangfire libraries

## Setup and Configuration

### Database Configuration

Ensure you have a SQL Server instance running and accessible. Update the connection string in the code to point to your SQL Server instance:

### Worker Configuration

The number of workers is configured in the code. By default, Hangfire uses `Environment.ProcessorCount * 5` as the number of workers, which can be adjusted by setting `options.WorkerCount`:

```csharp
services.AddHangfireServer(options =>
{
    options.Queues = new[] { "critical", "default", "low-priority" };
    // Example configuration for 40 workers
    options.WorkerCount = 40; // Configure number of workers
});
```

### Key Storage

Data protection keys are stored in the `/keys` directory. Ensure this directory is created and accessible by the application:

```csharp
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/keys"))
    .SetApplicationName("hangfire-app")
    .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
```

## Running the Application

1. **Build and Run Locally:**

   Make sure your environment is set up with the .NET 6 SDK, and then build and run the project using the following commands:

   ```bash
   dotnet build
   dotnet run
   ```

2. **Running with Docker and Docker Compose:**

   Use Docker Compose to set up and run the application. Below is an example `docker-compose.yml` file:

   ```yaml
   version: '3.8'

   services:
     hangfire-console-app:
       build:
         context: ./HangfireConsoleApp
       networks:
         - hangfire-net

     hangfire-dashboard:
       build:
         context: ./HangfireDashboard
       ports:
         - "8080:80"  # Maps port 80 in the container to port 8080 on your host
       networks:
         - hangfire-net
       volumes:
         - hangfire-keys:/keys  # Add this line to mount the keys volume

   networks:
     hangfire-net:
       external: false

   volumes:
     hangfire-keys:
       driver: local
   ```

   To build and run the containers:

   ```bash
   docker-compose up --build
   ```

```

## Hangfire Dashboard

The Hangfire dashboard is available at `http://localhost:8080/hangfire` and allows monitoring and managing jobs. It's configured with a simple authorization filter that allows all requests:

```csharp
var options = new DashboardOptions
{
    Authorization = new[] { new AllowAllAuthorizationFilter() }
};

app.UseHangfireDashboard("/hangfire", options);
```

## Scheduling Jobs

The application enqueues jobs in three different queues: `critical`, `default`, and `low-priority`. You can modify the job scheduling logic in the `ScheduleJobs` method.

```csharp
static void ScheduleJobs()
{
    for (int i = 0; i < 250; i++)
    {
        BackgroundJob.Enqueue(() => ExecuteJob("critical", i));
    }
    for (int i = 250; i < 600; i++)
    {
        BackgroundJob.Enqueue(() => ExecuteJob("low-priority", i));
    }
    for (int i = 600; i < 1000; i++)
    {
        BackgroundJob.Enqueue(() => ExecuteJob("default", i));
    }
}
```
## License

This project is licensed under the MIT License. See the LICENSE file for more details.
![image](https://github.com/user-attachments/assets/dc178d34-1408-42e8-91b6-03c8411d0c60)
![image](https://github.com/user-attachments/assets/63408262-2b27-4c52-a527-566239e7dabb)
![image](https://github.com/user-attachments/assets/5b9a5d95-7175-4b8c-9855-b736fa3b7840)
