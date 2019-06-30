using Akka.Actor;
using Akka.Event;

namespace UpForGrabber.ConsoleApp.Actors
{
    public class GithubRepoActor : ReceiveActor
    {
        private readonly BasicRepoInfo _repoInfo;
        private readonly ILoggingAdapter _logger;
        private readonly ActorSelection _githubClient;

        public GithubRepoActor(BasicRepoInfo repoInfo)
        {
            _logger = Context.GetLogger();
            _repoInfo = repoInfo;
            _githubClient = Context.ActorSelection("/user/" + Constants.ActorNames.GITHUB_CLIENT_ACTOR_NAME);

            Become(Normal);
            _logger.Info("Created actor at {Path}", Self.Path);

            _githubClient.Tell(new Messages.Messages.RetrieveLabels(_repoInfo.RepoId));
            // TODO: Tell self to start examining labels
        }

        private void Normal()
        {
            Receive<Messages.Messages.LabelsForRepo>(msg =>
            {
                
            });
        }
    }
}
