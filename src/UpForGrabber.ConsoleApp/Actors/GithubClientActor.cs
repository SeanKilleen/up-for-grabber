using System;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;

namespace UpForGrabber.ConsoleApp.Actors
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GithubClientActor : ReceiveActor
    {
        // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
        private readonly ILoggingAdapter _logger;
        private readonly IActorRef _githubWorkerActor;
        // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable
        private int _numberOfGithubRequestsMade;

        public GithubClientActor()
        {
            _logger = Context.GetLogger();

            var actorsList = Constants.GetTokens().Select(pt => Context.ActorOf(Propmaster.GithubWorkerActor(pt))).ToList();
            _githubWorkerActor = Context.ActorOf(Props.Empty.WithRouter(routerConfig: new RoundRobinGroup(actorsList.Select(x=>x.Path.ToString()).ToList())), "githubWorker");

            Receive<Messages.Messages.LogRequestsMadeSoFar>(m =>
            {
                _logger.Info("So far, we've queued {numberOfGithubRequests} requests to the Github API", _numberOfGithubRequestsMade);
            });

            Receive<Messages.Messages.RetrieveRepos>(msg =>
            {
                _numberOfGithubRequestsMade++;
                _githubWorkerActor.Forward(msg);
            });

            Receive<Messages.Messages.RetrieveLabels>(msg =>
            {
                _numberOfGithubRequestsMade++;
                _githubWorkerActor.Forward(msg);
            });

            Receive<Messages.Messages.GetIssueCountPerLabel>(msg =>
            {
                _numberOfGithubRequestsMade++;
                _githubWorkerActor.Forward(msg);
            });

            _logger.Info("GithubClientActor created and sitting at {clientActorPath}", Self.Path);
            _logger.Info("Scheduling messages every 5 seconds to tell us how many requests we've made so far.", Self.Path);
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(5), Self, new Messages.Messages.LogRequestsMadeSoFar(), Self);
        }
    }
}