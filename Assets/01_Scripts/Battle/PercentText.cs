using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PercentText : MonoBehaviour
{
    public TMP_Text percentText;

    //public Transform frontCanvas;

    public GameObject fireEffect;
    public GameObject emojiEffect;

    public float lowPercent;
    public float middlePercent;
    public float highPercent;
    public float jackpotPercent;

    private void Start()
    {
        fireEffect.SetActive(false);
        SetSadEmoji(false);
        percentText.text = "0.0 %";

        percentText.transform.SetParent(GameManager.Instance.FrontCanvas.transform);
    }

    public void SetPercent(float percent)
    {
        percentText.text = (percent * 0.1f).ToString("F" + 1) + " %";
        ChangeColor(percent);
    }

    void ChangeColor(float percent)
    {
        if (percent < lowPercent)
        {
            SetSadEmoji(true);
        }
        else if (lowPercent < percent && percent < middlePercent)
        {

        }
        else if (middlePercent < percent && percent < highPercent)
        {
            FireEffect();
        }

        if (percent == jackpotPercent)
        {

        }
    }

    void FireEffect()
    {
        fireEffect.SetActive(true);

    }

    void SetSadEmoji(bool isActive)
    {
        if (emojiEffect)
            emojiEffect.SetActive(isActive);
    }

    public void EndEffect()
    {
        fireEffect.SetActive(false);

        SetSadEmoji(false);
    }
}
