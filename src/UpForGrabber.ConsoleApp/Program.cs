using System;
using System.Threading.Tasks;
using Akka.Actor;
using Serilog;
using UpForGrabber.ConsoleApp.Actors;

namespace UpForGrabber.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const string ORG_NAME = "microsoft"; //TODO: Extract into setting
            var logger = new LoggerConfiguration()
                .WriteTo.ColoredConsole()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();

            Log.Logger = logger;

            var actorSystem = ActorSystem.Create(Constants.APP_NAME, "akka { loglevel=INFO,  loggers=[\"Akka.Logger.Serilog.SerilogLogger, Akka.Logger.Serilog\"]}");
            var githubApiActor = actorSystem.ActorOf(Propmaster.GithubClientActor(), Constants.ActorNames.GITHUB_CLIENT_ACTOR_NAME);

            await actorSystem.WhenTerminated;
        }
    }
}