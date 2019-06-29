using Akka.Actor;
using UpForGrabber.ConsoleApp.Actors;

namespace UpForGrabber.ConsoleApp
{
    public static class Propmaster
    {
        public static Props GithubWorkerActor(string token) => Props.Create(() => new GithubWorkerActor(Constants.APP_NAME, token));
    }
}