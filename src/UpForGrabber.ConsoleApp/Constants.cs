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
        public const string ORG_NAME_TO_CHECK = "excellalabs"; //TODO: Extract to settings

        public static Dictionary<string, string> GetPeopleAndTokens()
        {
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var tokenFromConfig = config[Constants.GITHUB_TOKEN_ENV_VAR_NAME];

            return new Dictionary<string, string>
            {
                { "me", tokenFromConfig }
            };
        }

        public class ActorNames
        {
            public const string GITHUB_CLIENT_ACTOR_NAME = "githubClient";
        }
    }
}