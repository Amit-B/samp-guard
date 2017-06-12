using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace SAMP_IL_Guard.Forms
{
    public partial class ChangeName : Form
    {
        public ChangeName()
        {
            InitializeComponent();
        }
        public string name = string.Empty;
        private bool close = false;
        private void ChangeName_Load(object sender, EventArgs e)
        {
            UpdateMyLanguage();
            textBox1.Text = name = Func.GetRegistry("PlayerName", "Unknown", Strings.RegistryKey).ToString();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            bool abort = false;
            for (int i = 0; i < textBox1.Text.Length && !abort; i++)
                if (!(textBox1.Text[i] >= '0' && textBox1.Text[i] <= '9')
                    && !(textBox1.Text[i] >= 'A' && textBox1.Text[i] <= 'Z')
                    && !(textBox1.Text[i] >= 'a' && textBox1.Text[i] <= 'z')
                    && textBox1.Text[i] != '_' && textBox1.Text[i] != '[' && textBox1.Text[i] != ']'
                    && textBox1.Text[i] != '.' && textBox1.Text[i] != '(' && textBox1.Text[i] != ')'
                    && textBox1.Text[i] != '@' && textBox1.Text[i] != '$')
                    abort = true;
            if (abort)
                textBox1.Text = name;
            else
                name = textBox1.Text;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            name = textBox1.Text;
            close = true;
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            name = string.Empty;
            this.Close();
        }
        private void ChangeName_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!close)
                name = string.Empty;
        }
        private void UpdateMyLanguage()
        {
            RightToLeft = L("RTL") == "1" ? RightToLeft.Yes : RightToLeft.No;
            Text = L("CNHeader");
            label1.Text = L("CNText");
            button1.Text = L("CNButtonChange");
            button2.Text = L("CNButtonCancel");
        }
        private string L(string n)
        {
            return Variables.language.GetString(n);
        }
    }
}