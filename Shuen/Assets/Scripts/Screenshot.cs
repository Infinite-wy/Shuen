using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class Screenshot : MonoBehaviour
{
    // 双击检测参数
    private float doubleClickTime = 0.3f; // 双击最大间隔时间（秒）
    private float lastClickTime = 0;
    private bool isFirstClickInArea = false; // 标记第一次点击是否在区域内

    // Windows API声明
    [DllImport("user32.dll", SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

    private const byte VK_LWIN = 0x5B;    // 左Win键
    private const byte VK_SHIFT = 0x10;   // Shift键
    private const byte VK_S = 0x53;       // S键
    private const uint KEYEVENTF_KEYUP = 0x0002; // 按键释放标志

    void Update()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            // 射线检测（从相机到鼠标位置）
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            // 检测是否点击在当前物体的Box Collider 2D上
            if (hit.collider != null && hit.collider == GetComponent<BoxCollider2D>())
            {
                float timeSinceLastClick = Time.time - lastClickTime;

                if (timeSinceLastClick < doubleClickTime && timeSinceLastClick > 0)
                {
                    // 执行截图操作
                    SimulateWinShiftS();
                    isFirstClickInArea = false; // 重置双击状态
                }
                else
                {
                    isFirstClickInArea = true; // 标记第一次有效点击
                }

                lastClickTime = Time.time;
            }
            else
            {
                // 如果点击在区域外，重置状态
                isFirstClickInArea = false;
            }
        }
    }

    private void SimulateWinShiftS()
    {
        try
        {
            // 按下Win键
            keybd_event(VK_LWIN, 0, 0, UIntPtr.Zero);
            
            // 按下Shift键
            keybd_event(VK_SHIFT, 0, 0, UIntPtr.Zero);
            
            // 按下S键
            keybd_event(VK_S, 0, 0, UIntPtr.Zero);
            
            // 释放S键
            keybd_event(VK_S, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            
            // 释放Shift键
            keybd_event(VK_SHIFT, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            
            // 释放Win键
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

            Debug.Log("截图快捷键已触发");
        }
        catch (Exception e)
        {
            Debug.LogError("模拟按键失败: " + e.Message);
        }
    }
}