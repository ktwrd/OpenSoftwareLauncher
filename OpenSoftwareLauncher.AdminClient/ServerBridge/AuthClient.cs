using OSLCommon;
using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.AdminClient.ServerBridge
{
    public class AuthClient
    {
        public AuthClient(Client client)
        {
            Client = client;
        }
        private Client Client = null;
        private HttpClient HttpClient => Client?.HttpClient;
        private string[] aggExcp(Exception ex, int level = 0)
        {
            var lines = new List<string>();
            lines.Add("-" + ex.Message);
            if (ex.InnerException != null)
            {
                var re = aggExcp(ex.InnerException, level + 1);
                foreach (var i in re)
                    lines.Add("".PadLeft(level * 2, ' ') + i);
            }
            return lines.ToArray();
        }
        /// <summary>
        /// Validate current token or a fresh token.
        /// </summary>
        /// <param name="token">Default <see cref="Program.Config.Auth.Token"/></param>
        /// <param name="endpoint">Target endpoint. Default <see cref="Program.Config.Endpoint"/></param>
        /// <param name="save">Save to config if token success</param>
        /// <param name="showMessageBox">Show message box on error</param>
        /// <param name="throwHttpException">throw <see cref="HttpException"/> if unauthorized</param>
        /// <returns><see cref="null"/> if failure</returns>
        public AccountTokenDetailsResponse ValidateToken(
            string token = null,
            string endpoint = null,
            bool save = true,
            bool showMessageBox = false,
            bool throwHttpException = false)
        {
            if (token == null)
                token = Program.Config.Auth.Token;
            if (endpoint != null)
            {
                Program.Config.Endpoint = endpoint;
                Program.ConfigSave();
                Endpoint.Base = endpoint;
            }

            var url = Endpoint.TokenDetails(token);
            HttpResponseMessage response = null;
            try
            {
                var tsk = HttpClient.GetAsync(url);
                tsk.Wait();
                response = tsk.Result;
            }
            catch (Exception agg)
            {
                new CopyableErrorModal(aggExcp(agg), "Failed to connect").Show();
                return null;
            }
            var stringContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var deserialized = JsonSerializer.Deserialize<ObjectResponse<AccountTokenDetailsResponse>>(stringContent, Program.serializerOptions);
                if (save)
                {
                    Client.TokenData = new AccountToken(deserialized.Data)
                    {
                        Token = token
                    };
                    Client.Token = Client.TokenData.Token;
                    Program.Config.Auth.Token = token;
                    Program.ConfigSave();
                }
                return deserialized.Data;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var exceptionDeserialized = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                if (showMessageBox)
                {
                    var modal = new CopyableErrorModal(new string[]
                    {
                        $"Error {(int)response.StatusCode}",
                        LocaleManager.Get(exceptionDeserialized.Data.Message)
                    });
                    modal.Show();
                    modal.Focus();
                }
                if (throwHttpException)
                    throw exceptionDeserialized.Data;
            }
            else
            {
                Program.MessageBoxShow($"URL: {url}\nCode: {(int)response.StatusCode}\n======== Content ========\n{stringContent}", LocaleManager.Get("ServerResponse_Invalid"));
            }
            return null;
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="endpoint"></param>
        /// <param name="save"></param>
        /// <param name="showMessageBox"></param>
        /// <param name="throwHttpException"></param>
        /// <returns>Can return <see cref="null"/> if unauthorized (401)</returns>
        public GrantTokenResponse ValidateCredentials(
            string username,
            string password,
            string endpoint = null,
            bool save = true,
            bool showMessageBox = true,
            bool throwHttpException = false)
        {
            endpoint = endpoint ?? Program.Config.Endpoint;
            Program.Config.Endpoint = endpoint;
            Program.ConfigSave();
            Endpoint.Base = endpoint;

            var url = Endpoint.TokenGrant(username, password);
            HttpResponseMessage response = null;
            try
            {
                var tsk = HttpClient.GetAsync(url);
                tsk.Wait();
                response = tsk.Result;
            }
            catch (Exception agg)
            {
                new CopyableErrorModal(aggExcp(agg), "Failed to connect").Show();
                return null;
            }
            var stringContent = response.Content.ReadAsStringAsync().Result;

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var deserialized = JsonSerializer.Deserialize<ObjectResponse<GrantTokenResponse>>(stringContent, Program.serializerOptions);

                if (deserialized.Data.Success)
                {
                    Client.TokenData = deserialized.Data.Token;
                    Client.Token = Client.TokenData.Token;
                    if (save)
                    {
                        Program.Config.Auth.Token = Client.TokenData.Token;
                        Program.ConfigSave();
                    }
                    Client.Permissions = deserialized.Data.Permissions;
                }
                else if (showMessageBox && !deserialized.Data.Success)
                {
                    Program.MessageBoxShow(LocaleManager.Get(deserialized.Data.Message, LocaleManager.Get("Client_TokenGrantFailed")));
                }

                return deserialized.Data;
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var exceptionDeserialized = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                if (showMessageBox)
                {
                    var modal = new CopyableErrorModal(new string[]
                    {
                        $"Error {(int)response.StatusCode}",
                        LocaleManager.Get(exceptionDeserialized.Data.Message)
                    });
                    modal.Show();
                    modal.Focus();
                }
                if (throwHttpException)
                    throw exceptionDeserialized.Data;
            }
            else
            {
                var httpException = new HttpException((int)response.StatusCode, stringContent);
                if (showMessageBox)
                {
                    var modal = new HttpExceptionModal(new HttpException((int)response.StatusCode, ""), (int)response.StatusCode, stringContent, url);
                    modal.Show();
                    modal.Focus();
                }
                if (throwHttpException)
                    throw httpException;
            }
            return null;
        }
    }
}
