#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Dynamo.Wpf.Extensions;
using GitHubForDynamoWPF;
#endregion

namespace GitHubForDynamo
{
    public class GitHubViewExtension : IViewExtension
    {
        private MenuItem gitHubMenuItem;

        public string UniqueId => "{A56E3851-9BFF-4F92-A1E7-0E07FBF56AE3}";

        public string Name => "GitHub for Dynamo";

        public void Dispose()
        {
            
        }
        
        public void Startup(ViewStartupParams p)
        {
         
        }

        public void Loaded(ViewLoadedParams p)
        {
            gitHubMenuItem = new MenuItem
            {
                Header = this.Name
            };

            gitHubMenuItem.Click += (sender, args) =>
            {
                Window gitHubWindow = new GitHubForDynamoWPF.MainWindow
                {
                    Owner = p.DynamoWindow,
                    Title = this.Name
                };

                gitHubWindow.Left = gitHubWindow.Owner.Left + 400;
                gitHubWindow.Top = gitHubWindow.Owner.Top + 200;

                gitHubWindow.Show();
            };

            p.AddMenuItem(MenuBarType.View, gitHubMenuItem);
        }

        public void Shutdown()
        {
           
        }

    }
}
