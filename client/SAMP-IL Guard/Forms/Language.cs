using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace SAMP_IL_Guard.Forms
{
    public partial class Language : Form
    {
        public Language()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Variables.language = Languages.English.ResourceManager;
            this.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Variables.language = Languages.Hebrew.ResourceManager;
            this.Close();
        }
    }
}