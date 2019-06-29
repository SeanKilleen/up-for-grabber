using System.Collections.Generic;
using Akka.Actor;

public class GitHubOrgActor : ReceiveActor
{
    List<IActorRef> _orgRepos;
    ActorSelection _githubActor;
    string _orgName;
    IReadOnlyList<string> _repoNames;
    public GitHubOrgActor(string orgName)
    {
        _orgName = orgName;
        _orgRepos = new List<IActorRef>();
        _githubActor = Context.ActorSelection("/user/githubApi");

        Receive<ReposDiscovered>(msg=> {

        });

        _githubActor.Tell(new GetGitHubRepos(orgName));
    }

    public class ReposDiscovered {
        string OrgName {get;}
        IReadOnlyList<string> RepoNames {get;}

        public ReposDiscovered(string orgName, List<string> repoNames){
            OrgName = orgName;
            RepoNames = repoNames;
        }

    }
}