using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KeyboardHook
{
       class Program
       {
              const int WM_KEYDOWN = 0x100;
              const int WM_KEYUP = 0x101;
              const int WM_SYSKEYDOWN = 0x104;
              const int WM_SYSKEYUP = 0x105;
              [StructLayout(LayoutKind.Sequential)]
              private struct KBDLLHOOKSTRUCT
              {
                     public Keys key;
                     public int vkCode;
                     public int scanCode;
                     public int flags;
                     public int time;
                     public IntPtr extra;
              }

                public Program()
                {
                    Hook();
                }
                ~Program()
                {
                    UnHook();
                }
                private delegate IntPtr LowLevelKeyBoardProc(int nCode, int lParam, IntPtr wParam);
                private LowLevelKeyBoardProc keyboardProcess;

              public static IntPtr ptrHook;

              public event KeyEventHandler KeyUp;
              public event KeyEventHandler KeyDown;

              [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
              private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyBoardProc callback, IntPtr hMod, uint dwThreadId);

              [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
              private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, int wp, IntPtr lp);

              [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
              private static extern IntPtr GetModuleHandle(string name);

              [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
              public static extern bool UnhookWindowsHookEx(IntPtr hook);

            [DllImport("user32.dll")]
            public static extern bool GetMessage(dynamic msg, dynamic hWnd, dynamic mMin, dynamic mMax);

        public void Hook()
              {
                     ProcessModule proccessModule = Process.GetCurrentProcess().MainModule;
                     keyboardProcess = new LowLevelKeyBoardProc(CaptureKey);
                     ptrHook = SetWindowsHookEx(13, keyboardProcess, GetModuleHandle(proccessModule.ModuleName), 0);
              }

              public void UnHook()
              {
                     UnhookWindowsHookEx(ptrHook);
              }

        private IntPtr CaptureKey(int nCode, int wp, IntPtr lp)
        {

            if (nCode >= 0) { 
                KBDLLHOOKSTRUCT keyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));
                KeyEventArgs eventArgs = new KeyEventArgs(keyInfo.key);

                Console.WriteLine(keyInfo.key+ " " + wp + " " + lp + " - " + KeyDown);
                /*
                if (wp == WM_KEYDOWN || wp == WM_SYSKEYDOWN) {
                    KeyDown(this, eventArgs);
                }
                else if (wp == WM_KEYUP || wp == WM_SYSKEYUP){
                    KeyUp(this, eventArgs);
                }
                */
    
            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
        }


        static void Main(string[] args)
              {
                    Program p = new Program();
                    p.Hook();

                    while (GetMessage(null, null,0, 0));
              }

       }
}
