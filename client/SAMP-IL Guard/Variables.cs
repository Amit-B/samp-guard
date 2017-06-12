using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
namespace SAMP_IL_Guard
{
    public static class Variables
    {
        public enum Requests
        {
            INVALID_REQUEST = -001,
            REQUEST_PROC = 000,
            REQUEST_CHEATS = 001,
            REQUEST_CLEO = 002,
            REQUEST_PROCLIST = 003,
            REQUEST_HWID = 004,
            REQUEST_TEXT = 005,
            REQUEST_TXD = 006
        }
        public static Forms.Error errForm = new Forms.Error();
        public static Main mainForm = null;
        private static Process proc_ = null;
        public static Process proc
        {
            get
            {
                Process[] pr = Process.GetProcessesByName(modifiedName);
                //mainForm.Text = (modifiedName + " = " + pr.Length);
                return proc_ = (pr.Length > 0 ? pr[0] : null);
            }
        }
        public const int LIST_DENIED = 0;
        public const int LIST_BANNED = 1;
        public const int LIST_DENIED_S = 2;
        public static string[] files = { "denied.txt", "banned.txt", "denied_s.txt" };
        public static List<string[]> lists = new List<string[]>();
        public static System.Resources.ResourceManager language = Languages.Hebrew.ResourceManager;
        public static string arg = string.Empty;
        public static List<long> prcSizes = new List<long>();
        public static int prcListBasedCount = 0;
        public static string modifiedName = "gta_sa";
    }
}