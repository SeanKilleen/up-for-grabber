using System;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Octokit;

namespace UpForGrabber.ConsoleApp.Actors
{
    public class GithubWorkerActor : ReceiveActor, IWithUnboundedStash
    {
        private readonly GitHubClient _apiClient;
        private readonly ILoggingAdapter _logger;
        public IStash Stash { get; set; }

        public GithubWorkerActor(string appName, string apiKey)
        {
            _logger = Context.GetLogger();
            _logger.Info("Creating a Github actor for {appName} with key of {apiKey}", appName, apiKey);

            var tokenAuth = new Credentials(apiKey);
            _apiClient = new GitHubClient(new ProductHeaderValue(appName)) { Credentials = tokenAuth };
            _logger.Info("GithubApiClient created.");

            Become(Normal);
        }
        private void Normal()
        {
            ReceiveAsync<Messages.Messages.RetrieveRepos>(async msg =>
            {
                _logger.Info("Retrieving repos for {OrgName} org", msg.OrgName);

                var repos = await _apiClient.Repository.GetAllForOrg(msg.OrgName);

                _logger.Info("Retrieved {TotalRepoCount} total repos for {OrgName} org on page", repos.Count, msg.OrgName);
                var eligibleRepos = repos.Where(x =>
                    !x.Private &&
                    !x.Archived &&
                    x.HasIssues
                ).Select(x=>new BasicRepoInfo(x.FullName, x.Name, x.Id, x.StargazersCount, x.ForksCount, x.OpenIssuesCount, x.UpdatedAt)).ToList();

                _logger.Info("After filtering, there are {EligibleRepoCount} eligible repos for {OrgName} on page", eligibleRepos.Count, msg.OrgName);
                CheckApiLimits(_apiClient.GetLastApiInfo());
                Sender.Tell(new Messages.Messages.ReposForOrganization(eligibleRepos));
            });

            ReceiveAsync<Messages.Messages.RetrieveLabels>(async msg =>
            {
                var labels = await _apiClient.Issue.Labels.GetAllForRepository(msg.RepoId);
                var selectedLabelInfo = labels.Select(x => x.Name).ToList();

                CheckApiLimits(_apiClient.GetLastApiInfo());
                Sender.Tell(new Messages.Messages.LabelsForRepo(selectedLabelInfo));
            });
        }

        private void Paused()
        {
            Receive<Messages.Messages.Resume>(m =>
            {
                _logger.Info("Resuming github worker activity.");
                Stash.UnstashAll();
                Become(Normal);
            });
            Receive<object>(m =>
            {
                _logger.Info("Received a message but am currently paused; will stash for later retrieval.");
                Stash.Stash();
            });
        }

        private void CheckApiLimits(ApiInfo apiInfo)
        {
            if (apiInfo == null)
            {
                _logger.Warning("Received null ApiInfo -- likely the first request, though.");
                return;
            }

            _logger.Info("We have {numberRemaining} requests remaining before a reset at {resetTime}", apiInfo.RateLimit.Remaining, apiInfo.RateLimit.Reset);

            if (apiInfo.RateLimit.Remaining <= Constants.GITHUB_CLIENT_COUNT)
            {
                _logger.Warning("Only {numberOfRemainingRequests} requests remaining; pausing until {resetTime}.", apiInfo.RateLimit.Remaining, apiInfo.RateLimit.Reset);

                var timeSpanDifference = apiInfo.RateLimit.Reset - DateTimeOffset.Now;
                timeSpanDifference = timeSpanDifference + TimeSpan.FromSeconds(5); // add an additional 5 seconds just to be safe.

                _logger.Info("Looks like we'll have to wait for {secondsToWait} seconds. Scheduling resume message now.", timeSpanDifference.TotalSeconds);
                Context.System.Scheduler.ScheduleTellOnce(timeSpanDifference, Self, new Messages.Messages.Resume(), Self);
                Become(Paused);
            }
        }

    }
}