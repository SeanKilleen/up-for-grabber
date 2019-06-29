﻿using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;

namespace UpForGrabber.ConsoleApp.Actors
{
    public class GithubOrganizationActor : ReceiveActor
    {
        private ActorSelection _githubClient;

        public GithubOrganizationActor()
        {
            _githubClient = Context.ActorSelection("/user/" + Constants.ActorNames.GITHUB_CLIENT_ACTOR_NAME);

            Receive<Messages.RetrieveRepos>(msg =>
            {
                _githubClient.Tell(msg);
            });
        }
    }
}