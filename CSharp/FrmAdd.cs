using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RealJabber
{
    public partial class FrmAdd : Form
    {
        private string jid = "";

        public string AddJID
        {
            get { return jid; }
        }

        public FrmAdd()
        {
            InitializeComponent();
        }

        /// <summary>Add button</summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tbAdd.Text.Contains(" "))
            {
                MessageBox.Show("Spaces are not allowed.\nTry adding their full chat email address.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }
            jid = tbAdd.Text;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>Cancel button</summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
