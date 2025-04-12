using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class WindowTransparent : MonoBehaviour
{
    // 自定义结构体
    private struct MARGINS
    {
        // 窗框的四个顶点坐标
        public int cxLeftWidth;
        public int cxTopHeight;
        public int cyRightWidth;
        public int cyBottomHeight;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);



    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong); 

    [DllImport("user32.dll")] 
    static extern uint GetWindowLong(IntPtr hWnd, int nIndex);



    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern int SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags); // 设置窗口透明度



    const int GWL_EXSTYLE = -20;

    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020;

    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1); // 窗口置顶

    const uint LWA_ALPHA = 0x00000002; // 使用Alpha通道替代颜色键
    const uint LWA_COLORKEY = 0x00000001; // 设置颜色键的标志



    const uint SWP_NOSIZE = 0x0001;
    const uint SWP_NOMOVE = 0x0002;
    const uint SWP_FRAMECHANGED = 0x0020;



    private IntPtr hWnd;
   
    
    private bool currentTransparentState = true;
    public BoxCollider2D[] interactiveColliders; // 可交互的碰撞器



    private void Start()
    {
        // MessageBox(new IntPtr(0), "Hello World!", "Hello", 0); // 测试

#if !Unity_EOITOR_

        hWnd = GetActiveWindow(); // 获取当前活动窗口句柄

        MARGINS margins = new MARGINS(){ cxLeftWidth = -1 }; // 创建结构体实例

        DwmExtendFrameIntoClientArea(hWnd, ref margins); // 调用函数

        uint baseStyle = GetWindowLong(hWnd, GWL_EXSTYLE); // 获取窗口样式

        SetWindowLong(hWnd, GWL_EXSTYLE, baseStyle | WS_EX_LAYERED); // 设置窗口样式

        // SetLayeredWindowAttributes(hWnd, 0, 255, LWA_COLORKEY); // 设置窗口透明度
        SetLayeredWindowAttributes(hWnd, 0, 255, LWA_ALPHA);

        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE); // 窗口置顶

#endif

        Application.runInBackground = true; // 设置程序在后台运行
    }
    

    void Update()
    {
#if !UNITY_EDITOR
        bool isOverCollider = CheckMouseOverCollider();
        UpdateWindowTransparency(isOverCollider);
#endif
    }

    bool CheckMouseOverCollider()
    {
        if (interactiveColliders == null || interactiveColliders.Length == 0)
            return false;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (var collider in interactiveColliders)
        {
            if (collider.OverlapPoint(mousePosition))
                return true;
        }
        return false;
    }

    void UpdateWindowTransparency(bool isOverCollider)
    {
        if (isOverCollider == currentTransparentState)
            return;

        uint currentStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
        uint newStyle = isOverCollider ? 
            currentStyle & ~WS_EX_TRANSPARENT : 
            currentStyle | WS_EX_TRANSPARENT;

        SetWindowLong(hWnd, GWL_EXSTYLE, newStyle);
        SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE);
        currentTransparentState = isOverCollider;
    }
}