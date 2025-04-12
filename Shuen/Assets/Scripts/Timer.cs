using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;


public class Timer : MonoBehaviour
{
    public UnityEvent OnTimerEnd; // 使用UnityEvent类型，计时结束事件

    [Header("UI Elements")]
    public TMP_Text timerText;
    public TMP_InputField minutesInput; // 分钟输入框

    public GameObject pauseButton; // 暂停按钮
    public GameObject resumeButton; // 继续按钮

    [Header("Timer Settings")]
    public float timeLimit = 0f; // 默认1分钟
    public bool inHours = true;

    [Header("Audio Settings")]
    public AudioClip timerEndSound; // 计时结束音效
    public AudioSource audioSource; // 音频源组件

    TimeSpan timeConvertor; // 用于时间转换

    float time;
    bool startTimer;
    float remainingTime; // 用于保存暂停时的时间
    bool hasTriggeredEnd; // 防止重复触发结束事件



    private void Start()
    {
        timerText.text = timeLimit.ToString();
        time = timeLimit;

        startTimer = false;

        // 确保有AudioSource组件
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 初始化按钮状态
        pauseButton.SetActive(true);
        resumeButton.SetActive(false);

        // 初始化结束触发器 
        hasTriggeredEnd = false; 

    }

    public void StartTimer()
    {
        // 从输入框获取分钟并转换为秒
        if (!float.TryParse(minutesInput.text, out timeLimit))
        {
            timeLimit = 1f; // 默认值
        }
        
        time = timeLimit * 60; // 转换为秒

        remainingTime = time;  // 初始化剩余时间

        UpdateTimerDisplay();

        startTimer = true;

        // 初始化按钮状态
        pauseButton.SetActive(true);
        resumeButton.SetActive(false);
    }

    void UpdateTimerDisplay()
    {
        timeConvertor = TimeSpan.FromSeconds(remainingTime);
        
        string formattedTime = inHours ? 
            $"{timeConvertor.Hours:D2}:{timeConvertor.Minutes:D2}:{timeConvertor.Seconds:D2}" :
            $"{timeConvertor.Minutes:D2}:{timeConvertor.Seconds:D2}";
        
        timerText.text = formattedTime;
    }

    public void Update()
    {
        if(!startTimer)
        {
            return;
        }

        if(remainingTime>0f)
        {
            remainingTime-=Time.deltaTime;

            UpdateTimerDisplay();

            // if(inMinutes)
            // {
            //     timeConvertor = TimeSpan.FromSeconds(time);
            //     float minutes = timeConvertor.Minutes;
            //     float seconds = timeConvertor.Seconds;

            //     timerText.text = $"{minutes}:{seconds}";
            // }
            // else
            // {
            //     timerText.text = Mathf.CeilToInt(time).ToString();
            // }
            
            // slider.fillAmount = time * multiplierFactor; // 进度条
        }
        else
        {
            startTimer = false;

            // 计时结束恢复按钮状态
            pauseButton.SetActive(true);
            resumeButton.SetActive(false);


            // 触发结束事件（确保只触发一次）
            if (!hasTriggeredEnd)
            {
                OnTimerEnd.Invoke();
                hasTriggeredEnd = true;
            }
        }
    }

    // 添加播放音效的方法
    public void PlayEndSound()
    {
        if (timerEndSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(timerEndSound);
        }
    }


    public void PauseTimer()
    {
        if(startTimer)
        {
            startTimer = false;
            pauseButton.SetActive(false);
            resumeButton.SetActive(true);
        }
    }

    public void ResumeTimer()
    {
        startTimer = true;
        pauseButton.SetActive(true);
        resumeButton.SetActive(false);
    }

    public void ResetTimer()
    {
        StartTimer(); // 重新开始计时器
    }
}
