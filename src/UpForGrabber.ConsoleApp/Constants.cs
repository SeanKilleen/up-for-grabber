using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace UpForGrabber.ConsoleApp
{
    public static class Constants
    {
        public const string GITHUB_TOKEN_ENV_VAR_NAME = "UPFORGRABBER_GITHUB_TOKEN";
        public const int DEFAULT_PAGE_SIZE = 200;
        public const string APP_NAME = "Up-for-grabber";
        public const int GITHUB_CLIENT_COUNT = 1;
        public const string ORG_NAME_TO_CHECK = "microsoft"; //TODO: Extract to settings

        public static List<string> GetTokens()
        {
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var tokensFromConfig = config[Constants.GITHUB_TOKEN_ENV_VAR_NAME];
            var tokens = tokensFromConfig.Split(',', System.StringSplitOptions.RemoveEmptyEntries);

            return new List<string>(tokens);
        }

        public class ActorNames
        {
            public const string GITHUB_CLIENT_ACTOR_NAME = "githubClient";
        }
    }
}