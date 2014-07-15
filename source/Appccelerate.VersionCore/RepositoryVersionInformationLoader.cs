﻿namespace Appccelerate.Version
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LibGit2Sharp;

    public class RepositoryVersionInformationLoader
    {
        public RepositoryVersionInformation GetRepositoryVersionInformation(string startingPath)
        {
            string repositoryPath = Repository.Discover(startingPath);

            var repository = new Repository(repositoryPath);

            RepositoryVersionInformation repositoryVersionInformation = this.GetRepositoryVersionInformation(repository);
            return repositoryVersionInformation;
        }

        public RepositoryVersionInformation GetRepositoryVersionInformation(Repository repository)
        {
            IEnumerable<Tag> allVersionTags = repository.Tags.Where(tag => tag.Name.StartsWith("v="));

            Commit lastTaggedCommit =
                repository.Head.Commits.FirstOrDefault(commit => allVersionTags.Any(tag => tag.Target == commit));

            if (lastTaggedCommit == null)
            {
                throw new InvalidOperationException("No version tag found. Add a tag with name 'v=<version pattern>'");
            }

            Tag latestVersionTag = allVersionTags.Last(tag => tag.Target.Sha == lastTaggedCommit.Sha);

            var qf = new CommitFilter
                         {
                             Since = repository.Head.Tip,
                             Until = lastTaggedCommit,
                             SortBy = CommitSortStrategies.Topological | CommitSortStrategies.Time
                         };

            int commits = repository.Commits.QueryBy(qf).Count();

            return new RepositoryVersionInformation(
                latestVersionTag.Name.Substring(2),
                commits,
                latestVersionTag.IsAnnotated ? latestVersionTag.Annotation.Message.Replace("\n", "").Replace("\r", "") : string.Empty);
        }
    }
}