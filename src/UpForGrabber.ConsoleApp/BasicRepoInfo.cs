using System;
using System.Collections.Generic;
using System.Text;

namespace UpForGrabber.ConsoleApp
{
    public class BasicRepoInfo
    {
        public string RepoFullName { get; }
        public string RepoName { get; }
        public long RepoId { get; }

        public int StarCount { get; }
        public int ForkCount { get; }
        public int OpenIssueCount { get; }
        public DateTimeOffset LastUpdated { get; }

        public BasicRepoInfo(string repoFullName, string repoName, long repoId, int starCount, int forkCount, int openIssueCount, DateTimeOffset lastUpdated)
        {
            RepoFullName = repoFullName;
            RepoName = repoName;
            RepoId = repoId;
            StarCount = starCount;
            ForkCount = forkCount;
            OpenIssueCount = openIssueCount;
            LastUpdated = lastUpdated;
        }
    }
}
