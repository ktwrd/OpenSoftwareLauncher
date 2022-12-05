using kate.shared.Helpers;
using OSLCommon.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
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
