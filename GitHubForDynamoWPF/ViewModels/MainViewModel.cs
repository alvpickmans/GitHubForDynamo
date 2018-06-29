using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.ObjectModel;
using System.Configuration;
using Dynamo.Core;
using Dynamo.Extensions;
using Dynamo.ViewModels;
using Dynamo.UI;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using System.Net.Http;
using Octokit;
using GitHubForDynamoWPF.Helpers;
using GitHubForDynamoWPF.Attributes;
using System.Windows.Forms;

namespace GitHubForDynamoWPF.ViewModels
{
    [CustomEnum(true)]
    public enum LogStatus
    {
        [TextValue("Loggin in to GitHub...")]
        Initiated,
        [TextValue("Log in completed.")]
        Completed,
        [TextValue("Something went wrong :(")]
        Failed,
        [TextValue("Logged")]
        Logged,
        [TextValue("Bye bye!")]
        Unlogged
    }

    public partial class MainViewModel : NotificationObject, IDisposable
    {
        #region Private Properties
        private ReadyParams readyParams;
        private string uniqueId;
        private string extensionName;
        private Window owner;
        private DynamoViewModel dynamoVM;
        private LogStatus logStatus;
        private string loginMessage;
        private string currentWorkSpaceName;
        private string userName;
        private GitHubClient client;
        private User user;
        private bool isSigned;
        #endregion

        #region Public Prooperties
        public ObservableCollection<Repository> UserRepositories = new ObservableCollection<Repository>();

        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                RaisePropertyChanged("UserName");
            }
        }

        public LogStatus LogStatus
        {
            get { return logStatus; }
            set
            {
                logStatus = value;
                RaisePropertyChanged("LogStatus");
                LoginMessage = value.TextValue();
            }
        }

        public string LoginMessage
        {
            get
            {
                return loginMessage;
            }
            set
            {
                loginMessage = value;
                RaisePropertyChanged("LoginMessage");
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

        public bool IsSigned
        {
            get { return isSigned; }
            set
            {
                isSigned = value;
                RaisePropertyChanged("IsSigned");
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
        public MainViewModel(ReadyParams p, Window owner,string id, string extName)
        {
            readyParams = p;
            uniqueId = id;
            extensionName = extName;
            this.owner = owner;
            this.dynamoVM = this.owner.DataContext as DynamoViewModel;
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
            this.UserName = String.Empty;
            this.UserRepositories.Clear();
        }

        #endregion

        #region Events
        private void CurrentWorkspaceChanged(Dynamo.Graph.Workspaces.IWorkspaceModel obj)
        {
            CurrentWorkSpaceName = obj.Name;
        }

        public async void SignIn(object sender, RoutedEventArgs e, string password)
        {
            LogStatus = LogStatus.Initiated;

            ConfigurationManager.AppSettings.Set("User", UserName);
            ConfigurationManager.AppSettings.Set("Password", password.ToSecureString().EncryptString());

            client.Credentials = new Credentials(UserName, password, AuthenticationType.Basic);
            System.Net.HttpStatusCode result = await client.SetCurrentUser(this);
            if (result == System.Net.HttpStatusCode.Unauthorized)
            {
                LogStatus = LogStatus.Failed;
            }
            else
            {
                LogStatus = LogStatus.Completed;
                await client.GetAllRepositories(this);
            }

            await Task.Delay(2000).ContinueWith(t =>
            {
                LoginMessage = String.Empty;
            });

        }

        public void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Dynamo Files (*.dyn)|*.dyn"
            };

            if(dialog.ShowDialog() == DialogResult.OK)
            {
                string file = dialog.FileName;
                //if (dynamoVM.CurrentSpace.HasUnsavedChanges)
                //{
                //    var saveConfirm = System.Windows.MessageBox.Show("Do you want to save changes?", extensionName, MessageBoxButton.YesNo);
                //    if (saveConfirm == MessageBoxResult.Yes)
                //    {
                        
                //    }
                //}
                //dynamoVM.Model.RemoveWorkspace(dynamoVM.CurrentSpace);
                //readyParams.CommandExecutive.ExecuteCommand(new Dynamo.Models.DynamoModel.OpenFileCommand(file), uniqueId, extensionName);
                dynamoVM.Model.OpenFileFromPath(file);
                dynamoVM.ShowStartPage = false;
            }
        }
        #endregion

        public void Dispose()
        {
            readyParams.CurrentWorkspaceChanged -= CurrentWorkspaceChanged;
        }
    }
}
