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

namespace PL
{
    public partial class PuttyForm : Form
    {
        static Process process = null;
        const int SW_MAXIMIZE = 1;
       
        public PuttyForm()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll")]
        static extern int ShowWindowAsync(IntPtr hwnd, int nCmdShow);
        private void button1_Click(object sender, EventArgs e)
        {
            ProcessStartInfo cmd = new ProcessStartInfo
            {
                FileName = Directory.GetCurrentDirectory() + @"\data\Putty\putty.exe",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                Arguments = $"-ssh root@195.246.106.130 222 -pw xxxxxx",
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            process = Process.Start(cmd);
            //this.WindowState = FormWindowState.Maximized;
            process.WaitForInputIdle();

            SetParent(process.MainWindowHandle, splitContainer1.Panel2.Handle);
            MoveWindow(process.MainWindowHandle, -5, -30, splitContainer1.Panel2.Width + 15, splitContainer1.Panel2.Height + 40, false);

            ShowWindowAsync(process.MainWindowHandle, SW_MAXIMIZE);

        }

        private void PuttyForm_Resize(object sender, EventArgs e)
        {
            if(process != null)
            {
                MoveWindow(process.MainWindowHandle, -5, -30, splitContainer1.Panel2.Width + 15, splitContainer1.Panel2.Height + 40, false);
            }
        }
    }
}
