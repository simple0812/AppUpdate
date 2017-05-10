using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace DashboardApp
{
   public static class FileHelper
    {
        [DllImport("kernel32")]
        private static extern SafeFileHandle CreateFileW(string path, FileAccess access, FileShare share, IntPtr prt, FileMode mode, int x, IntPtr prtx);

       public static void Pipe()
       {
            var handle = CreateFileW(@"\\.\pipe\mypipe", FileAccess.ReadWrite, FileShare.None, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            if (handle.IsInvalid)
            {
                var err = Marshal.GetLastWin32Error();
                Debug.WriteLine(err);
                return;
                //handle error
            }
            var sw = new StreamWriter(new FileStream(handle, FileAccess.Write), Encoding.UTF8);
            sw.WriteLine("Hello from AppContainer");
            sw.Flush();

        }

    }
}
