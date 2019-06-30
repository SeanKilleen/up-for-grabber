using System;
using System.Collections.Generic;
using Octokit;

namespace UpForGrabber.ConsoleApp.Messages
{
    public static class Messages
    {
        public class Resume
        {
        }

        public class LogRequestsMadeSoFar
        {
        }

        public class RetrieveRepos
        {
            public string OrgName { get; }
            public RetrieveRepos(string orgName)
            {
                OrgName = orgName;
            }
        }

        public class ReposForOrganization
        {
            public IReadOnlyList<BasicRepoInfo> Repositories { get; }
            public ReposForOrganization(List<BasicRepoInfo> repos)
            {
                Repositories = repos;
            }
        }

        public class RetrieveLabels
        {
            public long RepoId { get; }
            public RetrieveLabels(long repoId)
            {
                RepoId = repoId;
            }
        }

        public class LabelsForRepo
        {
            public IReadOnlyList<string> Labels { get; }
            public LabelsForRepo(List<string> labels)
            {
                Labels = labels;
            }
        }
    }
}