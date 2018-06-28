using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security;
using System.Net;
using Octokit;
using GitHubForDynamoWPF.ViewModels;
using GitHubForDynamoWPF.Attributes;
using MahApps.Metro.Controls;

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

        public static void ToggleFlyout(this Flyout flyout)
        {
            flyout.IsOpen = !flyout.IsOpen;
        }

        /// To use this extantion method, the enum need to have CustomEnumAttribute with CustomEnumAttribute(true)
        public static string TextValue(this Enum myEnum)
        {
            string value = string.Empty;
            /*Check : if the myEnum is a custom enum*/
            var customEnumAttribute = (CustomEnumAttribute)myEnum
                                      .GetType()
                                      .GetCustomAttributes(typeof(CustomEnumAttribute), false)
                                      .FirstOrDefault();

            if (customEnumAttribute == null)
            {
                throw new Exception("The enum don't contain CustomEnumAttribute");
            }
            else if (customEnumAttribute.IsCustomEnum == false)
            {
                throw new Exception("The enum is not a custom enum");
            }

            /*Get the TextValueAttribute*/
            var textValueAttribute = (TextValueAttribute)myEnum
                                         .GetType().GetMember(myEnum.ToString()).Single()
                                         .GetCustomAttributes(typeof(TextValueAttribute), false)
                                         .FirstOrDefault();
            value = (textValueAttribute != null) ? textValueAttribute.Value : string.Empty;
            return value;
        }

        #region GitHub API
        public async static Task<HttpStatusCode> SetCurrentUser(this GitHubClient client, MainViewModel viewModel)
        {
            try
            {
                viewModel.User = await client.User.Current();
                return HttpStatusCode.OK;
            }
            catch (AuthorizationException e)
            {
                return e.HttpResponse.StatusCode;
            }
        }

        public async static Task GetAllRepositories(this GitHubClient client, MainViewModel viewModel)
        {
            // Clear collection
            viewModel.UserRepositories.Clear();

            var repos = await client.Repository.GetAllForCurrent();

            foreach(var repo in repos)
            {
                viewModel.UserRepositories.Add(repo);
            }
        }

        #endregion
    }
}
