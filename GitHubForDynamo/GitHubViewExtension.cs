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
using MahApps.Metro.Controls;
#endregion

namespace GitHubForDynamo
{
    public class GitHubViewExtension : IViewExtension
    {
        private MenuItem gitHubMenuItem;
        private MetroWindow gitHubWindow;
        private bool DisposeWindow;

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
            DisposeWindow = false;
            var mainViewModel = new GitHubForDynamoWPF.ViewModels.MainViewModel(p);
            gitHubWindow = new GitHubForDynamoWPF.Views.MainView(mainViewModel)
            {
                Title = this.Name,
                Owner = p.DynamoWindow,
                DataContext = mainViewModel
            };

            // When closing, actually hide the window so it is kept on the background
            gitHubWindow.Closing += (sender, args) =>
            {
                if (!DisposeWindow)
                {
                    args.Cancel = true;
                    (sender as Window).Hide();
                }
            };

            gitHubMenuItem = new MenuItem
            {
                Header = this.Name
            };

            gitHubMenuItem.Click += (sender, args) =>
            {
                gitHubWindow.Show();
            };

            p.AddSeparator(MenuBarType.Packages, new Separator());
            p.AddMenuItem(MenuBarType.Packages, gitHubMenuItem);
        }

        public void Shutdown()
        {
            DisposeWindow = true;
            gitHubWindow.Close();
        }

    }
}
