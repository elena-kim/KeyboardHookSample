using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace KeyboardHookSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Dll Import

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, KeyboardProc callback, IntPtr hInstance, int threadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
        #endregion

        private delegate IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private readonly KeyboardProc _proc;
        private static IntPtr hHook = IntPtr.Zero;

        private const int HC_ACTION = 0;
        private const int WH_KEYBOARD = 2;
        private const int KF_REPEAT = 0x4000;

        #region Virtual Keys
        //https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
        
        private const int VK_F1 = 0x70;   //F1 key
        private const int VK_F2 = 0x71;   //F2 key
        private const int VK_F3 = 0x72;   //F3 key
        private const int VK_F4 = 0x73;   //F4 key
        private const int VK_F5 = 0x74;   //F5 key
        private const int VK_F6 = 0x75;   //F6 key
        private const int VK_F7 = 0x76;   //F7 key
        private const int VK_F8 = 0x77;   //F8 key
        private const int VK_F9 = 0x78;   //F9 key
        private const int VK_F10 = 0x79;  //F10 key
        private const int VK_F11 = 0x7A;  //F11 key
        private const int VK_F12 = 0x7B;  //F12 key
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            _proc = HookProc;
            SetHook();
        }

        public void SetHook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hHook = SetWindowsHookEx(WH_KEYBOARD, _proc, hInstance, GetCurrentThreadId());
        }

        public static void UnHook()
        {
            UnhookWindowsHookEx(hHook);
        }

        private IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code == HC_ACTION)
            {
                var repeat = HiWord(lParam) & KF_REPEAT;

                if (repeat == 0)
                {
                    Debug.WriteLine($"{code} / {wParam} / {lParam}");

                    switch ((int)wParam)
                    {
                        case VK_F1:
                            this.txb.Text += "F1 was pressed" + Environment.NewLine; break;
                        case VK_F2:
                            this.txb.Text += "F2 was pressed" + Environment.NewLine; break;
                        case VK_F3:
                            this.txb.Text += "F3 was pressed" + Environment.NewLine; break;
                        case VK_F4:
                            this.txb.Text += "F4 was pressed" + Environment.NewLine; break;
                        case VK_F5:
                            this.txb.Text += "F5 was pressed" + Environment.NewLine; break;
                        case VK_F6:
                            this.txb.Text += "F6 was pressed" + Environment.NewLine; break;
                    }
                }
            }
            return CallNextHookEx(hHook, code, (int)wParam, lParam);
        }

        private static ulong HiWord(IntPtr ptr)
        {
            if (((ulong)ptr & 0x80000000) == 0x80000000)
                return ((ulong)ptr >> 16);
            else
                return ((ulong)ptr >> 16) & 0xffff;
        }

        protected override void OnClosed(EventArgs e)
        {
            UnHook();
            base.OnClosed(e);
        }
    }
}
