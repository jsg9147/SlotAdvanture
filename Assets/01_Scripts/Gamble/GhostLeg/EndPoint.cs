using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider2D))]
public class EndPoint : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    public GhostLegController ghostLegManager;
    public Sprite xSprite;
    public Image iconImage;

    ItemData itemData;

    public bool win;
    public void SetWin(bool isWin)
    {
        this.win = isWin;
        if (!isWin)
            iconImage.sprite = xSprite;
    }

    public void SetReward(ItemData itemData)
    {
        if (win)
        {
            this.itemData = itemData;
            iconImage.sprite = itemData.icon;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Slime")
        {
            ghostLegManager.GameEnd(win);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(itemData)
            UIController.Instance.ItemInfoPopupActive(itemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIController.Instance.PopupOff();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        UIController.Instance.PopupOff();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UIController.Instance.SetActiveMoveIcon(false);
    }
}
