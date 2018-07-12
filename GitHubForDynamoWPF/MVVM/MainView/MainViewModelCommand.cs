using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Dynamo.UI.Commands;
using LibGit2Sharp;
using MahApps.Metro.Controls;
using GitHubForDynamoWPF.Helpers;

namespace GitHubForDynamoWPF.ViewModels
{
    public partial class MainViewModel
    {
        public void InitializeDelegateCommands()
        {
            ToggleFlyOutCommand = new DelegateCommand(ToggleFlyOut, CanToggleFlyOut);
            CreateNewProjectCommand = new DelegateCommand(CreateNewProject, CanCreateNewProject);
            AddExistingProjectCommand = new DelegateCommand(AddExistingProject, CanAddExistingProject);
        }
        public DelegateCommand ToggleFlyOutCommand { get; set; }
        public DelegateCommand CreateNewProjectCommand { get; set; }
        public DelegateCommand AddExistingProjectCommand { get; set; }

        private void ToggleFlyOut(object parameters)
        {
            Flyout flyout = parameters as Flyout;
            flyout.ToggleFlyout();
        }

        private bool CanToggleFlyOut(object parameters)
        {
            return true;
        }

        private void CreateNewProject(object parameters)
        {
            MessageBox.Show("New project");
        }

        private bool CanCreateNewProject(object parameters)
        {
            return true;
        }

        private void AddExistingProject(object parameters)
        {
            string path = parameters as string;
            using(var repo = new Repository(path))
            {
                foreach (var item in repo.RetrieveStatus(new LibGit2Sharp.StatusOptions()))
                {
                    System.Diagnostics.Debug.WriteLine(item.FilePath);
                }
            }
        }

        private bool CanAddExistingProject(object parameters)
        {
            return true;
        }
    }
}
