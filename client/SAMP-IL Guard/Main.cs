//#define _DEBUG_
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Management;
using System.Resources;
using Microsoft.Win32;
using System.Threading;
using System.Security.Principal;
// 543, 419
namespace SAMP_IL_Guard
{
    public partial class Main : Form
    {
        private bool hasArgs = false;
        public Main()
        {
            InitializeComponent();
            ResourceManager newlang = null;
            if (System.AppDomain.CurrentDomain.FriendlyName.Contains(" he") || System.AppDomain.CurrentDomain.FriendlyName.Contains(" HE"))
                newlang = Languages.Hebrew.ResourceManager;
            else if (System.AppDomain.CurrentDomain.FriendlyName.Contains(" en") || System.AppDomain.CurrentDomain.FriendlyName.Contains(" EN"))
                newlang = Languages.English.ResourceManager;
            if(newlang != null)
            {
                Variables.language = newlang;
                hasArgs = true;
                UpdateMyLanguage();
            }
        }
        private Socket s = null;
        private List<string> serverlist = new List<string>(), dlls = new List<string>();
        private string url = Strings.Site, crashfile = string.Empty, agreement = string.Empty, lastsent = string.Empty, failmsg = string.Empty, sampcfgpath = string.Empty, hwid = string.Empty;
        private int clientID = -1, playerID = -1, uptime = 0, connecting = 0, moonsize = 0, collcd = 0, serverID = -1;
        private double collmax = 0.0;
        private bool connected = false, exitAfterDisconnect = false, ws = true, heb = true, lockmoon = false;
        private WebClient wc = new WebClient();
        private StreamWriter crashdetect = null;
        private ManagementEventWatcher[] prcWatchers = { null, null };
        const int Timestamp = 0, DisableHM = 1, FPSLimit = 2, PageSize = 3, MultiCore = 4, AudioProxyOff = 5, AudioMsgOff = 6, MAX_SAMP_CFG = 7;
        private void Form1_Load(object sender, EventArgs e)
        {
            /*if (!((new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                .IsInRole(WindowsBuiltInRole.Administrator)))
            {
                MessageBox.Show("נא להפעיל את התוכנה כמנהל.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }*/
            long ticks = 0;
            ticks = DateTime.Now.Ticks / 10000;
            long first = ticks;
            string copy = string.Empty;
            copy += "\n1 - " + ((DateTime.Now.Ticks / 10000) - ticks);
            Variables.mainForm = this;
            try
            {
                Process[] pr = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
                if (pr.Length > 1)
                    for (int i = 0; i < pr.Length; i++)
                    {
                        if (pr[i] == Process.GetCurrentProcess())
                            continue;
                        pr[i].Kill();
                    }
            }
            catch
            {
                Func.Error(L("ErrorPrc"), true);
                return;
            }
            try
            {
                crashdetect = new StreamWriter(crashfile = Environment.CurrentDirectory + "/sguard-crash-detect.txt", true, Encoding.UTF8);
                crashdetect.AutoFlush = true;
            }
            catch
            {
                Func.Error(L("ErrorFiles"), true);
                ReleaseCrashDetect();
                return;
            }
            copy += "\n2 - " + ((DateTime.Now.Ticks / 10000) - ticks);
            ticks = DateTime.Now.Ticks / 10000;
            try
            {
                crashdetect.WriteLine(L("CDLoading"));
                agreement = Func.ReadFromWeb(Strings.Site + "doc/client-agreement" + (CurrentLanguageID() == 1 ? "-en" : "") + ".txt").Replace("\n", "\r\n");
                crashdetect.WriteLine(L("CDInternet"));
                string reg = Func.GetRegistry("Guard", "", Strings.SelfReg) as string;
                if (reg == null || reg.ToString().Length == 0 ||
                    (Convert.ToInt32(reg) != agreement.Length && Func.GetRegistry("ShowAgain", "", Strings.SelfReg).ToString() == "1") ||
                    (Func.GetRegistry("ShowAgain", "", Strings.SelfReg) as string).Length == 0)
                {
                    Forms.Accept a = new Forms.Accept(agreement);
                    a.ShowDialog();
                    if (a.allow)
                    {
                        Func.SetRegistry("Guard", agreement.Length.ToString(), Strings.SelfReg, true);
                        Func.SetRegistry("ShowAgain", a.showagain ? "1" : "0", Strings.SelfReg, true);
                        Func.SetRegistry("Language", CurrentLanguageID().ToString(), Strings.SelfReg, true);
                        reg = "X";
                    }
                    else
                    {
                        ReleaseCrashDetect();
                        return;
                    }
                }
                if (reg != "X" && !hasArgs)
                {
                    string lang = Func.GetRegistry("Language", CurrentLanguageID().ToString(), Strings.SelfReg) as string;
                    if (reg == null || reg.ToString().Length == 0)
                        Func.SetRegistry("Language", lang = CurrentLanguageID().ToString(), Strings.SelfReg, true);
                    Variables.language = lang == "0" ? Languages.Hebrew.ResourceManager : Languages.English.ResourceManager;
                    heb = Variables.language == Languages.Hebrew.ResourceManager;
                    UpdateMyLanguage();
                }
                label3.Text = L("MsgOffline");
                crashdetect.WriteLine(L("CDRegistry"));
            }
            catch
            {
                ReleaseCrashDetect();
                Func.Error(L("ErrorNet"), true);
                return;
            }
            copy += "\n3 - " + ((DateTime.Now.Ticks / 10000) - ticks);
            ticks = DateTime.Now.Ticks / 10000;
            int errcode = 0;
            try
            {
                crashdetect.WriteLine(L("CDPath"));
                if (!Directory.Exists(Func.GetGTASAPath()))
                {
                    Func.Error(L("ErrorGTA"), true);
                    return;
                }
                errcode++; // 1
                if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
                {
                    Func.Error(L("ErrorAlreadyActive"), true);
                    return;
                }
                errcode++; // 2
                string lastVersion = Func.ReadFromWeb(url + "version.txt");
                //Clipboard.SetText(lastVersion);
                crashdetect.WriteLine(L("CDVersion") + lastVersion);
                copy += "\n3 A - " + ((DateTime.Now.Ticks / 10000) - ticks);
                ticks = DateTime.Now.Ticks / 10000;
                errcode++; // 3
                if (lastVersion == Strings.Version)
                {
                    label2.Text = label2.Text.Replace("X", Strings.Version);
                    string tmp = string.Empty,
                        fname = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/sguard-servers.txt";
                    errcode++; // 4
                    crashdetect.WriteLine(L("CDDocuments"));
                    File.WriteAllText(fname, Func.ReadFromWeb(url + "servers.txt"));
                    string[] lines = File.ReadAllLines(fname);
                    File.Delete(fname);
                    errcode++; // 5
                    crashdetect.WriteLine(L("CDServers"));
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (serverlist.Contains(lines[i]))
                            continue;
                        serverlist.Add(lines[i]);
                        dataGridView1.Rows.Add(new object[]
                        {
                            serverlist.Count.ToString(),
                            lines[i].Split(':')[2],
                            lines[i].Split(':')[3]
                        });
                    }
                    copy += "\n3 B - " + ((DateTime.Now.Ticks / 10000) - ticks);
                    ticks = DateTime.Now.Ticks / 10000;
                    errcode++; // 6
                    label4.Text = "IP: " + serverlist[dataGridView1.SelectedRows[0].Index].Split(':')[0] + ":" + serverlist[dataGridView1.SelectedRows[0].Index].Split(':')[1];
                    new Thread(() =>
                        {
                            for (int i = 0, m = Variables.files.GetLength(0); i < m; i++)
                                Variables.lists.Add(Func.ReadFromWeb(url + Variables.files[i]).Split('\n'));
                            for (int i = 0; i < Variables.lists.Count; i++)
                                for (int j = 0; j < Variables.lists[i].Length; j++)
                                    Variables.lists[i][j] = Variables.lists[i][j].Replace("\r", "");
                        }).Start();
                    copy += "\n3 C - " + ((DateTime.Now.Ticks / 10000) - ticks);
                    ticks = DateTime.Now.Ticks / 10000;
                    errcode++; // 7
                    crashdetect.WriteLine(L("CDLists"));
                    UpdateNickname();
                    errcode++; // 8
                    crashdetect.WriteLine(L("CDNickname"));
                    //if (Func.HasCheats() >= 2)
                    //    toolTip1.Show(warn, this, 0, 0, 4000);
                    fileSystemWatcher1.Path = Func.GetGTASAPath();
                    try
                    {
                        prcWatchers[0] = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
                        prcWatchers[0].EventArrived += new EventArrivedEventHandler(prcStarted_EventArrived);
                        prcWatchers[0].Start();
                        prcWatchers[1] = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
                        prcWatchers[1].EventArrived += new EventArrivedEventHandler(prcStopped_EventArrived);
                        prcWatchers[1].Start();
                    }
                    catch
                    {
                    }
                    errcode++; // 9
                    sampcfgpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/GTA San Andreas User Files/SAMP/sa-mp.cfg";
                    if (!File.Exists(sampcfgpath))
                    {
                        ReleaseCrashDetect();
                        Func.Error(L("ErrorSAMPCFG"), true);
                        return;
                    }
                    copy += "\n3 D - " + ((DateTime.Now.Ticks / 10000) - ticks);
                    ticks = DateTime.Now.Ticks / 10000;
                    crashdetect.WriteLine(L("CDCFG"));
                    errcode++; // 10
                    new Thread(() =>
                        {
                            ManagementObjectCollection mbsList = null;
                            ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_processor");
                            mbsList = mbs.Get();
                            foreach (ManagementObject mo in mbsList)
                                hwid = mo["ProcessorID"].ToString();
                        }).Start();
                    Func.FixSecure();
                    errcode++; // 11
                    crashdetect.WriteLine(L("CDCheat"));
                    ReleaseCrashDetect();
                    copy += "\n3 E - " + ((DateTime.Now.Ticks / 10000) - ticks);
                    ticks = DateTime.Now.Ticks / 10000;
                }
                else
                {
                    this.Visible = false;
                    crashdetect.WriteLine(L("CDIncorrectVersion"));
                    Forms.NewVersion nv = new Forms.NewVersion(lastVersion);
                    nv.ShowDialog();
                    ReleaseCrashDetect();
                    this.Close();
                }
            }
            catch
            {
                MessageBox.Show(L("ErrorCode") + errcode, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            ticks = DateTime.Now.Ticks / 10000;
            copy += "\n4 - " + ((DateTime.Now.Ticks / 10000) - ticks);
            copy += "\nFinal - " + (ticks - first);
            //Clipboard.SetText(copy);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (!button2.Enabled)
                return;
            if (dataGridView1.SelectedRows[0].Index == -1)
                Func.Error(L("ErrorServerSelect"), false);
            else if (Variables.proc != null)
                Func.Error(L("ErrorGameActive"), false);
            else if (Func.GetRegistry("PlayerName", "Unknown", Strings.RegistryKey).ToString() != toolStripStatusLabel4.Text)
                Func.Error(L("ErrorName"), false);
            else
            {
                serverID = dataGridView1.SelectedRows[0].Index;
                string[] srv = serverlist[dataGridView1.SelectedRows[0].Index].Split(':');
                const int IP = 0, PORT = 1;
                label3.Text = L("MsgConnecting") + srv[IP] + ":" + srv[PORT] + "...";
                button2.Enabled = false;
                string port = srv[IP] == "127.0.0.1" ? (srv[PORT].Remove(3, 1) + "8") : Func.ReadFromWeb("http://guard.sa-mp.co.il/assign/" + srv[IP] + "_" + srv[PORT]);
                failmsg = L("MsgConnectionFailed").Replace("X", serverlist[dataGridView1.SelectedRows[0].Index].Split(':')[0]).Replace("Y", serverlist[dataGridView1.SelectedRows[0].Index].Split(':')[1]);
                if (port == null || !Connect(srv[IP], Convert.ToInt32(port)))
                {
                    label3.Text = failmsg;
                    button2.Enabled = true;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            label3.Text = L("MsgDisconnecting");
            Disconnect("Disconnected from server using the program");
            label3.Text = L("MsgOffline");
            button2.Enabled = true;
            button1.Enabled = false;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName(Strings.SAMPProcess).Length > 0)
                Func.Error(L("ErrorAlreadySamp"), false);
            else
                Process.Start(Func.GetGTASAPath() + "/" + Strings.SAMPProcess + ".exe");
            /*Memory.OpenGTAProcess();
            MessageBox.Show(Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.Widescreen)).ToString());
            Memory.WriteMemory(Memory.GTAMemoryAddresses.Widescreen, 1);
            MessageBox.Show(Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.Widescreen)).ToString());
            Memory.CloseGTAProcess();*/
            //timer2.Start();
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            //Memory.OpenGTAProcess();
            //label1.Text = Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.TEST)).ToString();
            //Memory.CloseGTAProcess();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            int tatata = 0;
            uptime++;
            if (uptime % 20 == 0)
                UpdateNickname();
            try
            {
                if (s == null) return;
                byte[] buffer = new byte[1024];
                int iRx = s.Receive(buffer);
                char[] chars = new char[iRx];
                Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
                string tmp = new string(chars);
                if (tmp != lastsent)
                {
#if _DEBUG_
                    MessageBox.Show((lastsent = tmp).Split('/')[2]);
#endif
                    //try
                    //{
                        string[] splited = (tmp[tmp.Length - 1] == ';' ? (tmp = tmp.Remove(tmp.Length - 1)) : tmp).Split(';');
                        string prefix = DateTime.Now.ToLongDateString() + "|" + DateTime.Now.ToLongTimeString() + "|";
                        if (splited.Length > 1)
                        {
                            for (int i = 0, m = splited.GetLength(0); i < m; i++)
                                if (splited[i].Length > 1)
                                {
                                    Recieve((lastsent = (prefix + splited[i])).Split('|')[2]);
                                    textBox1.Text = textBox1.Text + "Recieved: " + lastsent.Split('|')[2] + "\n";
                                }
                        }
                        else
                        {
                            Recieve((lastsent = (prefix + tmp)).Split('|')[2]);
                            textBox1.Text = textBox1.Text + "Recieved: " + lastsent.Split('|')[2] + "\n";
                        }
                        /*
                    }
                    catch (Exception ex)
                    {
                        Func.Error(L("ErrorRecieve"), false);
                        Disconnect("", false);
                        label3.Text = L("MsgOffline");
                        button2.Enabled = true;
                        button1.Enabled = false;
                        MessageBox.Show(ex.Message);
                    }*/
                }
            }
            catch
#if _DEBUG_
                (SocketException se)
#endif
            {
#if _DEBUG_
                Error(se.Message);
#endif
            }
            if (connecting > 0 && !connected)
            {
                connecting--;
                if (connecting == 0)
                {
                    label3.Text = failmsg;
                    button2.Enabled = true;
                    ReleaseSocket();
                }
            }
            try
            {
                if (IsConnected() && Variables.proc != null)
                {
                    tatata = 1;
                    Memory.OpenGTAProcess();
                    if (lockmoon)
                        Memory.WriteMemory(Memory.GTAMemoryAddresses.MoonSize, moonsize);
                    tatata = 2;
                    /*if (dlls.Count > 0)
                    {
                        bool dllinj = false;
                        if (Variables.proc.Modules.Count != dlls.Count)
                            dllinj = true;
                        tatata = 3;
                        if (dllinj)
                            for (int i = 0; i < Variables.proc.Modules.Count && !dllinj; i++)
                                if (Variables.proc.Modules[i].FileName != dlls[i])
                                    dllinj = true;
                        tatata = 4;
                        if (dllinj)
                        {
                            tatata = 5;
                            string s = string.Empty;
                            try
                            {
                                for (int i = 0; i < Variables.proc.Modules.Count; i++)
                                    s += Variables.proc.Modules[i].FileName + "\n";
                            }
                            catch
                            {
                            }
                            tatata = 6;
                            s += "\n\n----------------\n\n";
                            for (int i = 0; i < dlls.Count; i++)
                                s += dlls[i] + "\n";
                            tatata = 7;
                            //Clipboard.SetText(s);
                            //tatata = 555;
                            Func.Error("זוהתה הזרקת קובץ למשחק.", true);
                            return;
                        }
                    }*/
                    tatata = 3;
                    string toSend = string.Empty;
                    if (uptime % 3 == 0)
                    {
                        tatata = 100;
                        toSend = "infoint";
                        string[] info = new string[]
                            {
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.RadioStation)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.Fireproof)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.Cheat_InvisibleCars)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.Cheat_DriveOnWater)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.Cheat_NoReload)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.Joypad)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.PlayerState)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.JumpState)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.RunningState)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.Lock)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.Menu)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.Town)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.InfiniteRun)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.RadioVolume)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.SfxVolume)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.AFK)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.VehicleSirens)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.VehicleHorn)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.MoonSize)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.Cheat_StopGameClock)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.NightVision)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.ThermalVision)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.HUD)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.Blur)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.Stamina)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.PistolClip)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.ShotgunClip)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.UziClip)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.AssaultClip)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.Widescreen)).ToString()
                            };
                        tatata = 101;
                        for (int i = 0; i < info.Length; i++)
                            toSend += " " + info[i];
                        tatata = 102;
                    }
                    else if (uptime % 3 == 1)
                    {
                        tatata = 200;
                        toSend = "infofloat";
                        string[] info = new string[]
                            {
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.Gravity)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.MaxHealth)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.WaveLevel)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.VehicleMass)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.VehicleNitro)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.VehicleExplodeTimer)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.TrainSpeed)).ToString(),
                                Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.RotationSpeed)).ToString()
                            };
                        tatata = 201;
                        for (int i = 0; i < info.Length; i++)
                            toSend += " " + info[i];
                        tatata = 202;
                    }
                    else if (uptime % 3 == 2)
                    {
                        tatata = 300;
                        toSend = "infosamp";
                        int[] array = new int[MAX_SAMP_CFG];
                        string[] lines = File.ReadAllLines(sampcfgpath);
                        tatata = 301;
                        for (int i = 0, j = 0; i < lines.Length; i++)
                        {
                            switch (lines[i].Split('=')[0])
                            {
                                case "timestamp": j = Timestamp; break;
                                case "disableheadmove": j = DisableHM; break;
                                case "fpslimit": j = FPSLimit; break;
                                case "pagesize": j = PageSize; break;
                                case "multicore": j = MultiCore; break;
                                case "audioproxyoff": j = AudioProxyOff; break;
                                case "audiomsgoff": j = AudioMsgOff; break;
                            }
                            array[j] = Convert.ToInt16(lines[i].Split('=')[1]);
                        }
                        tatata = 302;
                        for (int i = 0; i < array.Length; i++)
                            toSend += " " + array[i];
                        tatata = 303;
                    }
                    Send(toSend, true);
                    Memory.CloseGTAProcess();
                }
                tatata = 5;
            }
            catch (Exception ex)
            {
                Func.Error(L("ErrorSent") + "\n" + ex.Message + "\n" + tatata, true);
            }
        }
        public bool Connect(string ip, int port)
        {
            try
            {
                s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.ReceiveTimeout = 500;
                s.SendTimeout = 500;
                IPAddress remoteIPAddress = IPAddress.Parse(ip);
                IPEndPoint remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, port);
                //s.Connect(remoteEndPoint);
                IAsyncResult result = s.BeginConnect(remoteEndPoint, null, null);
                bool success = result.AsyncWaitHandle.WaitOne(5000, true);
                if (!success)
                {
                    s.Close();
                    return false;
                }
                if (s.Connected)
                {
                    connecting = 3;
                    timer1.Start();
                    Send("connect " + Func.UniqueID(ip == "127.0.0.1") + " " + Strings.Version);
                }
                else
                    return false;
            }
            catch// (SocketException se)
            {
                //Func.Error("ההתחברות נכשלה. מידע נוסף:\n" + se.Message + "\n" + se.ErrorCode, false);
                ReleaseSocket();
                return false;
            }
            return true;
        }
        public void Disconnect(string reason, bool serverAlive = true)
        {
            try
            {
                if (serverAlive)
                {
                    Send("disconnect " + reason);
                    s.Disconnect(false);
                }
                ReleaseSocket();
                timer1.Stop();
                timer3.Stop();
                UpdateClientID(-1);
                dlls.Clear();
                if (Func.IsSecured())
                    Func.SecureGTASA(false);
                linkLabel2.Visible = false;
                connected = false;
            }
            catch (SocketException se)
            {
                Func.Error(L("ErrorDisconnectFailed") + "\n" + se.Message, false);
            }
        }
        private void Send(string data, bool ignoredebug = false)
        {
            try
            {
                s.Send(System.Text.Encoding.ASCII.GetBytes(data + ";"));
                if (!ignoredebug)
                    textBox1.Text += "Sent: " + data + "\n";
            }
            catch (SocketException se)
            {
                Func.Error(L("ErrorSend") + "\n" + se.Message, false);
            }
        }
        private void SendResponse(Variables.Requests request, string response)
        {
            Send("response " + Convert.ToInt32(request) + " " + response);
        }
        private void Recieve(string data)
        {
            string cmd = data.Contains(":") ? data.Split(':')[0] : data;
            if (cmd == "connected")
            {
                UpdateClientID(Convert.ToInt32(data.Split(':')[1]));
                UpdatePlayerID(Convert.ToInt32(data.Split(':')[2]));
                Send("hwid " + hwid);
                if (!Func.IsSecured())
                    Func.SecureGTASA(true);
                connecting = 0;
                connected = true;
                timer1.Interval = 500;
                timer3.Start();
                button2.Enabled = false;
                button1.Enabled = true;
                linkLabel2.Visible = true;
                Process[] samp;
                if ((samp = Process.GetProcessesByName(Strings.SAMPProcess)).Length > 0)
                    samp[0].Kill();
                label3.Text = L("MsgConnected")
                    .Replace("X", serverlist[dataGridView1.SelectedRows[0].Index].Split(':')[0])
                    .Replace("Y", serverlist[dataGridView1.SelectedRows[0].Index].Split(':')[1])
                    .Replace("Z", clientID.ToString());
            }
            else if (cmd == "badversion")
            {
                button1_Click(null, null);
                Func.Error(L("ErrorVersion")
                    .Replace("\\n", "\n")
                    .Replace("{OWN}", Strings.Version.Trim())
                    .Replace("{NEW}", data.Split(':')[1].Trim()), false);
            }
            else if (cmd == "id")
            {
                int id = Convert.ToInt32(data.Split(':')[1]);
                UpdatePlayerID(id);
                if (id != -1)
                {
                    dlls.Clear();
                    for (int i = 0; i < Variables.proc.Modules.Count; i++)
                        dlls.Add(Variables.proc.Modules[i].FileName);
                }
            }
            else if (cmd == "proc")
            {
                Process[] plist = Process.GetProcessesByName(data.Split(':')[1]);
                SendResponse(Variables.Requests.REQUEST_PROC, (plist.Length > 0).ToString());
            }
            else if (cmd == "proclist")
            {
                Process[] plist = Process.GetProcesses();
                string ret = string.Empty, seperator = GetString(data);
                for (int i = 0; i < plist.Length; i++)
                    ret += plist[i].ProcessName + (i == plist.Length - 1 ? "" : seperator);
                SendResponse(Variables.Requests.REQUEST_PROCLIST, ret);
            }
            else if (cmd == "webpage")
                Process.Start("http://" + GetString(data));
            else if (cmd == "cheats")
                SendResponse(Variables.Requests.REQUEST_CHEATS, (Func.HasCheats() > 0).ToString());
            else if (cmd == "cleo")
                SendResponse(Variables.Requests.REQUEST_CLEO, Directory.Exists(Func.GetGTASAPath() + "/CLEO").ToString());
            else if (cmd == "rename")
                Func.SetRegistry("PlayerName", data.Split(':')[1], Strings.RegistryKey);
            else if (cmd == "dltxd")
            {
                string url = GetString(data), target = Func.GetGTASAPath() + "/models/txd/" + url.Remove(0, url.LastIndexOf('/') + 1);
                Func.Backup(target);
                wc.DownloadFile("http://" + url, target);
            }
            else if (cmd == "txdexist")
                SendResponse(Variables.Requests.REQUEST_TXD, File.Exists(Func.GetGTASAPath() + "/models/txd/" + GetString(data) + ".txd").ToString());
            else if (cmd == "quit")
                Variables.proc.Kill();
            else if (cmd == "removejoypad")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.Joypad, 1);
                Memory.WriteMemory(Memory.GTAMemoryAddresses.MenuJoypad, 0);
                Memory.CloseGTAProcess();
            }
            else if (cmd == "gravity")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.Gravity, Convert.ToSingle(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "fireproof")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.Fireproof, Convert.ToUInt16(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "lock")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.Lock, Convert.ToUInt16(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "menu")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.Menu, Convert.ToUInt16(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "cheat")
            {
                Memory.OpenGTAProcess();
                Memory.GTAMemoryAddresses ad = Memory.GTAMemoryAddresses.Cheat_InvisibleCars;
                switch (Convert.ToInt16(data.Split(':')[1]))
                {
                    case 1: ad = Memory.GTAMemoryAddresses.Cheat_InvisibleCars; break;
                    case 2: ad = Memory.GTAMemoryAddresses.Cheat_DriveOnWater; break;
                    case 3: ad = Memory.GTAMemoryAddresses.Cheat_NoReload; break;
                    case 4: ad = Memory.GTAMemoryAddresses.Cheat_StopGameClock; break;
                }
                Memory.WriteMemory(ad, Convert.ToUInt16(data.Split(':')[2]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "maxhealth")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.MaxHealth, Convert.ToSingle(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "rain")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.Rain, Convert.ToSingle(0.9));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "waves")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.WaveLevel, Convert.ToSingle(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "camera")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.Camera1, Convert.ToSingle(data.Split(':')[1]));
                Memory.WriteMemory(Memory.GTAMemoryAddresses.Camera2, Convert.ToSingle(data.Split(':')[1]));
                Memory.WriteMemory(Memory.GTAMemoryAddresses.Camera3, Convert.ToSingle(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "camerareset")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.Camera1, Convert.ToSingle(1.5));
                Memory.WriteMemory(Memory.GTAMemoryAddresses.Camera2, Convert.ToSingle(3.6));
                Memory.WriteMemory(Memory.GTAMemoryAddresses.Camera3, Convert.ToSingle(0.06));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "infiniterun")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.InfiniteRun, Convert.ToUInt16(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "close")
            {
                label3.Text = L("MsgDisconnecting");
                Disconnect("Disconnected from server by CloseSGuardConnection()");
                label3.Text = L("MsgForceDisconnect");
                button2.Enabled = true;
                button1.Enabled = false;
            }
            else if (cmd == "auto")
            {
                label3.Text = L("MsgDisconnecting");
                Disconnect("Disconnected from server by CloseSGuardConnection()");
                label3.Text = L("MsgAutoDisconnect");
                button2.Enabled = true;
                button1.Enabled = false;
            }
            else if (cmd == "updatecfg")
            {
                string[] lines = File.ReadAllLines(sampcfgpath);
                string full = string.Empty;
                for (int i = 0; i < lines.Length; i++)
                {
                    full += lines[i].Split('=')[0] == data.Split(':')[1] ? (lines[i].Split('=')[0] + "=" + Convert.ToInt16(data.Split(':')[2])) : lines[i];
                    if (i < lines.Length - 1) full += "\n";
                }
            }
            else if (cmd == "vsirens")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.VehicleSirens, Convert.ToUInt16(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "moonsize")
            {
                Memory.OpenGTAProcess();
                moonsize = Convert.ToUInt16(data.Split(':')[1]);
                Memory.WriteMemory(Memory.GTAMemoryAddresses.MoonSize, moonsize);
                Memory.CloseGTAProcess();
            }
            else if (cmd == "moonsizelock")
                lockmoon = Convert.ToUInt16(data.Split(':')[1]) == 1;
            else if (cmd == "vision")
            {
                Memory.OpenGTAProcess();
                switch (Convert.ToUInt16(data.Split(':')[1]))
                {
                    case 0: Memory.WriteMemory(Memory.GTAMemoryAddresses.NightVision, Convert.ToUInt16(data.Split(':')[2])); break;
                    case 1: Memory.WriteMemory(Memory.GTAMemoryAddresses.ThermalVision, Convert.ToUInt16(data.Split(':')[2])); break;
                }
                Memory.CloseGTAProcess();
            }
            else if (cmd == "vnitro")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.VehicleNitro, Convert.ToSingle(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "explodetimer")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.VehicleExplodeTimer, Convert.ToSingle(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "trainspeed")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.TrainSpeed, Convert.ToSingle(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "hud")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.HUD, Convert.ToUInt16(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "blur")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(0x0704E8A, sizeof(byte), 0xE8);
                Memory.WriteMemory(0x0704E8B, sizeof(byte), 0x11);
                Memory.WriteMemory(0x0704E8C, sizeof(byte), 0xE2);
                Memory.WriteMemory(0x0704E8D, sizeof(byte), 0xFF);
                Memory.WriteMemory(0x0704E8E, sizeof(byte), 0xFF);
                Memory.WriteMemory(0x8D5104, sizeof(byte), Convert.ToUInt16(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "widescreen")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.Widescreen, Convert.ToUInt16(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "gettext")
            {
                string lastchars = string.Empty;
                try
                {
                    for (int i = 0; i < 10; i++)
                        lastchars += ((char)Convert.ToInt32(Memory.ValueOf(Memory.ReadMemory(0x969110 + i, sizeof(byte))))).ToString();
                    SendResponse(Variables.Requests.REQUEST_TEXT, lastchars);
                }
                catch
                {
                }
            }
            else if (cmd == "teamspeak")
                Process.Start("ts3server://" + data.Split(':')[1] + "?port=" + data.Split(':')[2]);
            else if (cmd == "clipboard")
                Clipboard.SetText(GetString(data));
            else if (cmd == "stamina")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.Stamina, Convert.ToUInt16(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "rotspeed")
            {
                Memory.OpenGTAProcess();
                Memory.WriteMemory(Memory.GTAMemoryAddresses.RotationSpeed, Convert.ToSingle(data.Split(':')[1]));
                Memory.CloseGTAProcess();
            }
            else if (cmd == "clipammo")
            {
                Memory.OpenGTAProcess();
                switch (Convert.ToUInt16(data.Split(':')[1]))
                {
                    case 1: Memory.WriteMemory(Memory.GTAMemoryAddresses.PistolClip, Convert.ToUInt16(data.Split(':')[2])); break;
                    case 2: Memory.WriteMemory(Memory.GTAMemoryAddresses.ShotgunClip, Convert.ToUInt16(data.Split(':')[2])); break;
                    case 3: Memory.WriteMemory(Memory.GTAMemoryAddresses.UziClip, Convert.ToUInt16(data.Split(':')[2])); break;
                    case 4: Memory.WriteMemory(Memory.GTAMemoryAddresses.AssaultClip, Convert.ToUInt16(data.Split(':')[2])); break;
                }
                Memory.CloseGTAProcess();
            }
        }
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < 2; i++)
                if (prcWatchers[i] != null)
                {
                    prcWatchers[i].Stop();
                    prcWatchers[i].Dispose();
                }
            if (Func.IsSecured())
                Func.SecureGTASA(false);
            if (!exitAfterDisconnect)
            {
                e.Cancel = true;
                if (connected)
                {
                    label3.Text = L("MsgDisconnecting");
                    Disconnect("Closed the program");
                    label3.Text = L("MsgOffline");
                    button2.Enabled = true;
                    button1.Enabled = false;
                }
                exitAfterDisconnect = true;
                this.Close();
            }
        }
        private void UpdateNickname()
        {
            toolStripStatusLabel4.Text = Func.GetRegistry("PlayerName", "Unknown", Strings.RegistryKey).ToString();
        }
        private void UpdateClientID(int n)
        {
            toolStripStatusLabel2.Text = (clientID = n) == -1 ? "N/A" : clientID.ToString();
        }
        private void UpdatePlayerID(int n)
        {
            toolStripStatusLabel6.Text = (playerID = n) == -1 ? "N/A" : playerID.ToString();
        }
        private void ToggleWindowStatus()
        {
            if (ws = !ws)
                this.Show();
            else
                this.Hide();
            notifyIcon1.Visible = !ws;
        }
        private void יציאהToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void sAMPILToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(url);
        }
        private void אודותToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Forms.About().ShowDialog();
        }
        private void מזערToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleWindowStatus();
        }
        private void הצגToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToggleWindowStatus();
        }
        private void sAMPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button3_Click(sender, e);
        }
        private void יציאהToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            ToggleWindowStatus();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                label4.Text = "IP: " + serverlist[e.RowIndex].Split(':')[0] + ":" + serverlist[e.RowIndex].Split(':')[1];
            }
            catch
            {
            }
        }
        private void הסרתציטיםToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int c = Func.HasCheats();
            if (c >= 2)
            {
                Forms.RemoveCheats rc = new Forms.RemoveCheats();
                rc.ShowDialog();
            }
            else
            {
                string x = L("MsgNoCheats");
                if (c == 1)
                    x += "\n" + L("MsgPrcCheats");
                Func.Error(x, false);
            }
        }
        private void הסרתCleoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string g = Func.GetGTASAPath(), path = g + "/CLEO", backup = g + "/CLEO-sGuard";
            if (Directory.Exists(path) || Directory.Exists(backup))
            {
                Forms.Cleo c = new Forms.Cleo();
                c.ShowDialog();
                if (c.ret == 1)
                {
                    if (Directory.Exists(backup))
                    {
                        if (Directory.Exists(path))
                            Func.Error(L("ErrorCleo1"), false);
                        else
                        {
                            Directory.Move(backup, path);
                            MessageBox.Show(L("MsgCleoOn"), "Cleo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                        Func.Error(L("ErrorCleo2"), false);
                }
                else if (c.ret == 2)
                {
                    if (Directory.Exists(path))
                    {
                        if (Directory.Exists(backup))
                            Func.Error(L("ErrorCleo3"), false);
                        else
                        {
                            Directory.Move(path, backup);
                            MessageBox.Show(L("MsgCleoOff"), "Cleo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                        Func.Error(L("ErrorCleo4"), false);
                }
            }
            else
                Func.Error(L("ErrorNoCleo"), false);
        }
        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName(Strings.SAMPProcess).Length > 0)
                MessageBox.Show(L("ErrorCNSamp"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                Forms.ChangeName cn = new Forms.ChangeName();
                cn.ShowDialog();
                if (cn.name.Length >= 3 && cn.name.Length <= 20)
                {
                    if (Process.GetProcessesByName(Strings.SAMPProcess).Length > 0)
                        MessageBox.Show(L("ErrorCNSamp"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        Func.SetRegistry("PlayerName", cn.name, Strings.RegistryKey);
                        UpdateNickname();
                        MessageBox.Show(L("MsgCN"), "Nickname", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            button2_Click(sender, e);
        }
        private string GetString(string data, string sep = ":")
        {
            return data.Remove(0, data.IndexOf(sep) + 1);
        }
        private void ReleaseCrashDetect()
        {
            crashdetect.Close();
            crashdetect.Dispose();
            if (File.Exists(crashfile))
                File.Delete(crashfile);
        }
        private void button2_EnabledChanged(object sender, EventArgs e)
        {
            toolStripDropDownButton1.Enabled = button2.Enabled;
        }
        private void ReleaseSocket()
        {
            if (s != null)
            {
                if(s.Connected)
                    s.Disconnect(false);
                s.Close();
            }
            s = null;
        }
        private void fileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            Func.Warning();
        }
        private void fileSystemWatcher1_Created(object sender, FileSystemEventArgs e)
        {
            Func.Warning();
        }
        private void fileSystemWatcher1_Deleted(object sender, FileSystemEventArgs e)
        {
            Func.Warning();
        }
        private void fileSystemWatcher1_Renamed(object sender, RenamedEventArgs e)
        {
            Func.Warning();
        }
        private bool IsConnected()
        {
            return Variables.proc != null && playerID != -1 && clientID != -1 && s != null;
        }
        private void prcStarted_EventArrived(object sender, EventArrivedEventArgs e)
        {
            try
            {
                if (IsConnected())
                    Send("prctrace 1 " + e.NewEvent.Properties["ProcessName"].Value);
            }
            catch //(Exception ex)
            {
                //Func.Error(L("ErrorPrcTrace") + "\n" + ex.Message, true);
            }
        }
        private void prcStopped_EventArrived(object sender, EventArrivedEventArgs e)
        {
            try
            {
                if (IsConnected())
                    Send("prctrace 0 " + e.NewEvent.Properties["ProcessName"].Value);
            }
            catch //(Exception ex)
            {
                //Func.Error(L("ErrorPrcTrace") + "\n" + ex.Message, true);
            }
        }
        private void UpdateMyLanguage()
        {
            RightToLeft = L("RTL") == "1" ? RightToLeft.Yes : RightToLeft.No;
            sAMPILToolStripMenuItem.Text = L("ToolStripSite");
            הסרתציטיםToolStripMenuItem.Text = L("ToolStripCheats");
            הסרתCleoToolStripMenuItem.Text = L("ToolStripCleo");
            אודותToolStripMenuItem.Text = L("ToolStripAbout");
            מזערToolStripMenuItem.Text = L("ToolStripMinimize");
            יציאהToolStripMenuItem.Text = L("ToolStripQuit");
            button2.Text = L("ButtonConnect");
            button1.Text = L("ButtonDisconnect");
            toolStripDropDownButton1.Text = L("ButtonChangeNick");
            linkLabel2.Text = L("ButtonPlay");
            label3.ResetText();
        }
        private string L(string n)
        {
            return Variables.language.GetString(n);
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ResourceManager lang = Variables.language;
            new Forms.Language().ShowDialog();
            if (Variables.language != lang)
            {
                Func.SetRegistry("Language", (Variables.language == Languages.Hebrew.ResourceManager ? 0 : 1).ToString(), Strings.SelfReg);
                UpdateMyLanguage();
            }
        }
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (IsConnected() && Variables.proc != null)
            {
                Memory.OpenGTAProcess();
                double col = Math.Min(Convert.ToDouble(Memory.ValueOf(Memory.ReadMemory(Memory.GTAMemoryAddresses.VehicleCollision))), 50.0);
                Memory.CloseGTAProcess();
                if (col > collmax)
                {
                    collmax = col;
                    collcd = 10;
                }
                if (collcd > 0)
                {
                    collcd--;
                    if (collcd == 0)
                    {
                        Send("collision " + collmax);
                        collmax = 0.0;
                    }
                }
            }
        }
        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (connected && serverID != -1)
            {
                Process[] samp;
                if ((samp = Process.GetProcessesByName(Strings.SAMPProcess)).Length > 0)
                    samp[0].Kill();
                string[] srv = serverlist[serverID].Split(':');
                Process.Start(Func.GetGTASAPath() + "/" + Strings.SAMPProcess + ".exe", srv[0] + ":" + srv[1]);
            }
        }
        private int CurrentLanguageID()
        {
            if (Variables.language == Languages.Hebrew.ResourceManager) return 0;
            else if (Variables.language == Languages.English.ResourceManager) return 1;
            return -1;
        }
    }
}