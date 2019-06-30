using Akka.Actor;

namespace UpForGrabber.ConsoleApp.Actors
{
    public class GithubRepoActor : ReceiveActor
    {
        private readonly BasicRepoInfo _repoInfo;

        public GithubRepoActor(BasicRepoInfo repoInfo)
        {
            _repoInfo = repoInfo;

            Become(Normal);

            // TODO: Tell self to start examining labels
        }

        private void Normal()
        {
        }
    }
}
