using kate.shared.Helpers;
using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.AdminClient
{
    public static class FormHelper
    {
        public static void LocaleControl(Control control)
        {
            if (control.Controls.Count > 0)
                foreach (Control elem in control.Controls)
                    LocaleControl(elem);

            foreach (Label elem in control.Controls.OfType<Label>())
                if (elem.Text.Length > 0)
                    elem.Text = LocaleManager.Get(elem.Text);
            foreach (Button elem in control.Controls.OfType<Button>())
                if (elem.Text.Length > 0)
                    elem.Text = LocaleManager.Get(elem.Text);
            foreach (GroupBox elem in control.Controls.OfType<GroupBox>())
                if (elem.Text.Length > 0)
                    elem.Text = LocaleManager.Get(elem.Text);
            foreach (ListView elem in control.Controls.OfType<ListView>())
            {
                foreach (ColumnHeader header in elem.Columns)
                    header.Text = LocaleManager.Get(header.Text);
                foreach (ListViewGroup group in elem.Groups)
                    group.Header = LocaleManager.Get(group.Header);
            }
            foreach (ToolStrip strip in control.Controls.OfType<ToolStrip>())
                LocaleControl(strip);
        }
        public static void LocaleControl(IEnumerable<Control> controls)
        {
            if (controls.Count() > 0)
                foreach (Control elem in controls)
                    LocaleControl(elem);

            foreach (Label elem in controls.OfType<Label>())
                if (elem.Text.Length > 0)
                    elem.Text = LocaleManager.Get(elem.Text);
            foreach (Button elem in controls.OfType<Button>())
                if (elem.Text.Length > 0)
                    elem.Text = LocaleManager.Get(elem.Text);
            foreach (GroupBox elem in controls.OfType<GroupBox>())
                if (elem.Text.Length > 0)
                    elem.Text = LocaleManager.Get(elem.Text);
            foreach (ListView elem in controls.OfType<ListView>())
            {
                foreach (ColumnHeader header in elem.Columns)
                    header.Text = LocaleManager.Get(header.Text);
                foreach (ListViewGroup group in elem.Groups)
                    group.Header = LocaleManager.Get(group.Header);
            }
            foreach (ToolStrip strip in controls.OfType<ToolStrip>())
                LocaleControl(strip);
        }
        public static void LocaleControl(Control.ControlCollection collection)
        {
            Control[] controlArray = new Control[collection.Count];
            collection.CopyTo(controlArray, 0);
            LocaleControl(controlArray);
        }
        public static void LocaleControl(ToolStripItemCollection collection)
        {
            foreach (ToolStripItem item in collection)
                item.ToolTipText = LocaleManager.Get(item.ToolTipText);

            foreach (var elem in collection.OfType<ToolStripButton>())
                elem.Text = LocaleManager.Get(elem.Text);
            foreach (var elem in collection.OfType<ToolStripLabel>())
                elem.Text = LocaleManager.Get(elem.Text);
            foreach (var elem in collection.OfType<ToolStripDropDownButton>())
                elem.Text = LocaleManager.Get(elem.Text);
            foreach (var elem in collection.OfType<ToolStripSplitButton>())
                elem.Text = LocaleManager.Get(elem.Text);
            foreach (var elem in collection.OfType<ToolStripDropDownItem>())
                LocaleControl(elem.DropDownItems);
        }
        public static void LocaleControl(ToolStrip strip)
        {
            LocaleControl(strip.Items);
        }
        public static void ReloadGenericCheckedboxList(CheckedListBox cbox, object[] newitems)
        {
            var CheckedItems = new List<object>();
            foreach (var item in cbox.CheckedItems)
                CheckedItems.Add(item);

            cbox.Items.Clear();
            int index = 0;
            foreach (var item in newitems)
            {
                cbox.Items.Add(item);
                if (CheckedItems.Contains(item))
                    cbox.SetItemChecked(index, true);
                index++;
            }
        }
        public static void ReloadGenericListUsernames(CheckedListBox cbox)
        {
            var newitems = Program.LocalContent.AccountDetailList
                .Select(v => v.Username)
                .Concat(cbox.Items.Cast<string>())
                .Distinct()
                .ToArray();
            ReloadGenericCheckedboxList(
                cbox,
                newitems);
        }
        public static void RelaodGenericListLicenses(CheckedListBox cbox)
        {
            var newitems = cbox.Items.Cast<string>()
                .Concat(Program.LocalContent.GetRemoteLocations())
                .Distinct()
                .ToArray();
            ReloadGenericCheckedboxList(cbox, newitems);
        }
        public static void ReloadGenericListPermissions(CheckedListBox cbox)
        {
            var newitems = cbox.Items.Cast<string>()
                .Concat(GeneralHelper.GetEnumList<AccountPermission>().Select(v => v.ToString()))
                .Distinct()
                .ToArray();
            ReloadGenericCheckedboxList(cbox, newitems);
        }
        public static void SelectAll(CheckedListBox cbox)
        {
            for (int i = 0; i < cbox.Items.Count; i++)
            {
                cbox.SetItemChecked(i, true);
            }
        }
        public static void SelectInverse(CheckedListBox cbox)
        {
            for (int i = 0; i < cbox.Items.Count; i++)
            {
                cbox.SetItemChecked(i, !cbox.GetItemChecked(i));
            }
        }
    }
}
