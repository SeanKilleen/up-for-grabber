﻿using System;
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

                _githubClient.Tell(new Messages.Messages.GetIssueCountPerLabel(_repoInfo.RepoId, _repoInfo.RepoFullName, _upForGrabsLabels));
            });

            Receive<Messages.Messages.LabelsAndIssuesResponse>(msg => {
                var countsPerLabel = msg.LabelsAndIssues.ToDictionary(k => k.Key, v => v.Value.Count);
                var mostRecentIssuePerLabel = msg.LabelsAndIssues.ToDictionary(k => k.Key, v => SelectLatestDate(v.Value));

                var totalUFGIssueCount = countsPerLabel.Sum(x=>x.Value);
                var totalUFGLabelCount = countsPerLabel.Count;

                var mostRecentUfgIssue = mostRecentIssuePerLabel.OrderByDescending(x=>x.Value).First().Value; 
                var dateDistnce = (DateTimeOffset.UtcNow - mostRecentUfgIssue).Days;

                _logger.Info("Counts per label for {RepoName}: {CountsPerLabel}", _repoInfo.RepoFullName, countsPerLabel);
                _logger.Info("LatestIssue per label for {RepoName}: {MostRecentIssuePerLabel}", _repoInfo.RepoFullName, mostRecentIssuePerLabel);

                _logger.Info("Repo report: {RepoName} has {StarCount} stars. It has {TotalOpenIssues} open issues, {TotalUFGIssueCount} of which exist across {LabelCount} up-for-grabs style labels. The most recent up-for-grabs issue was on {MostRecentUpForGrabsDate}, {DaysAgo} days ago.",
                    _repoInfo.RepoFullName, _repoInfo.StarCount, _repoInfo.OpenIssueCount, totalUFGIssueCount, totalUFGLabelCount, mostRecentUfgIssue, dateDistnce
                );
            });
        }

        private DateTimeOffset SelectLatestDate(List<IssueInfo> items){
            return items
                .Select(x=> {
                    if (x.Updated) 
                    {
                        return x.UpdatedAt.Value;
                    } 
                    else 
                    {
                        return x.CreatedAt;
                    }})
                .OrderByDescending(x => x)
                .First();
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
