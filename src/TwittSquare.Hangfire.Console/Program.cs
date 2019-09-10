using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.PostgreSql;
using TwittSquare.Hangfire.Task;

namespace TwittSquare.Hangfire.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            GlobalConfiguration.Configuration.UseStorage(new PostgreSqlStorage("Server=localhost;Port=5432;Database=twitter;User Id=twittsquare;Password=12345678;"));

            RecurringJob.AddOrUpdate(TaskId.DeleteExpirationLoginId, () => new LoginTask().DeleteExpirationLogin(), Cron.Hourly);
            RecurringJob.AddOrUpdate(TaskId.UserTaskCheckId, () => new UserTask().TaskCheck(), Cron.Hourly);

            using (var server = new BackgroundJobServer(new BackgroundJobServerOptions() { ServerName = $"{Environment.MachineName}.{Guid.NewGuid().ToString()}" }))
            {
                System.Console.WriteLine("Hangfire Server started. Press any key to exit...");
                while (true)
                {
                    try
                    {
                        System.Console.ReadKey();
                        return;
                    }
                    catch (Exception) { }
                }
            }
        }
    }
}
