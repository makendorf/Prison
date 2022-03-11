using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Network;
using System.Net;

namespace PL
{
    public partial class PuttyForm : Form
    {
        static Process process = null;
        const int SW_MAXIMIZE = 1;
        static List<ShopBox> ShopListBox = new List<ShopBox>();
        static List<Process> ProcessList = new List<Process>();
        static ClientID ShopID; 
        public PuttyForm(ClientID clID)
        {
            InitializeComponent();
            if(clID.Name == Dns.GetHostName())
            {
                ShopListBox.AddRange(clID.ShopBoxList);
            }
            else
            {
                foreach(var item in clID.ShopBoxList)
                {
                    var box = item;
                    box.IP = clID.IP;
                    box.Port += 200;
                    ShopListBox.Add(box);
                }
            }
            
            ShopID = clID;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        static extern int ShowWindowAsync(IntPtr hwnd, int nCmdShow);

        private void PuttyForm_Load(object sender, EventArgs e)
        {
            for(int i = 0; i < ShopListBox.Count; i++)
            {
                ShopBoxTabControl.TabPages.Add(ShopListBox[i].Name);
            }
        }
        
        private void PuttyForm_Resize(object sender, EventArgs e)
        {
            if(process != null)
            {
                for(int i = 0; i < ShopBoxTabControl.TabCount; i++)
                {
                    MoveWindow(ProcessList[i].MainWindowHandle, -5, -30, splitContainer1.Panel2.Width + 5, splitContainer1.Panel2.Height + 10, false);
                }
            }
        }

        private void StartSessionButton1_Click(object sender, EventArgs e)
        {
            int ID = ShopBoxTabControl.SelectedIndex;
            ProcessStartInfo cmd = new ProcessStartInfo
            {
                FileName = Directory.GetCurrentDirectory() + @"\data\Program\putty.exe",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                Arguments = $"-ssh root@{ShopListBox[ID].IP} {ShopListBox[ID].Port} -pw xxxxxx",
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            process = Process.Start(cmd);
            //this.WindowState = FormWindowState.Maximized;
            process.WaitForInputIdle();
            ProcessList.Add(process);
            SetParent(ProcessList[ID].MainWindowHandle, ShopBoxTabControl.TabPages[ID].Handle);
            MoveWindow(ProcessList[ID].MainWindowHandle, -5, -30, splitContainer1.Panel2.Width + 5, splitContainer1.Panel2.Height + 10, false);

            ShowWindowAsync(process.MainWindowHandle, SW_MAXIMIZE);
        }

        private void ShopBoxTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            IPLabel.Text = ShopListBox[ShopBoxTabControl.SelectedIndex].IP + ":" + ShopListBox[ShopBoxTabControl.SelectedIndex].Port;
        }
    }
}
