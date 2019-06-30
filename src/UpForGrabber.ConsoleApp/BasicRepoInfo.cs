using System;
using System.Collections.Generic;
using System.Text;

namespace UpForGrabber.ConsoleApp
{
    public class BasicRepoInfo
    {
        public string RepoFullName { get; }
        public long RepoId { get; }

        public BasicRepoInfo(string repoFullName, long repoId)
        {
            RepoFullName = repoFullName;
            RepoId = repoId;
        }
    }
}
