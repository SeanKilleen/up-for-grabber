using Akka.Actor;

namespace UpForGrabber.ConsoleApp.Actors
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GithubOrganizationActor : ReceiveActor
    {
        private readonly ActorSelection _githubClient;
        private readonly string _orgName; 

        public GithubOrganizationActor(string orgNameToCheck)
        {
            _orgName = orgNameToCheck;
            _githubClient = Context.ActorSelection("/user/" + Constants.ActorNames.GITHUB_CLIENT_ACTOR_NAME);

            Receive<Messages.RetrieveRepos>(msg =>
            {
                _githubClient.Tell(msg);
            });

            Self.Tell(new Messages.RetrieveRepos(_orgName));
        }
    }
}
