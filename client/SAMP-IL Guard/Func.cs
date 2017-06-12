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
using Microsoft.Win32;
using System.Threading;
namespace SAMP_IL_Guard
{
    public class Func
    {
        public static WebClient x = new WebClient();
        public static System.IO.Stream y = null;
        public static System.IO.StreamReader z = null;
        public static string ReadFromWeb(string site)
        {
            string s = string.Empty;
            try
            {
                y = x.OpenRead(site);
                z = new System.IO.StreamReader(y, Encoding.UTF8);
                s = z.ReadToEnd();
                z.Close();
                y.Close();
            }
            catch
            {
                return null;
            }
            return s;
        }
        public static object GetRegistry(string key, string defaultValue, string reg)
        {
            RegistryKey RegKey = Registry.CurrentUser.OpenSubKey(reg);
            if (RegKey == null)
                return null;
            object tmp = RegKey.GetValue(key, defaultValue);
            RegKey.Close();
            return tmp;
        }
        public static void SetRegistry(string key, string value, string reg, bool c = false)
        {
            RegistryKey RegKey = c ? Registry.CurrentUser.CreateSubKey(reg) : Registry.CurrentUser.OpenSubKey(reg, true);
            RegKey.SetValue(key, value);
            RegKey.Close();
        }
        public static string UniqueID(bool localhost = false)
        {
            return GetRegistry("PlayerName", "Player", Strings.RegistryKey).ToString() + "/" + (localhost ? "127.0.0.1" : GetIP());
        }
        public static string GetIP()
        {
            string ip = string.Empty;
            try
            {
                ip = x.DownloadString("http://checkip.dyndns.org");
            }
            catch
            {
                Func.Error("התחברות לאינטרנט נכשלה.", true);
            }
            return new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b").Match(ip).Value;
        }
        public static string GetGTASAPath()
        {
            string path = Func.GetRegistry("gta_sa_exe", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "/Rockstar Games/GTA San Andreas", Strings.RegistryKey).ToString();
            path = path.Remove(path.LastIndexOf('\\'));
            return path;
        }
        public static void Error(string text, bool abort)
        {
            //Variables.mainForm.Disconnect("Client error");
            Variables.errForm.SetError(text, abort);
            if (!Variables.errForm.Visible && !Variables.errForm.IsDisposed && Variables.errForm != null)
                Variables.errForm.ShowDialog();
        }
        public static void Backup(string path)
        {
            if (File.Exists(path))
            {
                string backups = GetGTASAPath() + "/sGuard Backups";
                if (!Directory.Exists(backups))
                {
                    Directory.CreateDirectory(backups);
                    File.WriteAllText(backups + "/מידע.txt",
@"זוהי תיקיית גיבוי שנוצרה באופן אוטומטי.
כאשר אחד משרתי הSA-MP שנכנסת אליהם מחליף קובץ קיים במחשב שלך,
גיבוי של אותו הקובץ נשמר בתיקייה זו.

תוכל להשתמש בגיבוי כדי לשחזר את אותם הקבצים.

אם אתה חושב שהיה נסיון לפעול נגדך בהחלפת הקובץ,
נא לעדכן אותנו בכתובת הבאה:
http://Guard.SA-MP.co.il/");
                }
                File.Move(path, backups + "/" + new FileInfo(path).Name);
            }
        }
        public static int HasCheats()
        {
            int type = 0;
            new Thread(() =>
            {
                Process[] plist = Process.GetProcesses();
                long s = 0;
                List<long> sizes = new List<long>();
                for (int i = 0; i < plist.Length && type == 0; i++)
                {
                    for (int j = 0, m = Variables.lists[Variables.LIST_DENIED].GetLength(0); j < m; j++)
                        if (plist[i].ProcessName.ToLower().Contains(Variables.lists[Variables.LIST_DENIED][j]) ||
                            plist[i].MainWindowTitle.ToLower().Contains(Variables.lists[Variables.LIST_DENIED][j]))
                            type = 1;
                }
                if (Variables.prcListBasedCount != plist.Length)
                {
                    Variables.prcSizes.Clear();
                    Variables.prcListBasedCount = plist.Length;
                    for (int i = 0; i < plist.Length; i++)
                    {
                        try
                        {
                            s = new FileInfo(plist[i].Modules[0].FileName).Length;
                            if (!Variables.prcSizes.Contains(s))
                                Variables.prcSizes.Add(s);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                sizes.AddRange(Variables.prcSizes);
                string flist = string.Empty, path = string.Empty;
                string[] files = Directory.GetFiles(path = Func.GetGTASAPath());
                for (int i = 0, m = files.GetLength(0); i < files.Length; i++)
                {
                    flist += files[i] + (i == m - 1 ? "" : ",");
                    s = new FileInfo(files[i]).Length;
                    if (!sizes.Contains(s))
                        sizes.Add(s);
                }
                for (int i = 0; i < flist.Length; i++)
                    if (flist[i] >= 'A' && flist[i] <= 'Z')
                        flist = flist.Replace(flist[i], (char)((int)flist[i] + 32));
                for (int i = 0, m = Variables.lists[Variables.LIST_DENIED].GetLength(0); i < m && type < 2; i++)
                    if (flist.Contains(Variables.lists[Variables.LIST_DENIED][i]))
                        type = type == 1 ? 3 : 2;
                for (int i = 0, m = Variables.lists[Variables.LIST_DENIED_S].GetLength(0); i < m && type < 2; i++)
                    if (sizes.Contains(Convert.ToInt64(Variables.lists[Variables.LIST_DENIED_S][i])))
                        type = type == 1 ? (i < plist.Length ? 3 : 1) : 1;
            }).Start();
            return type;
        }
        public static List<FileInfo> GetCheats()
        {
            string[] files = Directory.GetFiles(Func.GetGTASAPath());
            List<FileInfo> ret = new List<FileInfo>();
            FileInfo tmp = null;
            for (int i = 0; i < files.Length; i++)
            {
                tmp = new FileInfo(files[i]);
                for (int j = 0; j < Variables.lists[Variables.LIST_DENIED].Length; j++)
                    if (tmp.Name.ToLower().Contains(Variables.lists[Variables.LIST_DENIED][j]))
                        ret.Add(tmp);
                if (ret.Contains(tmp))
                    continue;
                for (int j = 0; j < Variables.lists[Variables.LIST_DENIED_S].Length; j++)
                    if (tmp.Length == Convert.ToInt64(Variables.lists[Variables.LIST_DENIED_S][j]))
                        ret.Add(tmp);
            }
            return ret;
        }
        public static void Warning()
        {
            //Func.Error("אני רואה שאתה מנסה לעקוף את המערכת שלי. זה נחמד! אבל לפעם הבאה, תנסה להיות חכם יותר ואל תעשה את זה כשהתוכנה פועלת.", true);
        }
        public static void SecureGTASA(bool s)
        {
            string path = GetGTASAPath() + "\\";
            const string n = "gta_sa";
            string gtasa = path + n + ".exe";
            if (s)
            {
                //int[] ss = new int[] { 8300, 8301, 8302, 8303 };
                //Random r = new Random();
                Variables.modifiedName = ConfuseString("sggta_sa");
                File.Move(gtasa, path + Variables.modifiedName + ".exe");
                Func.SetRegistry("gta_sa_exe", path + Variables.modifiedName + ".exe", Strings.RegistryKey);
            }
            else
            {
                if (File.Exists(gtasa))
                    File.Delete(gtasa);
                File.Move(path + Variables.modifiedName + ".exe", gtasa);
                Variables.modifiedName = n;
                Func.SetRegistry("gta_sa_exe", gtasa, Strings.RegistryKey);
            }
        }
        public static bool IsSecured()
        {
            return !File.Exists(GetGTASAPath() + "\\gta_sa.exe") && !HasSecureProblem();
        }
        public static bool HasSecureProblem()
        {
            string f = GetGTASAPath() + "\\gta_sa.exe";
            if (File.Exists(f))
            {
                try
                {
                    long len = 0;
                    using (FileStream fh = File.Open(f, FileMode.Append))
                    {
                        len = fh.Length;
                        fh.Close();
                    }
                    if (len < 100) return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }
        public static void FixSecure()
        {
            if (IsSecured() || HasSecureProblem())
            {
                string conf = "sggta_sa.exe", path = GetGTASAPath() + "\\";
                string[] fls = Directory.GetFiles(path, "*.exe");
                int idx = -1, c = 0;
                for (int i = 0, m = fls.GetLength(0); i < m && idx == -1; i++)
                {
                    c = 0;
                    fls[i] = fls[i].Replace(path, "");
                    for (int j = 0; j < fls[i].Length; j++)
                        if (conf.Contains(fls[i][j].ToString()))
                            c++;
                    if(c == conf.Length)
                        idx = i;
                }
                if (idx > -1)
                {
                    string gtasa = path + "gta_sa.exe";
                    if (File.Exists(gtasa))
                        File.Delete(gtasa);
                    File.Move(path + fls[idx], gtasa);
                    Func.SetRegistry("gta_sa_exe", gtasa, Strings.RegistryKey);
                }
            }
        }
        public static string ConfuseString(string s)
        {
            Random r = new Random();
            string ret = string.Empty;
            bool[] set = new bool[s.Length];
            for (int i = 0, idx = -1; i < s.Length; i++)
            {
                do idx = r.Next(s.Length);
                while(set[idx]);
                set[idx] = true;
                ret += s[idx];
            }
            return ret;
        }
    }
}