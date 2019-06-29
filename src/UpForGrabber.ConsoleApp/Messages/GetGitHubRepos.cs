using Akka.Actor;

public class GetGitHubRepos : ReceiveActor
{
    public string OrgName {get;}
    public GetGitHubRepos(string orgName)
    {
        OrgName = orgName;
    }
}