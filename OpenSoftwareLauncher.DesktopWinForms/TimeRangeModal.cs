using kate.shared.Helpers;
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
    public partial class TimeRangeModal : Form
    {
        public TimeRangeModal()
        {
            InitializeComponent();
        }
        public TimeRangeModal(long min, long max)
        {
            InitializeComponent();

            dateTimePickerMin.Value = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimePickerMin.Value = dateTimePickerMin.Value.AddMilliseconds(min).ToLocalTime();

            dateTimePickerMax.Value = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimePickerMax.Value = dateTimePickerMax.Value.AddMilliseconds(max).ToLocalTime();
        }

        public delegate void RangeDelegate(long min, long max);
        public event RangeDelegate Complete;
        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (Complete != null)
                Complete.Invoke(Minimum, Maximum);
            Close();
        }

        public long Minimum = 0;
        public long Maximum = 1;

        private void UpdateValues()
        {
            if (dateTimePickerMin.Value >= dateTimePickerMax.Value)
            {
                var dt = new DateTime(dateTimePickerMax.Value.Ticks);
                dt.AddMinutes(-1f);
                dateTimePickerMin.Value = dt;
            }
            else if (dateTimePickerMax.Value <= dateTimePickerMin.Value)
            {
                dateTimePickerMax.Value.AddMinutes(1);
            }

            var msMin = ((DateTimeOffset)dateTimePickerMin.Value.ToUniversalTime()).ToUnixTimeMilliseconds();
            var msMax = ((DateTimeOffset)dateTimePickerMax.Value.ToUniversalTime()).ToUnixTimeMilliseconds();

            Minimum = msMin;
            Maximum = msMax;
        }

        private void dateTimePickerMin_ValueChanged(object sender, EventArgs e)
        {
            UpdateValues();
        }

        private void dateTimePickerMax_ValueChanged(object sender, EventArgs e)
        {
            UpdateValues();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            dateTimePickerMin.Value = dateTimePickerMin.MinDate;
            dateTimePickerMax.Value = dateTimePickerMax.MaxDate;
        }
    }
}
