using Akka.Actor;
using Akka.Event;
using Akka.Logger.Serilog;

namespace UpForGrabber.ConsoleApp.Actors
{
    public class GithubRepoActor : ReceiveActor
    {
        private readonly BasicRepoInfo _repoInfo;
        private readonly ILoggingAdapter _logger;
        public GithubRepoActor(BasicRepoInfo repoInfo)
        {
            _logger = Context.GetLogger();
            _repoInfo = repoInfo;

            Become(Normal);
            _logger.Info("Created actor at {Path}", Self.Path);

            // TODO: Tell self to start examining labels
        }

        private void Normal()
        {
        }
    }
}
