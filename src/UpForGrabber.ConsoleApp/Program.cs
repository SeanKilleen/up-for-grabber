using System;
using System.Threading.Tasks;
using Akka.Actor;

namespace UpForGrabber.ConsoleApp
{
    class Program
    {
        static async Task  Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var system = ActorSystem.Create("upforgrabber");
            var githubApiActor = system.ActorOf(Props.Create<GitHubAPIActor>(), "githubApi");

            await system.WhenTerminated;
        }
    }
}