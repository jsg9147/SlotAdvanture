using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoll : MonoBehaviour
{
    public Sprite[] diceFaces;

    private Image imageIcon;
    private bool isRolling = false;
    private int result;

    public int Result
    {
        get { return result; }
    }

    void Start()
    {
        imageIcon = GetComponent<Image>();
    }

    public void StartRoll(float rollDuration)
    {
        result = Random.Range(0, diceFaces.Length);
        if (!isRolling)
        {
            isRolling = true;
            // 타이머를 시작하여 일정 간격으로 주사위 이미지를 변경
            InvokeRepeating("ChangeDiceFace", 0.0f, 0.1f);

            // 일정 시간이 지난 후 굴림을 멈추도록 설정
            Invoke("StopRoll", rollDuration);
        }
    }
    private void ChangeDiceFace()
    {
        // 주사위 이미지를 무작위로 선택하여 변경
        int randomFaceIndex = Random.Range(0, diceFaces.Length);
        imageIcon.sprite = diceFaces[randomFaceIndex];
    }

    private void StopRoll()
    {
        isRolling = false;
        imageIcon.sprite = diceFaces[result];
        // 굴리기를 멈추면 여기서 결과 처리 로직을 추가할 수 있습니다.
    }
}
