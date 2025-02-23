using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image cardImage;
    public int value;
    public bool isPlayerCard;

    public void SetValue(int cardValue, Sprite sprite)
    {
        if (cardValue <= 36)
        {
            value = cardValue % 9;
            if (value == 0)
                value = 9;
        }
        else
            value = 100;
        cardImage.sprite = sprite;
    }

    public void ResetValue()
    {
        value = 0;
        isPlayerCard = false;
        GetComponent<RectTransform>().anchorMin = new(0.5f, 0.5f);
        GetComponent<RectTransform>().anchorMax = new(0.5f, 0.5f);
        GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }
}
