using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
namespace SAMP_IL_Guard.Forms
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }
        private int startY = 0, endY = 0, pos = 0, status = 0;
        private string L(string n)
        {
            return Variables.language.GetString(n);
        }
        private void About_Load(object sender, EventArgs e)
        {
            RightToLeft = L("RTL") == "1" ? RightToLeft.Yes : RightToLeft.No;
            label1.RightToLeft = RightToLeft;
            label1.Text = Variables.language == Languages.Hebrew.ResourceManager ?
@"SA-MP Guard

Guard.SA-MP.co.il



פיתוח בהשראת קהילות SAMP-IL
sa-mp.co.il


מסופק ומאוחסן על ידי קבוצת Oversight
oversight-group.net





פיתוח ותכנות
2012-2013


Amit_B


פיתוח SA-MP Sockets Plugin
2012


BlueG


פיתוח KeepAlive Service
2013


Injection


טסטים


AAvivB

DroweN

ElrosH

FalleN

FireLorD

Fur1Xx

MrBluz

NiceGuy

RaiDeR

ועוד...



תודה לכל מי שעזר בטסטים, דיווח על האקים,

הציע רעיונות או תרם לפיתוח! :)" :
@"SA-MP Guard

Guard.SA-MP.co.il



Development inspired from SAMP-IL communities
sa-mp.co.il


Provided and hosted by Oversight Group
oversight-group.net





Scripting
2012-2013


Amit_B


Development of SA-MP Sockets Plugin
2012


BlueG


KeepAlive Service
2013


Injection


Testing


AAvivB

DroweN

ElrosH

FalleN

FireLorD

Fur1Xx

MrBluz

NiceGuy

RaiDeR

and more...



Thanks to anyone who have helped with the tests, reported hacks,

suggested ideas or contributed to the development! :)";
            label1.Location = new Point(0, startY = this.Size.Height);// + (this.Size.Height / 4)
            endY = 0 - (label1.Text.Split('\n').Length + 1) * 20;
            timer1.Start();
            timer2.Start();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            label1.Location = new Point(0, label1.Location.Y <= endY ? startY : (label1.Location.Y - 1));
        }
        private void About_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            switch (status)
            {
                case 0: label1.ForeColor = Color.FromArgb(255 - pos, 255, 255); break;
                case 1: label1.ForeColor = Color.FromArgb(0, 255 - pos, 255); break;
                case 2: label1.ForeColor = Color.FromArgb(0, 0, 255 - pos); break;
                case 3: this.Close(); break;
            }
            pos++;
            if (pos == 255)
            {
                pos = 0;
                status++;
            }
        }
        private void About_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.None)
                e.Cancel = true;
        }
    }
}