using System;
using System.Threading.Tasks;
using Akka.Actor;

namespace UpForGrabber.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            const string SYSTEM_NAME = "upforgrabber";
            var system = ActorSystem.Create(SYSTEM_NAME);

            var apiActorProps = Props.Create<GitHubAPIActor>();
            var githubApiActor = system.ActorOf(apiActorProps, "githubApi");

            await system.WhenTerminated;
        }
    }
}