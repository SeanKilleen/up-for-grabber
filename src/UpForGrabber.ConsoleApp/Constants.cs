using System.Collections.Generic;

namespace UpForGrabber.ConsoleApp
{
    public static class Constants
    {
        public const int PR_PAGE_SIZE = 200;
        public const string APP_NAME = "Up-for-grabber";
        public const int GITHUB_CLIENT_COUNT = 1;

        public static Dictionary<string, string> GetPeopleAndTokens()
        {
            return new Dictionary<string, string>()
            {
                { "me", "90c75a1e73b762a81d11f5362002e7b6784a91ac" },
            };
        }

        public class ActorNames
        {
            public const string GITHUB_CLIENT_ACTOR_NAME = "githubClient";
        }
    }
}