using System.Reflection.Metadata;
using Akka.Actor;
using Akka.Configuration;
using Microsoft.Extensions.Configuration;
using Octokit;
using Octokit.Internal;

public class GitHubAPIActor : ReceiveActor
{
    private readonly IActorRef _octokitActor;
    public GitHubAPIActor()
    {
        _octokitActor = Context.ActorOf(Props.Create<OctokitActor>(), "octokit");
        
        Receive<GetGitHubRepos>(msg =>
        {
            _octokitActor.Tell(msg, Sender);
        });
    }
}

public class OctokitActor : ReceiveActor
{
    private IGitHubClient _octokitClient;
    public OctokitActor()
    {
        var config = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        var ghToken = config["UPFORGRABBER_GITHUB_TOKEN"];

        _octokitClient = new GitHubClient(new ProductHeaderValue("Up-for-grabber"),
            new InMemoryCredentialStore(new Credentials(ghToken)));

        ReceiveAsync<GetGitHubRepos>(async msg =>
        {
            var repo = await _octokitClient.Repository.GetAllForOrg(msg.OrgName, new ApiOptions());
        });
    }
}