﻿using System;
using System.Threading.Tasks;
using Microsoft;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Git.Controls.Commits;
using Microsoft.TeamFoundation.Git.CoreServices;
using Microsoft.TeamFoundation.Git.Provider;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;

namespace GitPull.Services
{
    public class TeamExplorerService : ITeamExplorerService
    {
        readonly IServiceProvider serviceProvider;

        public TeamExplorerService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task PullAsync()
        {
            var repositoryPath = FindActiveRepositoryPath();
            if (repositoryPath == null)
            {
                return;
            }

            var service = serviceProvider.GetService(typeof(SccService)) as SccService;
            Assumes.Present(service);
            var teamExplorer = service.GetSccService<ITeamExplorer>();
            var page = teamExplorer.NavigateToPage(new Guid(TeamExplorerPageIds.GitCommits), null);
            if (page == null)
            {
                Console.WriteLine("Page is currently null");
                return;
            }

            var gitCommitsPageView = page.PageContent as GitCommitsPageView;
            var gitCommitsPageViewModel = gitCommitsPageView.ViewModel as GitCommitsPageViewModel;
            await gitCommitsPageViewModel.PullAsync(repositoryPath);
        }

        public string FindActiveRepositoryPath()
        {
            return
                serviceProvider.GetService(typeof(IGitExt)) is IGitExt gitExt &&
                gitExt.ActiveRepositories is var repos &&
                repos.Count > 0 ? repos[0].RepositoryPath : null;
        }
    }
}
