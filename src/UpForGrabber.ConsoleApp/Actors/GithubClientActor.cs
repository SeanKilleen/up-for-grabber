using System;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using Akka.Util.Internal;

namespace UpForGrabber.ConsoleApp.Actors
{
    public class GithubClientActor : ReceiveActor
    {
        private readonly ILoggingAdapter _logger;
        private readonly IActorRef _githubWorkerActor;
        private int _numberOfGithubRequestsMade;

        public GithubClientActor()
        {
            _logger = Context.GetLogger();

            var actorsList = Constants.GetPeopleAndTokens().Select(pt => Context.ActorOf(Propmaster.GithubWorkerActor(pt.Value), pt.Key)).ToList();
            _githubWorkerActor = Context.ActorOf(Props.Empty.WithRouter(new RoundRobinGroup(actorsList)), "githubWorker");

            Receive<Messages.LogRequestsMadeSoFar>(m =>
            {
                _logger.Info("So far, we've queued {numberOfGithubRequests} requests to the Github API", _numberOfGithubRequestsMade);
            });

            Receive<Messages.GetPagedRepositoryContributors>(m =>
            {
                _numberOfGithubRequestsMade++;
                _githubWorkerActor.Forward(m);
            });
            Receive<Messages.GetMembersOfGithubOrg>(m =>
            {
                _numberOfGithubRequestsMade++;
                _githubWorkerActor.Forward(m);
            });

            Receive<Messages.GetForksForUser>(m =>
            {
                _numberOfGithubRequestsMade++;
                _githubWorkerActor.Forward(m);
            });

            Receive<Messages.GetPagedRepoPullRequests>(m =>
            {
                _numberOfGithubRequestsMade++;
                _githubWorkerActor.Forward(m);
            });

            Receive<Messages.RetrieveSourceReposForForks>(m =>
            {
                foreach (var fork in m.Forks)
                {
                    _numberOfGithubRequestsMade++;
                    _githubWorkerActor.Tell(new Messages.GetSourceForFork(fork), Sender);
                }
            });

            Receive<Messages.RetrieveRepos>(msg =>
            {
                _numberOfGithubRequestsMade++;
                _githubWorkerActor.Forward(msg);
            });

            _logger.Info("GithubClientActor created and sitting at {clientActorPath}", Self.Path);
            _logger.Info("Scheduling messages every 5 seconds to tell us how many requests we've made so far.", Self.Path);
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(5), Self, new Messages.LogRequestsMadeSoFar(), Self);
        }
    }
}