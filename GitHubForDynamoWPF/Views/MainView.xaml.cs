using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using GitHubForDynamoWPF.ViewModels;
using GitHubForDynamoWPF.Helpers;
using GitHubForDynamoWPF.Interfaces;
using System.Diagnostics;

namespace GitHubForDynamoWPF.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : MetroWindow, IPassword
    {
        private MainViewModel viewModel;

        public MainView(MainViewModel viewModel)
        {
            this.viewModel = viewModel;
            InitializeComponent();

            this.dg_Repositories.ItemsSource = this.viewModel.UserRepositories;

            this.btn_SignInPanel.Click += (sender, args) =>
            {
                this.flyout_SignInPanel.ToggleFlyout();
            };

            this.btn_SignIn.Click += (sender, args) =>
            {
                viewModel.SignIn(sender, args, GetPassword());
            };

            this.btn_OpenFile.Click += viewModel.OpenFile;

        }

        public string GetPassword()
        {
            return this.pwBox_Password.Password;
        }

        private void OnHyperlinkCellClick(object sender, RoutedEventArgs e)
        {
            var destination = ((Hyperlink)e.OriginalSource).NavigateUri;
            Process.Start(destination.ToString());
        }
    }
}
