using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FairyGUI
{
    public class KeybordTool : MonoBehaviour
    {
        private static KeybordTool instance;

        public static KeybordTool Instance
        {
            get
            {
                if (!instance)
                {
                    instance = FindObjectOfType<KeybordTool>();
                    if (!instance)
                    {
                        var manager = new GameObject("KeybordTool");
                        instance = manager.AddComponent<KeybordTool>();
                    }
                }
                return instance;
            }
        }

        private bool forcePopup = true;
        static Coroutine closeCoroutine = null;

        #region Win32API Wrapper
        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "PostMessage")]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);
        private const Int32 WM_SYSCOMMAND = 274;
        private const UInt32 SC_CLOSE = 61536;
        #endregion

        static KeybordTool()
        {
            HideKeyboard();
        }

        #region Coroutine
        IEnumerator DelayOpen()
        {
            yield return new WaitForEndOfFrame();
            string _file = "C:\\Program Files\\Common Files\\microsoft shared\\ink\\TabTip.exe";
            if (File.Exists(_file))
            {
                using (Process _process = Process.Start(_file)) { };
            }
        }
        IEnumerator DelayQuit()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            HideKeyboard();
        }
        #endregion

        static void HideKeyboard()
        {
            try
            {
                IntPtr _touchhWnd = IntPtr.Zero;
                _touchhWnd = FindWindow("IPTip_Main_Window", null);
                if (_touchhWnd != IntPtr.Zero) PostMessage(_touchhWnd, WM_SYSCOMMAND, SC_CLOSE, 0);
            }
            catch { }
        }

        #region EventSystem Interface Implement
        public void Select()
        {
            if (null != closeCoroutine)
            {
                StopCoroutine(closeCoroutine);
                closeCoroutine = null;
            }
            StartCoroutine("DelayOpen");
        }

        public void Deselect()
        {
            if (null == closeCoroutine)
            {
                closeCoroutine = StartCoroutine("DelayQuit");
            }
        }

        void OnDestroy()
        {
            HideKeyboard();
        }
        #endregion
    }
}