using OSLCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.AdminClient
{
    public partial class PublishedReleaseFileEditorForm : Form
    {
        public PublishedReleaseFileEditorForm()
        {
            InitializeComponent();
        }

        public PublishedRelease Release = new PublishedRelease();
        public PublishedReleaseFile TargetFile = null;
        public void RefreshListView()
        {
            List<string> selectedUids = new List<string>();
            foreach (ListViewItem item in listViewFiles.SelectedItems)
            {
                selectedUids.Add(item.Name);
            }

            listViewFiles.Items.Clear();
            for (int i = 0; i < Release.Files.Length; i++)
            {
                var file = Release.Files[i];
                var item = new ListViewItem(new string[]
                {
                    file.UID,
                    file.Type.ToString(),
                    file.Platform.ToString()
                });
                item.Selected = selectedUids.Contains(file.UID);
                listViewFiles.Items.Add(item);
            }
        }

        private void listViewFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            TargetFile = null;
            foreach (ListViewItem item in listViewFiles.SelectedItems)
            {
                foreach (var file in Release.Files)
                {
                    if (item.Name == file.UID)
                    {
                        TargetFile = file;
                        break;
                    }
                }
            }
            if (TargetFile != null)
            {
            }
        }

        private void buttonFileSave_Click(object sender, EventArgs e)
        {

        }
    }
}
