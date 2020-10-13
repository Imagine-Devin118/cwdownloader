using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using System.Threading.Tasks;

public class DisplayManager : MonoBehaviour
{
    [DllImport("user32.dll")]
    public static extern System.IntPtr FindWindowEx(IntPtr parent, IntPtr childe, string strclass, string strname);

    [DllImport("user32.dll")]
    public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [HideInInspector]
    public bool isExclusive = false;

    private async void Start()
    {
        IntPtr handle = (IntPtr)System.Diagnostics.Process.GetCurrentProcess().Id;
        EnumWindows(new EnumWindowsCallBack(EnumWindCallback), handle);
        ChangeTitle("想象力课堂资源离线包安装器");
        await MaximizesWindow();
    }

    public async Task MaximizesWindow()
    {
        Screen.SetResolution(DisplayParameter.ScreenWidth, DisplayParameter.ScreenHeight, false);
        Screen.fullScreen = false;
        await new UnityEngine.WaitForSeconds(0.25f);
        var p = FindWindowEx(System.IntPtr.Zero, System.IntPtr.Zero, null, Application.productName);
        ShowWindow(p, 5);
        isExclusive = false;
    }

    public async Task ExclusiveWindow()
    {
        Screen.SetResolution(DisplayParameter.ScreenWidth, DisplayParameter.ScreenHeight, true);
        Screen.fullScreen = true;
        await new UnityEngine.WaitForSeconds(0.25f);
        var p = FindWindowEx(System.IntPtr.Zero, System.IntPtr.Zero, null, Application.productName);
        SetWindowLong(p, -16, 369164288);
        MoveWindow(p, 0, 0, DisplayParameter.ScreenWidth, DisplayParameter.ScreenHeight, false);
        SetWindowLong(p, -16, 369164288);
        MoveWindow(p, 0, 0, DisplayParameter.ScreenWidth, DisplayParameter.ScreenHeight, false);
        SetWindowLong(p, -16, 369164288);
        MoveWindow(p, 0, 0, DisplayParameter.ScreenWidth, DisplayParameter.ScreenHeight, false);
        isExclusive = true;
    }


    #region WIN32API
    delegate bool EnumWindowsCallBack(IntPtr hwnd, IntPtr lParam);
    [DllImport("user32", CharSet = CharSet.Unicode)]
    static extern bool SetWindowTextW(IntPtr hwnd, string title);
    //回调指针，值
    [DllImport("user32")]
    static extern int EnumWindows(EnumWindowsCallBack lpEnumFunc, IntPtr lParam);
    [DllImport("user32")]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref IntPtr lpdwProcessId);
    [DllImport("user32")]
    static extern bool CloseWindow(IntPtr hwnd);
    [DllImport("user32")]
    static extern int SendMessageA(IntPtr hWnd, int wMsg, int wParam, IntPtr lParam);
    [DllImport("shell32")]
    static extern IntPtr ExtractIcon(int hInst, string lpszExeFileName, int nIconIndex);
    #endregion
    /// <summary>
    /// 当前窗口句柄
    /// </summary>
    public static IntPtr myWindowHandle;

    /// <summary>
    /// 改变窗口标题
    /// </summary>
    /// <param name="title"></param>
    public static void ChangeTitle(string title)
    {
        SetWindowTextW(myWindowHandle, title);//设置窗口标题
    }
    /// <summary>
    /// 更改窗口图标
    /// </summary>
    /// <param name="icon">图标路径</param>
    public static void ChangeICON(string icon)
    {
        IntPtr result = ExtractIcon(0, icon, 0);
        if (result != IntPtr.Zero)
            SendMessageA(myWindowHandle, 0x80, 0, result);
        else
            Debug.Log("SetIconFail");
    }

    private bool EnumWindCallback(IntPtr hwnd, IntPtr lParam)
    {
        IntPtr pid = IntPtr.Zero;
        GetWindowThreadProcessId(hwnd, ref pid);
        if (pid == lParam)  //判断当前窗口是否属于本进程
        {
            myWindowHandle = hwnd;
            return false;
        }
        return true;
    }

}

public static class DisplayParameter
{
    public static int ScreenWidth = 960;//分辨率宽度
    public static int ScreenHeight = 540;//分辨率高度
}
