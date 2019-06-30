using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;

namespace UpForGrabber.ConsoleApp.Actors
{
    public class GithubRepoActor : ReceiveActor
    {
        private readonly BasicRepoInfo _repoInfo;
        private readonly ILoggingAdapter _logger;
        private readonly ActorSelection _githubClient;
        private List<string> _upForGrabsLabels;

        public GithubRepoActor(BasicRepoInfo repoInfo)
        {
            _logger = Context.GetLogger();
            _repoInfo = repoInfo;
            _githubClient = Context.ActorSelection("/user/" + Constants.ActorNames.GITHUB_CLIENT_ACTOR_NAME);

            Become(Normal);
            _logger.Info("Created actor at {Path}", Self.Path);

            _githubClient.Tell(new Messages.Messages.RetrieveLabels(_repoInfo.RepoId));
            // TODO: Tell self to start examining labels
        }

        private void Normal()
        {
            Receive<Messages.Messages.LabelsForRepo>(msg =>
            {
                var matchingLabels = msg.Labels.Where(LabelIsPotentiallyUpForGrabs).ToList();

                if (!matchingLabels.Any())
                {
                    _logger.Info("No matching up-for-grabs labels for {RepoName} -- stopping", _repoInfo.RepoFullName);
                    return;
                }

                _logger.Info("Found {MatchingLabelCount} potential up-for-grabs labels for {RepoName}: {LabelsList}", matchingLabels.Count, _repoInfo.RepoFullName, matchingLabels);

                _upForGrabsLabels = matchingLabels;
            });
        }

        private static bool LabelIsPotentiallyUpForGrabs(string name)
        {
            return ThingsToSearch.Any(x => name.Contains(x, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <remarks>Brainstormed from up-for-grabs.net</remarks>
        private static readonly List<string> ThingsToSearch = new List<string>
        {
            // Used jQuery on up-for-grabs: $("p.label > a").each(function(index){console.log($(this).text())})
            "up for",
            "up-for",
            "first",
            "help",
            "bitesize",
            "teaser",
            "easy",
            "novice",
            "trivial",
            "junior",
            "jump",
            "first",
            "newbie",
            "quick",
            "welcome",
            "low-hanging",
            "low hanging",
            "low_hanging",
            "you take it",
            "new",
            "accepting"
        };

    }
}
