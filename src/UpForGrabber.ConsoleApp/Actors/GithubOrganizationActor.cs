using System.Runtime.CompilerServices;
using Akka.Actor;

namespace UpForGrabber.ConsoleApp.Actors
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GithubOrganizationActor : ReceiveActor
    {
        // ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
        private readonly ActorSelection _githubClient;
        private readonly string _orgName;
        // ReSharper restore PrivateFieldCanBeConvertedToLocalVariable

        public GithubOrganizationActor(string orgNameToCheck)
        {
            _orgName = orgNameToCheck;
            _githubClient = Context.ActorSelection("/user/" + Constants.ActorNames.GITHUB_CLIENT_ACTOR_NAME);

            Become(Normal);

            Self.Tell(new Messages.Messages.RetrieveRepos(_orgName));
        }

        private void Normal()
        {
            Receive<Messages.Messages.RetrieveRepos>(msg =>
            {
                _githubClient.Tell(msg);
            });

            Receive<Messages.Messages.ReposForOrganization>(msg =>
            {

            });

        }
    }
}
