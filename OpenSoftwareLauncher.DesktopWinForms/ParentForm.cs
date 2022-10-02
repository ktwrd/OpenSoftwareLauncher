﻿using System;
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
    public partial class ParentForm : Form
    {
        public UserDetailsForm userDetailsForm;
        public ParentForm()
        {
            InitializeComponent();
        }

        private void ParentForm_Shown(object sender, EventArgs e)
        {
            userDetailsForm = new UserDetailsForm();
            userDetailsForm.MdiParent = this;
            userDetailsForm.Show();
        }
    }
}