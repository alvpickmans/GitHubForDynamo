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
using System.Diagnostics;

namespace GitHubForDynamoWPF.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : MetroWindow
    {
        private MainViewModel viewModel;

        public MainView(MainViewModel viewModel)
        {
            this.viewModel = viewModel;
            InitializeComponent();

            this.dg_Repositories.ItemsSource = this.viewModel.UserRepositories;

            this.btn_SignInPanel.Click += (sender, args) =>
            {
                this.flyout_SignInPanel.IsOpen = true;
            };

            this.btn_SignIn.Click += (sender, args) =>
            {
                viewModel.Client.SignIn(this.txtBox_UserName.Text, this.pwBox_Password.Password, viewModel);
                this.flyout_SignInPanel.IsOpen = false;
            };

        }

        private void OnHyperlinkCellClick(object sender, RoutedEventArgs e)
        {
            //e.Handled = true;
            var destination = ((Hyperlink)e.OriginalSource).NavigateUri;
            Process.Start(destination.ToString());
        }
    }
}
