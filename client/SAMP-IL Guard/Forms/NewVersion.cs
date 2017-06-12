using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace SAMP_IL_Guard.Forms
{
    public partial class NewVersion : Form
    {
        public NewVersion(string newest)
        {
            InitializeComponent();
            label3.Text = Strings.Version;
            label5.Text = newest;
        }
        private void NewVersion_Load(object sender, EventArgs e)
        {
            UpdateMyLanguage();
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel1.Text);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void UpdateMyLanguage()
        {
            RightToLeft = L("RTL") == "1" ? RightToLeft.Yes : RightToLeft.No;
            Text = L("NVHeader");
            label1.Text = L("NVMessage");
            label2.Text = L("NVOwn");
            label4.Text = L("NVNewest");
            label6.Text = L("NVLink");
            label7.Text = L("NVWarning");
            button1.Text = L("NVButtonClose");
        }
        private string L(string n)
        {
            return Variables.language.GetString(n);
        }
    }
}