using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class BossRoomEffect : MonoBehaviour
{
    public float scrollSpeed = 5.0f;
    public float tileSizeZ = 5.0f;

    public RectTransform topWarning;
    public RectTransform bottomWarning;

    private Vector3 startTopPosition;
    private Vector3 startBotPosition;
    private void Start()
    {
        startTopPosition = topWarning.anchoredPosition;
        startBotPosition = bottomWarning.anchoredPosition;
    }

    private void FixedUpdate()
    {
        WarningMove();
    }

    void WarningMove()
    {
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);

        // 새로운 위치로 배경 이동
        topWarning.anchoredPosition = startTopPosition + (Vector3.left * newPosition);
        bottomWarning.anchoredPosition = startBotPosition + (Vector3.right * newPosition);
    }
}
