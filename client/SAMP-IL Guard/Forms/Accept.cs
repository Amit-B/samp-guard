using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace SAMP_IL_Guard.Forms
{
    public partial class Accept : Form
    {
        public bool allow = false, showagain = false;
        private string ag = string.Empty;
        public Accept(string ag)
        {
            InitializeComponent();
            this.ag = ag;
        }
        private void Accept_Load(object sender, EventArgs e)
        {
            UpdateMyLanguage();
            textBox1.Text = ag;
            try
            {
                if ((Func.GetRegistry("ShowAgain", "", Strings.SelfReg) as string).Length != 0)
                {
                    this.Text += " - עדכון";
                    checkBox2.Checked = Func.GetRegistry("ShowAgain", "", Strings.SelfReg) as string == "1";
                }
            }
            catch
            {
            }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Strings.Site + "index.php?p=view&f=client-agreement" + (Variables.language == Languages.English.ResourceManager ? "-en" : ""));
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            button1.Enabled = checkBox1.Checked;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void Accept_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!allow)
                Application.Exit();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            allow = true;
            showagain = checkBox2.Checked;
            this.Close();
        }
        private void UpdateMyLanguage()
        {
            //RightToLeft = L("RTL") == "1" ? RightToLeft.Yes : RightToLeft.No;
            Text = L("ACHeader");
            label1.Text = L("ACText");
            button1.Text = L("ACContinue");
            button2.Text = L("ACClose");
            linkLabel1.Text = L("ACSource");
            checkBox1.Text = L("ACRead");
            checkBox2.Text = L("ACAgain");
        }
        private string L(string n)
        {
            return Variables.language.GetString(n);
        }
    }
}