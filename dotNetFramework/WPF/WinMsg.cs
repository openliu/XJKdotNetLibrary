﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using XJK.PInvoke;

namespace XJK.WPF
{
    public delegate void WndMsgProcPowerBroadcast(PBT pbt, WndMsgEventArgs e);

    public static class WinMsg
    {
        public static event WndMsgProc WndMsgProc;

        static WinMsg()
        {
            WinMsgHelperWindow.WndProcReceived += WinMsgHelperWindow_WndProcReceived;
        }

        private static void WinMsgHelperWindow_WndProcReceived(WndMsgEventArgs e)
        {
            WndMsgProc?.Invoke(e);
        }

        public static void AddEvent(WndMsgProc wndMsgProc)
        {
            WndMsgProc += wndMsgProc;
        }

        public static void AddEvent(int msg, WndMsgProc action)
        {
            AddEvent(e =>
            {
                if (e.Msg == msg)
                {
                    action(e);
                }
            });
        }

        public static void RegisterAutoRestart(WndMsgProc shutdownAction, string LaunchParameter = "")
        {
            AddEvent(e =>
            {
                if (e.Msg == WindowsMessages.QUERYENDSESSION)
                {
                    Kernel32.RegisterApplicationRestart(LaunchParameter, 0);
                }
                if (e.Msg == WindowsMessages.ENDSESSION)
                {
                    shutdownAction(e);
                }
            });
        }

        public static void RegisterPowerBroadcast(WndMsgProcPowerBroadcast wndMsgProc)
        {
            AddEvent(e =>
            {
                if (e.Msg == WindowsMessages.POWERBROADCAST)
                {
                    wndMsgProc((PBT)e.wParam, e);
                }
            });
        }

        public static void BroadcastMessage(int msg, string MsgBody = "")
        {
            User32.SendMessage(
                SpecialWindowHandles.HWND_BROADCAST,
                msg,
                IntPtr.Zero,
                MsgBody);
        }

    }
}
