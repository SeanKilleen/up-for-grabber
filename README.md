# up-for-grabber

A tool to search an organization for up-for-grabs style repositories to recommend to up-for-grabs.net.

[![Build Status](https://dev.azure.com/sjkilleen/up-for-grabber/_apis/build/status/SeanKilleen.up-for-grabber?branchName=master)](https://dev.azure.com/sjkilleen/up-for-grabber/_build/latest?definitionId=2&branchName=master)

## How the tool works / will work (logic-wise)

* Gets all the repositories for an organization that are active, not archived, etc.
* Gets all of the labels that those repositories use
* Attempts to match those labels to the types of labels we find on <http://up-for-grabs.net>. If no labels are found, stops looking at that repository.
* Finds all the issues with those labels. If no issues are found, stops looking at that repository.
* Spits out information about the number issues in a repository with up for grabs tags, as well as the freshness & popularity of the repository. Currently this is done via log messages but in the future we'll do something more advanced.

## How the tool works (tech-wise)

* Uses Octokit .NET to obtain information from GitHub.
* Uses Akka .NET to communicate and send requests through to GitHub.
* Uses Akka's Become/Unbecome and Octokit's API checking to pause itself before it hits api limitations. This way it can pick up where it left off.
* Uses Akka.NET routers to create a github client for every token we have, and runs things in parallel.
* The app is built using .NET Core.

## Using the tool

You'll need to:

* Generate a GitHub token. NOTE: This works best if you have more than one GitHub token because they can operate in parallel and will make it less likely for you to hit an API rate limit. If you do get close to the rate limit, the app will pause & resume.
* It's also a good idea to install [Seq](http://getseq.net), a fantastic application for looking through the log output that we're generating. If you install Seq with the default parameters, we'll output right to it on `localhost:5341`.
* Set an environment variable called `UPFORGRABBER_GITHUB_TOKEN` to the token(s) you generated; if more than one, comma-separate them.
* Set the constant variable for the org you want to check (we'll extract this to a setting at some point)
* Build / run the console app. Kill it off when it looks like it's done.
* Check out the logging output in Seq, where you can query it for numbers etc.