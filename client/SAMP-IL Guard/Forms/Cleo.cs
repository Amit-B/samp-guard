using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace SAMP_IL_Guard.Forms
{
    public partial class Cleo : Form
    {
        public Cleo()
        {
            InitializeComponent();
        }
        private void Cleo_Load(object sender, EventArgs e)
        {
            UpdateMyLanguage();
        }
        public int ret = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                ret = 1;
            else if (radioButton2.Checked)
                ret = 2;
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void UpdateMyLanguage()
        {
            RightToLeft = L("RTL") == "1" ? RightToLeft.Yes : RightToLeft.No;
            Text = L("CLHeader");
            label1.Text = L("CLText");
            radioButton1.Text = L("CLActive");
            radioButton2.Text = L("CLDeactive");
            button1.Text = L("CLButtonDo");
            button2.Text = L("CLButtonCancel");
        }
        private string L(string n)
        {
            return Variables.language.GetString(n);
        }
    }
}