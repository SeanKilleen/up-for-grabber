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
        private readonly PullRequestRequest _defaultPrRequest;
        public IStash Stash { get; set; }

        public GithubWorkerActor(string appName, string apiKey)
        {
            _defaultPrRequest = new PullRequestRequest { State = ItemStateFilter.All };

            _logger = Context.GetLogger();
            _logger.Info("Creating a Github actor for {appName} with key of {apiKey}", appName, apiKey);

            var tokenAuth = new Credentials(apiKey);
            _apiClient = new GitHubClient(new ProductHeaderValue(appName)) { Credentials = tokenAuth };
            _logger.Info("GithubApiClient created.");

            Become(Normal);
        }
        private void Normal()
        {
            ReceiveAsync<Messages.GetMembersOfGithubOrg>(async m =>
            {
                var users = await _apiClient.Organization.Member.GetAll(m.GithubOrgName);
                var userNames = users.Select(x => x.Login).ToList();
                CheckApiLimits(_apiClient.GetLastApiInfo());
                Sender.Tell(new Messages.UsernameList(userNames));
            });

            ReceiveAsync<Messages.GetPagedRepositoryContributors>(async m =>
            {
                _logger.Info("Getting page {pageNumber} of contributors for {repoFullName}", m.PageNumber, m.RepoFullName);

                var apiOptions = new ApiOptions
                {
                    PageSize = Constants.PR_PAGE_SIZE,
                    StartPage = m.PageNumber,
                    PageCount = 1
                };

                var contributors = await _apiClient.Repository.GetAllContributors(m.RepositoryId, false, apiOptions);

                CheckApiLimits(_apiClient.GetLastApiInfo());

                if (contributors.Any())
                {
                    _logger.Info("Contributors found, which means we'll try the next page too");
                    Context.Parent.Tell(new Messages.GetPagedRepositoryContributors(m.RepoFullName, m.RepositoryId, m.PageNumber + 1), Sender);

                    var formattedContributors = contributors.Select(x => new Messages.ContributorInfo(m.RepoFullName, x.Login, x.Contributions)).ToList();
                    Sender.Tell(new Messages.ContributorList(formattedContributors));
                }

                else
                {
                    _logger.Info("No contributors found for page {pageNumber} of repo ID {repoFullName} -- not taking further action", m.PageNumber, m.RepoFullName);
                }

            });

            ReceiveAsync<Messages.GetForksForUser>(async m =>
            {
                var result = await _apiClient.Repository.GetAllForUser(m.UserName);
                var forks = result.Where(x => x.Fork).ToList();
                CheckApiLimits(_apiClient.GetLastApiInfo());
                Sender.Tell(new Messages.ForkListForUser(m.UserName, forks));
            });

            ReceiveAsync<Messages.GetSourceForFork>(async m =>
            {
                var fullRepoInfo = await _apiClient.Repository.Get(m.Fork.Owner.Login, m.Fork.Name);
                var sourceRepo = fullRepoInfo.Source;
                CheckApiLimits(_apiClient.GetLastApiInfo());
                Sender.Tell(new Messages.SourceRepo(sourceRepo));
            });

            ReceiveAsync<Messages.GetPagedRepoPullRequests>(async m =>
            {
                _logger.Info("Getting page {pageNumber} of PRs from repo {repoFullName}", m.StartingPageNumber, m.RepoFullName);

                var apiOptions = new ApiOptions
                {
                    PageSize = Constants.PR_PAGE_SIZE,
                    StartPage = m.StartingPageNumber,
                    PageCount = 1
                };

                var pullRequests = await _apiClient.PullRequest.GetAllForRepository(m.RepositoryId, _defaultPrRequest, apiOptions);

                CheckApiLimits(_apiClient.GetLastApiInfo());

                if (pullRequests.Any())
                {
                    _logger.Info("PRs found, which means we'll try the next page too");
                    Context.Parent.Tell(new Messages.GetPagedRepoPullRequests(m.RepoFullName, m.RepositoryId, m.StartingPageNumber + 1), Sender);

                    var formattedInfo = pullRequests.Select(x => new Messages.PRInfo(m.RepoFullName, m.RepositoryId, x.Number, x.User.Login, x.Merged, x.MergedAt)).ToList();
                    Sender.Tell(new Messages.PRInfoList(formattedInfo));
                }
                else
                {
                    _logger.Info("No PRs found for page {pageNumber} of repo ID {repoFullName} -- not taking further action", m.StartingPageNumber, m.RepoFullName);
                }
            });
        }

        private void Paused()
        {
            Receive<Messages.Resume>(m =>
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
                Context.System.Scheduler.ScheduleTellOnce(timeSpanDifference, Self, new Messages.Resume(), Self);
                Become(Paused);
            }
        }

    }
}