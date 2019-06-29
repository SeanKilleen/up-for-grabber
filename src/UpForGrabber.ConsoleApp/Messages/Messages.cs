using System;
using System.Collections.Generic;
using Octokit;

namespace UpForGrabber.ConsoleApp
{
    public static class Messages
    {
        public class BuildPortfolioFor
        {
            public string GithubOrgName { get; }

            public BuildPortfolioFor(string githubOrgName)
            {
                GithubOrgName = githubOrgName;
            }
        }

        public class ContributorInfo
        {
            public string RepoFullName { get; }
            public string UserName { get; }
            public int TotalContributions { get; }

            public ContributorInfo(string repoFullName, string userName, int totalContributions)
            {
                RepoFullName = repoFullName;
                UserName = userName;
                TotalContributions = totalContributions;
            }
        }
        public class ContributorList
        {
            public IReadOnlyList<ContributorInfo> Contributors { get; }

            public ContributorList(IReadOnlyList<ContributorInfo> contributors)
            {
                Contributors = contributors;
            }
        }
        public class GetPagedRepositoryContributors
        {
            public string RepoFullName { get; }
            public long RepositoryId { get; }
            public int PageNumber { get; }

            public GetPagedRepositoryContributors(string repoFullName, long repositoryId, int pageNumber)
            {
                RepoFullName = repoFullName;
                RepositoryId = repositoryId;
                PageNumber = pageNumber;
            }
        }
        public class GetMembersOfGithubOrg
        {
            public string GithubOrgName { get; }

            public GetMembersOfGithubOrg(string githubOrgName)
            {
                GithubOrgName = githubOrgName;
            }
        }

        public class UsernameList
        {
            public IReadOnlyList<string> UserNames { get; set; }
            public UsernameList(List<string> userNames)
            {
                UserNames = userNames;
            }
        }

        public class AddUserToOrg
        {
            public string UserName { get; }
            public AddUserToOrg(string userName)
            {
                UserName = userName;
            }
        }

        public class AllUsersAdded
        {
        }

        public class Resume
        {
        }

        public class RetrieveForks
        {
        }

        public class GetForksForUser
        {
            public string UserName { get; }
            public GetForksForUser(string username)
            {
                UserName = username;
            }
        }

        public class ForkListForUser
        {
            public string UserName { get; }
            public IReadOnlyList<Repository> Forks { get; } 
            public ForkListForUser(string userName, IReadOnlyList<Repository> forks)
            {
                UserName = userName;
                Forks = forks;
            }
        }

        public class RetrieveSourceRepos
        {
        }

        public class RetrieveSourceReposForForks
        {
            public IReadOnlyList<Repository> Forks { get; } 
            public RetrieveSourceReposForForks(IReadOnlyList<Repository> forks)
            {
                Forks = forks;
            }
        }

        public class GetSourceForFork
        {
            public Repository Fork { get; set; }
            public GetSourceForFork(Repository fork)
            {
                Fork = fork;
            }
        }

        public class SourceRepo
        {
            public Repository SourceRepository { get; }
            public SourceRepo(Repository sourceRepo)
            {
                SourceRepository = sourceRepo;
            }
        }

        public class PRInfoList
        {
            public IReadOnlyList<PRInfo> PrInfoList { get; }

            public PRInfoList(List<PRInfo> prInfoList)
            {
                PrInfoList = prInfoList;
            } 
        }
        public class GetPagedRepoPullRequests
        {
            public string RepoFullName { get; }
            public long RepositoryId { get; }
            public int StartingPageNumber { get; }
            public GetPagedRepoPullRequests(string repoFullName, long repositoryId, int startingPageNumber)
            {
                RepoFullName = repoFullName;
                RepositoryId = repositoryId;
                StartingPageNumber = startingPageNumber;
            }
        }

        public class PRInfo
        {
            public string RepoFullName { get; }
            public long RepoId { get; }
            public int PullRequestId { get; }
            public string AuthorLogin { get; }
            public bool IsMerged { get; }
            public DateTimeOffset? PRMergeDate { get; }
            public PRInfo(string repoFullName, long repoId, int pullRequestId, string authorLogin, bool isMerged, DateTimeOffset? prMergeDate)
            {
                RepoFullName = repoFullName;
                RepoId = repoId;
                PullRequestId = pullRequestId;
                AuthorLogin = authorLogin;
                IsMerged = isMerged;
                PRMergeDate = prMergeDate;
            }
        }

        public class LogRequestsMadeSoFar
        {
        }
    }
}