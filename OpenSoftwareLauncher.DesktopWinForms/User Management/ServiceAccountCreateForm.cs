using kate.shared.Helpers;
using OSLCommon;
using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public partial class ServiceAccountCreateForm : Form
    {
        public ServiceAccountCreateForm()
        {
            InitializeComponent();
        }

        public void Locale()
        {
            Text = LocaleManager.Get(Text);
            labelUsername.Text = LocaleManager.Get(labelUsername.Text);
            labelPermissions.Text = LocaleManager.Get(labelPermissions.Text);
            labelLicenses.Text = LocaleManager.Get(labelLicenses.Text);
            buttonPush.Text = LocaleManager.Get(buttonPush.Text);
        }

        private void ServiceAccountCreateForm_Shown(object sender, EventArgs e)
        {
            Locale();

            var enumList = GeneralHelper.GetEnumList<AccountPermission>();
            foreach (var item in enumList)
            {
                checkedListBoxPermissions.Items.Add(item.ToString());
            }

            var licenseList = Program.LocalContent.GetRemoteLocations();
            foreach (var item in licenseList)
                checkedListBoxLicenses.Items.Add(item);
        }

        private void buttonPush_Click(object sender, EventArgs e)
        {
            Enabled = false;
            var licenseList = new List<string>();
            foreach (var item in checkedListBoxLicenses.CheckedItems)
                licenseList.Add(item.ToString());
            var permissionList = new List<AccountPermission>();
            foreach (var item in checkedListBoxPermissions.CheckedItems)
                permissionList.Add((AccountPermission)Enum.Parse(typeof(AccountPermission), item.ToString()));
            var body = new ServiceAccountCreateRequest()
            {
                Licenses = licenseList.ToArray(),
                Permissions = permissionList.ToArray(),
                Username = textBoxUsername.Text
            };

            var url = Endpoint.ServiceAccountCreate(Program.Client.Token);
            var request = Program.Client.HttpClient.PostAsync(
                url,
                new StringContent(JsonSerializer.Serialize(body, Program.serializerOptions))).Result;

            var stringContent = request.Content.ReadAsStringAsync().Result;

            if ((int)request.StatusCode == 400)
            {
                var exceptionDeser = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                Program.MessageBoxShow(exceptionDeser.Data.Message);
            }
            else if ((int)request.StatusCode == 401)
            {
                var exceptionDeser = JsonSerializer.Deserialize<ObjectResponse<HttpException>>(stringContent, Program.serializerOptions);
                Program.MessageBoxShow(exceptionDeser.Data.Message);
                Close();
            }
            else if ((int)request.StatusCode == 200)
            {
                var deser = JsonSerializer.Deserialize<ObjectResponse<GrantTokenResponse>>(stringContent, Program.serializerOptions);
                var msgResponse = MessageBox.Show("Copy token to clipboard?", LocaleManager.Get("Title"), MessageBoxButtons.OKCancel);
                if (msgResponse == DialogResult.OK)
                {
                    Clipboard.SetText(deser.Data.Token.Token);
                }
                MessageBox.Show($"Username: {textBoxUsername}\nToken: {deser.Data.Token.Token}", "Success");
                Close();
            }
            else
            {
                MessageBox.Show($"URL: {url}\nCode: {(int)request.StatusCode}\n-------- content --------\n{stringContent}", "Invalid Response");
                Close();
            }

            Enabled = true;
        }
    }
}
