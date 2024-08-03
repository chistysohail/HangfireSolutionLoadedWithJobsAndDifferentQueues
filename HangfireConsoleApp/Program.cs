using System;
using System.Threading;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

class Program
{
    static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage("Server=host.docker.internal,1434;Database=HangfireDemo;User Id=sa;Password=YourNewStrong(!)Password;MultipleActiveResultSets=true;TrustServerCertificate=True;Connection Timeout=30;", new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    }));

                // Configure Hangfire server to listen to specific queues
                services.AddHangfireServer(options =>
                {
                    options.Queues = new[] { "critical", "default", "low-priority" };
                    // Configure number of workers 
                    /*
                     options.WorkerCount = 40; // Configure number of workers
                    //Default Calculation: Hangfire defaults to Environment.ProcessorCount * 5. If you have 8 logical processors, this would typically be 40 workers.
                    
                     */
                });
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .Build();

        // Start the host
        host.Start();

        ScheduleJobs();

        Console.WriteLine("Hangfire Server started. Press Ctrl+C to exit...");
        Thread.Sleep(Timeout.Infinite); // Keep the application running

        // Stop the host
        host.StopAsync().Wait();
    }

    static void ScheduleJobs()
    {
        // Enqueue 25 jobs in the 'critical' queue
        for (int i = 0; i < 250; i++)
        {
            int jobId = i;
            BackgroundJob.Enqueue(() => ExecuteJob("critical", jobId));
        }

        // Enqueue 35 jobs in the 'low-priority' queue
        for (int i = 250; i < 600; i++)
        {
            int jobId = i;
            BackgroundJob.Enqueue(() => ExecuteJob("low-priority", jobId));
        }

        // Enqueue 40 jobs in the 'default' queue
        for (int i = 600; i < 1000; i++)
        {
            int jobId = i;
            BackgroundJob.Enqueue(() => ExecuteJob("default", jobId));
        }
    }

    public static void ExecuteJob(string queue, int jobId)
    {
        // Log which worker is processing the job
        var workerId = Thread.CurrentThread.ManagedThreadId;
        Console.WriteLine($"Queue {queue}: Worker {workerId} is executing job {jobId}");
        // Simulate job execution
        Thread.Sleep(1000);
    }
}
