using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security;
using Octokit;
using GitHubForDynamoWPF.ViewModels;

namespace GitHubForDynamoWPF.Helpers
{
    public static class ExtensionMethods
    {
        #region Protect String
        // https://weblogs.asp.net/jongalloway/encrypting-passwords-in-a-net-app-config-file
        private static byte[] entropy = System.Text.Encoding.Unicode.GetBytes("GitHub For Dynamo is Cool");

        public static SecureString ToSecureString(this string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        public static string ToInsecureString(this SecureString input)
        {
            string insecure = String.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                insecure = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }

            return insecure;
        }

        public static string EncryptString(this SecureString input)
        {
            byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                System.Text.Encoding.Unicode.GetBytes(input.ToInsecureString()),
                entropy,
                System.Security.Cryptography.DataProtectionScope.CurrentUser
                );

            return Convert.ToBase64String(encryptedData);
        }

        public static SecureString DecryptString(this string encryptedData)
        {
            try
            {
                byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    entropy,
                    System.Security.Cryptography.DataProtectionScope.CurrentUser
                    );

                return System.Text.Encoding.Unicode.GetString(decryptedData).ToSecureString();
            }
            catch
            {
                return new SecureString();
            }
        } 
        #endregion

        #region GitHub API
        public async static void SetCurrentUser(this GitHubClient client, MainViewModel viewModel)
        {
            try
            {
                viewModel.User = await client.User.Current();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async static void GetAllRepositories(this GitHubClient client, MainViewModel viewModel)
        {
            // Clear collection
            viewModel.UserRepositories.Clear();

            var repos = await client.Repository.GetAllForCurrent();

            foreach(var repo in repos)
            {
                viewModel.UserRepositories.Add(repo);
            }
        }

        public static void SignIn(this GitHubClient client, string user, string password, MainViewModel viewModel)
        {
            ConfigurationManager.AppSettings.Set("User", user);
            ConfigurationManager.AppSettings.Set("Password", password.ToSecureString().EncryptString());

            client.Credentials = new Credentials(user, password, AuthenticationType.Basic);
            client.SetCurrentUser(viewModel);
            client.GetAllRepositories(viewModel);
            
        }

        #endregion
    }
}
