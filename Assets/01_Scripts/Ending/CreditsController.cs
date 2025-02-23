using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditsController : MonoBehaviour
{
    public TMP_Text creditsText;
    public float scrollSpeed = 20f;
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        animator.Play("CreditsAnimation");
    }

    void FixedUpdate()
    {
        // 텍스트를 위로 움직이기
        RectTransform rectTransform = creditsText.GetComponent<RectTransform>();
        rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);

        // 텍스트가 일정 위치까지 도달하면 움직임을 멈춤 (원하는 위치로 수정)
        if (rectTransform.anchoredPosition.y > 500f)
        {
            // 여기에서 다른 작업 수행 가능
            // 예를 들면 씬 전환 등
        }
    }
}
