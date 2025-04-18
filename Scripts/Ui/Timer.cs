using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    public float startTime = 30f; // 초기 시간 설정
    private float timeRemaining;
    public Action OnTimerEnd; // 타이머 종료 시 실행

    public void Init()
    {
        timeRemaining = startTime;
        enabled = true;
    }

    private void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = $"Time: {timeRemaining:F2} seconds";
        }
        else
        {
            timerText.text = "Time: 0.00 seconds";
            OnTimerEnd?.Invoke(); // 타이머 종료 이벤트 호출
            enabled = false; // 타이머 업데이트 중지
        }
    }

    private void OnDestroy()
    {
        OnTimerEnd = null; // 소멸할때 제거한다.
    }
}