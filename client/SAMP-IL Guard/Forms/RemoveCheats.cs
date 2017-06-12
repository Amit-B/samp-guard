using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
namespace SAMP_IL_Guard.Forms
{
    public partial class RemoveCheats : Form
    {
        public RemoveCheats()
        {
            InitializeComponent();
        }
        private List<FileInfo> list = null;
        private void RemoveCheats_Load(object sender, EventArgs e)
        {
            UpdateMyLanguage();
            list = Func.GetCheats();
            if (list.Count == 0)
                this.Close();
            else
            {
                for (int i = 0; i < list.Count; i++)
                    listBox1.Items.Add(list[i].Name);
                listBox1.SetSelected(0, true);
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            button2.Enabled = checkBox1.Checked;
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex == -1)
                return;
            textBox1.Text = list[listBox1.SelectedIndex].Name;
            textBox2.Text = list[listBox1.SelectedIndex].DirectoryName;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(textBox2.Text);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < list.Count; i++)
                    File.Delete(list[i].FullName);
                MessageBox.Show("כל הקבצים הקשורים בצ'יטים נמחקו בהצלחה.", "הסרת צ'יטים", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("לא היה ניתן למחוק את כל הקבצים.", "הסרת צ'יטים", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void UpdateMyLanguage()
        {
            RightToLeft = L("RTL") == "1" ? RightToLeft.Yes : RightToLeft.No;
            Text = L("RCHeader");
            label1.Text = L("RCMessage");
            groupBox1.Text = L("RCDetails");
            label2.Text = L("RCFileName");
            label3.Text = L("RCPath");
            checkBox1.Text = L("RCCheckBox");
            button1.Text = L("RCButtonOpenFolder");
            button2.Text = L("RCButtonRemove");
            button3.Text = L("RCButtonCancel");
        }
        private string L(string n)
        {
            return Variables.language.GetString(n);
        }
    }
}