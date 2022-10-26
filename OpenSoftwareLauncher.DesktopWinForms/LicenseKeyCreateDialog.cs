using kate.shared.Helpers;
using OSLCommon.Authorization;
using OSLCommon.Licensing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public partial class LicenseKeyCreateDialog : Form
    {
        public LicenseKeyCreateDialog()
        {
            InitializeComponent();
        }
        public void Locale()
        {
            label5.Text = LocaleManager.Get("Permission_Plural");
            label4.Text = LocaleManager.Get("License_Plural");
        }

        public void UpdateEnableState()
        {
            buttonReleaseAdd.Enabled = comboBoxReleaseSelect.SelectedIndex > 0;
            buttonReleaseRemove.Enabled = listBoxRelease.SelectedIndex >= 0;
        }
        public void ResetComboBox()
        {
            comboBoxReleaseSelect.Items.Clear();
            comboBoxReleaseSelect.Items.Add("");
            comboBoxReleaseSelect.SelectedIndex = 0;
            foreach (var i in Program.LocalContent.GetRemoteLocations())
                if (!listBoxRelease.Items.Contains(i))
                    comboBoxReleaseSelect.Items.Add(i);
            UpdateEnableState();
        }
        private void buttonReleaseAdd_Click(object sender, EventArgs e)
        {
            listBoxRelease.Items.Add(comboBoxReleaseSelect.SelectedItem.ToString());
            ResetComboBox();
            UpdateEnableState();
        }
        private void buttonPush_Click(object sender, EventArgs e)
        {
            string[] streams = new string[listBoxRelease.Items.Count];
            for (int i = 0; i < streams.Length; i++)
                streams[i] = listBoxRelease.Items[i].ToString();
            AccountPermission[] permissions = new AccountPermission[checkedListBoxPermissions.Items.Count];
            for (int i = 0; i < permissions.Length; i++)
                permissions[i] = (AccountPermission)Enum.Parse(typeof(AccountPermission), checkedListBoxPermissions.Items[i].ToString());
            var data = new CreateProductKeyRequest
            {
                Count = int.Parse(numericUpDownLicenseCount.Value.ToString()),
                RemoteLocations = streams,
                ExpiryTimestamp = 0,
                Note = textBoxNote.Text,
                GroupLabel = textBoxName.Text,
                Permissions = permissions
            };

            var res = Program.Client.CreateLicenseKeys(data);
            List<string> fileLines = new List<string>();
            fileLines.Add($"{res.Keys.Length} Keys");
            fileLines.Add($"Created at; {DateTimeOffset.UtcNow}");
            fileLines.Add($"Group Id: {res.GroupId}");
            fileLines.Add("=".PadRight(32, '='));
            foreach (var item in res.Keys)
                fileLines.Add(item.Key);

            var location = ShowFileDialog();
            File.WriteAllLines(location, fileLines);
            Process.Start("explorer", $"/select,\"{location}\"");
        }
        public string ShowFileDialog()
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save License Keys",
                Filter = "Text File|*.txt|Any|*.*",
                FilterIndex = 0
            };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                return dialog.FileName;
            }
            else
            {
                return ShowFileDialog();
            }
        }

        private void buttonReleaseRemove_Click(object sender, EventArgs e)
        {
            listBoxRelease.Items.Remove(listBoxRelease.SelectedItem);
            ResetComboBox();
        }

        private void LicenseKeyCreateDialog_Shown(object sender, EventArgs e)
        {
            var enumArr = GeneralHelper.GetEnumList<AccountPermission>();
            checkedListBoxPermissions.Items.Clear();
            foreach (var en in enumArr)
                checkedListBoxPermissions.Items.Add(en.ToString());
            ResetComboBox();
            Locale();
        }

        private void comboBoxReleaseSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEnableState();
        }

        private void listBoxRelease_MouseUp(object sender, MouseEventArgs e)
        {
            UpdateEnableState();
        }
    }
}
