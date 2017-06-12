using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace SAMP_IL_Guard.Forms
{
    public partial class Error : Form
    {
        public Error()
        {
            InitializeComponent();
        }
        private void Error_Load(object sender, EventArgs e)
        {
            UpdateMyLanguage();
        }
        private bool aborted = false;
        public void SetError(string errorText, bool abort)
        {
            if (aborted) return;
            label2.Text = errorText;
            if (abort)
            {
                if (!aborted) aborted = true;
                label2.Text += "\n(לא ניתן להמשיך להשתמש בתוכנה)";
            }
            button1.Text = abort ? "יציאה" : "סגור";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (aborted)
                Application.Exit();
            else
                this.Close();
        }
        private void Error_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (aborted)
                Application.Exit();
        }
        private void UpdateMyLanguage()
        {
            RightToLeft = L("RTL") == "1" ? RightToLeft.Yes : RightToLeft.No;
            Text = L("EHeader");
            label1.Text = L("EError");
            button1.Text = L("EButtonClose");
        }
        private string L(string n)
        {
            return Variables.language.GetString(n);
        }
    }
}