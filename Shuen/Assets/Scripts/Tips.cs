using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tips : MonoBehaviour
{
    public GameObject ShuenTips;
    public bool isShuenTipsOpen = false; 

    void Start()
    {
        // 根据初始状态决定是否显示 ShuenTips
        if (isShuenTipsOpen && ShuenTips != null)
        {
            ShuenTips.SetActive(true);
        }
    }

    public void ReadOver()
    {
        // 隐藏被点击的物体
        ShuenTips.SetActive(false);
        isShuenTipsOpen = false; // 更新状态
    }
}
