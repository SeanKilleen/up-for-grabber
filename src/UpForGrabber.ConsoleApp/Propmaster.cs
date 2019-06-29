using Akka.Actor;
using UpForGrabber.ConsoleApp.Actors;

namespace UpForGrabber.ConsoleApp
{
    public static class Propmaster
    {
        public static Props GithubWorkerActor(string token) => Props.Create(() => new GithubWorkerActor(Constants.APP_NAME, token));
        public static Props GithubClientActor() => Props.Create<GithubClientActor>();
        public static Props GithubOrgActor(string orgNameToCheck) => Props.Create<GithubOrganizationActor>(orgNameToCheck);
    }
}