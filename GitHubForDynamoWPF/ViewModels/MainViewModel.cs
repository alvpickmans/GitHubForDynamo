using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Configuration;
using Dynamo.Core;
using Dynamo.Extensions;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Octokit;
using GitHubForDynamoWPF.Helpers;

namespace GitHubForDynamoWPF.ViewModels
{
    public class MainViewModel : NotificationObject, IDisposable
    {
        #region Private Properties
        private ReadyParams readyParams;
        private string statusMessage;
        private string currentWorkSpaceName;
        private GitHubClient client;
        private User user;
        #endregion

        #region Public Prooperties
        public ObservableCollection<Repository> UserRepositories = new ObservableCollection<Repository>();

        public string StatusMessage
        {
            get
            {
                return statusMessage;
            }
            set
            {
                statusMessage = value;
                RaisePropertyChanged("StatusMessage");
            }
        }

        public string CurrentWorkSpaceName
        {
            get
            {
                return currentWorkSpaceName;
            }
            set
            {
                currentWorkSpaceName = value;
                RaisePropertyChanged("CurrentWorkSpaceName");
            }
        }

        public User User
        {
            get
            {
                return user;
            }
            set
            {
                user = value;
                RaisePropertyChanged("User");
            }
        }

        public GitHubClient Client
        {
            get
            {
                return client;
            }
            set
            {
                client = value;
                RaisePropertyChanged("Client");
            }
        }
        #endregion

        #region Constructor
        public MainViewModel(ReadyParams p)
        {
            readyParams = p;
            client = new GitHubClient(new ProductHeaderValue("github-for-dynamo"));
            client.SetRequestTimeout(new TimeSpan(0, 0, 10));

            // Seems that CurrentWorkSpace only gets updated when a new file is opened, but nor when closed.
            readyParams.CurrentWorkspaceChanged += CurrentWorkspaceChanged;
            CurrentWorkspaceChanged(readyParams.CurrentWorkspaceModel);

        }
        #endregion

        #region Public Methods

        public void SignOut()
        {
            ConfigurationManager.AppSettings.Set("User", String.Empty);
            ConfigurationManager.AppSettings.Set("Password", String.Empty);

            this.client.Credentials = null;
            //this.client.SetCurrentUser(this);
        }

        #endregion

        #region Events
        private void CurrentWorkspaceChanged(Dynamo.Graph.Workspaces.IWorkspaceModel obj)
        {
            CurrentWorkSpaceName = obj.Name;
        } 
        #endregion

        public void Dispose()
        {
            readyParams.CurrentWorkspaceChanged -= CurrentWorkspaceChanged;
        }
    }
}
