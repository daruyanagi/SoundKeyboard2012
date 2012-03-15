using System;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace SoundKeyboard2012
{
    public static class GlobalKeybordMonitor
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hHook);

        [DllImport("user32.dll")]
        static extern short VkKeyScan(char ch);

        public const int WH_KEYBOARD_LL = 13;
        public const int HC_ACTION = 0;
        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_SYSKEYDOWN = 0x0104;
        public const int WM_SYSKEYUP = 0x0105;

        public sealed class GlobalKeyEventArgs : EventArgs
        {
            internal GlobalKeyEventArgs(KBDLLHOOKSTRUCT keyData)
            {
                KeyCode = keyData.vkCode;
                KeyData = KeyInterop.KeyFromVirtualKey(keyData.vkCode);
                Flags = keyData.flags;
                ScanCode = keyData.scanCode;
                Time = keyData.time;
                Handled = false;
            }

            public int KeyCode  { get; private set; }
            public Key KeyData  { get; private set; }
            public int Flags    { get; private set; }
            public int ScanCode { get; private set; }
            public int Time     { get; private set; }
            public bool Handled { get; set; }
        }

        private static IntPtr _Hook;
        private static LowLevelKeyboardProc _HookProc = new LowLevelKeyboardProc(HookProc);

        // public static event EventHandler<GlobalKeyEventArgs> SysKeyDown;
        public static event EventHandler<GlobalKeyEventArgs> KeyDown;
        // public static event EventHandler<GlobalKeyEventArgs> SysKeyUp;
        public static event EventHandler<GlobalKeyEventArgs> KeyUp;

        public static bool Enabled { get { return _Hook != IntPtr.Zero; } }

        static GlobalKeybordMonitor()
        {
            _Hook = SetWindowsHookEx(WH_KEYBOARD_LL, _HookProc, GetModuleHandle(null), 0);
            
            AppDomain.CurrentDomain.DomainUnload += (_sender, _e) =>
            {
                if (_Hook != IntPtr.Zero) UnhookWindowsHookEx(_Hook);
            };
        }

        private static IntPtr HookProc(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            bool handled = false;

            if (nCode == HC_ACTION)
            {
                GlobalKeyEventArgs event_args = new GlobalKeyEventArgs(lParam);
                switch (wParam.ToInt32())
                {
                    case WM_KEYDOWN:
                    case WM_SYSKEYDOWN:
                        CallEvent(KeyDown, event_args);
                        break;

                    case WM_KEYUP:
                    case WM_SYSKEYUP:
                        CallEvent(KeyUp, event_args);
                        break;
                }
                handled = event_args.Handled;
            }

            return handled
                ? (IntPtr) 1
                : CallNextHookEx(_Hook, nCode, wParam, ref lParam);
        }

        private static void CallEvent(
            EventHandler<GlobalKeyEventArgs> event_handler, GlobalKeyEventArgs event_args)
        {
            if (event_handler != null) event_handler(null, event_args);
        }
    }
}
