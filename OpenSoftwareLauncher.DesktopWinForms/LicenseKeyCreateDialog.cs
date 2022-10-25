using OSLCommon.Licensing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            var data = new CreateProductKeyRequest
            {
                Count = int.Parse(numericUpDownLicenseCount.Value.ToString()),
                RemoteLocations = streams,
                ExpiryTimestamp = 0,
                Note = textBoxNote.Text,
                GroupLabel = textBoxName.Text
            };

            var res = Program.Client.CreateLicenseKeys(data);
            Console.WriteLine();
        }

        private void buttonReleaseRemove_Click(object sender, EventArgs e)
        {
            listBoxRelease.Items.Remove(listBoxRelease.SelectedItem);
            ResetComboBox();
        }

        private void LicenseKeyCreateDialog_Shown(object sender, EventArgs e)
        {
            ResetComboBox();
        }

        private void comboBoxReleaseSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEnableState();
        }
    }
}
