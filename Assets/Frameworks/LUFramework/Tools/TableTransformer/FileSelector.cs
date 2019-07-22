/*
 * 时间 : 2019/7/16
 * 作者 : LeeroyLin
 * 项目 : 表转换器
 * 描述 : 文件选择器
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace LUFramework
{
    public delegate int BrowseCallBackProc(IntPtr hwnd, int msg, IntPtr lp, IntPtr wp);

    /// <summary>
    /// 文件选择器
    /// </summary>
    public class FileSelector
    {
        #region 常量
        // Constants for sending and receiving messages in BrowseCallBackProc
        public const int WM_USER = 0x400;
        public const int BFFM_INITIALIZED = 1;
        public const int BFFM_SELCHANGED = 2;
        public const int BFFM_VALIDATEFAILEDA = 3;
        public const int BFFM_VALIDATEFAILEDW = 4;
        public const int BFFM_IUNKNOWN = 5; // provides IUnknown to client. lParam: IUnknown*
        public const int BFFM_SETSTATUSTEXTA = WM_USER + 100;
        public const int BFFM_ENABLEOK = WM_USER + 101;
        public const int BFFM_SETSELECTIONA = WM_USER + 102;
        public const int BFFM_SETSELECTIONW = WM_USER + 103;
        public const int BFFM_SETSTATUSTEXTW = WM_USER + 104;
        public const int BFFM_SETOKTEXT = WM_USER + 105; // Unicode only
        public const int BFFM_SETEXPANDED = WM_USER + 106; // Unicode only
        #endregion

        #region 引入外部方法
        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SHBrowseForFolder([In, Out] SelectFileInfo ofn);

        [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);

        [DllImport("user32.dll", PreserveSig = true)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, string lParam);
        #endregion

        #region 公共方法
        /// <summary>
        /// 获得选择文件名
        /// </summary>
        /// <returns>选择的文件名</returns>
        public static string GetSelectFileName()
        {
            SelectFileInfo selectFileName = new SelectFileInfo
            {
                pszDisplayName = new string(new char[2000]),
                lpszTitle = "选择文件夹",
                lpfn = new BrowseCallBackProc(OnBrowseEvent)
            };
            IntPtr intPtr = SHBrowseForFolder(selectFileName);
            char[] chArray = new char[2000];
            for (int i = 0; i < chArray.Length; i++)
            {
                chArray[i] = '\0';
            }
            SHGetPathFromIDList(intPtr, chArray);
            string fullPath = new string(chArray);
            fullPath = fullPath.Substring(0, fullPath.IndexOf('\0'));
            return fullPath;
        }

        /// <summary>
        /// 事件处理方法
        /// </summary>
        public static int OnBrowseEvent(IntPtr hWnd, int msg, IntPtr lp, IntPtr lpData)
        {
            switch (msg)
            {
                case BFFM_INITIALIZED: // Required to set initialPath
                {
                    //Win32.SendMessage(new HandleRef(null, hWnd), BFFM_SETSELECTIONA, 1, lpData);
                    // Use BFFM_SETSELECTIONW if passing a Unicode string, i.e. native CLR Strings.
                    SendMessage(new HandleRef(null, hWnd), BFFM_SETSELECTIONW, 1, Application.dataPath.Replace("/", "\\"));
                    break;
                }
                case BFFM_SELCHANGED:
                {
                    IntPtr pathPtr = Marshal.AllocHGlobal((int)(260 * Marshal.SystemDefaultCharSize));

                    /*
                    if (SHGetPathFromIDList(lp, pathPtr))
                        SendMessage(new HandleRef(null, hWnd), BFFM_SETSTATUSTEXTW, 0, pathPtr);
                    Marshal.FreeHGlobal(pathPtr);
                    */
                    break;
                }
            }

            return 0;
        }
        #endregion
    }

    /// <summary>
    /// 选择文件信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class SelectFileInfo
    {
        public IntPtr hwndOwner = IntPtr.Zero;
        public IntPtr pidlRoot = IntPtr.Zero;
        public string pszDisplayName = null;
        public string lpszTitle = null;
        public uint ulFlags = 0;
        public BrowseCallBackProc lpfn;
        public IntPtr lParam = IntPtr.Zero;
        public int iImage = 0;
    }
}