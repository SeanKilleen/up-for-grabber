using System.Collections.Generic;
using Akka.Actor;

public class GitHubOrgActor : ReceiveActor
{
    List<IActorRef> _orgRepos;
    readonly ActorSelection _githubActor;
    string _orgName;
    IReadOnlyList<string> _repoNames;
    public GitHubOrgActor(string orgName)
    {
        _orgName = orgName;
        _orgRepos = new List<IActorRef>();
        _githubActor = Context.ActorSelection("/user/githubApi");

        Receive<ReposDiscovered>(msg =>
        {
            foreach (var repo in msg.RepoNames)
            {
                var repoRef = Context.ActorOf(Props.Create<RepoActor>(repo));
                _orgRepos.Add(repoRef);
            }
        });

        _githubActor.Tell(new GetGitHubRepos(orgName));
    }

    public class ReposDiscovered {
        public string OrgName {get;}
        public IReadOnlyList<string> RepoNames {get;}

        public ReposDiscovered(string orgName, List<string> repoNames){
            OrgName = orgName;
            RepoNames = repoNames;
        }

    }
}

public class RepoActor : ReceiveActor
{
    public RepoActor()
    {

    }
}