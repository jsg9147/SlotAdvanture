using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ConsumeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDropHandler, IDragHandler, IPointerClickHandler
{
    public int index;

    [HideInInspector]
    public ItemData itemData;
    public Sprite baseIcon;

    public Image itemSprite;
    public TMP_Text countText;

    public int itemCount;

    public void OnDrag(PointerEventData eventData)
    {
        // 없으면 Drop 이 동작 안함
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (ItemManager.Instance.selectedItemSlot != null)
        {
            if (ItemManager.Instance.selectedItemSlot.itemData != null)
            {
                if (ItemManager.Instance.selectedItemSlot.itemData.type != ItemType.Consumable)
                    return;
            }

            ItemData tempData = itemData;
            int tempCount = itemCount;
            SetItemObject(ItemManager.Instance.selectedItemSlot.itemData);
            SpriteChange(ItemManager.Instance.selectedItemSlot.itemData);
            itemCount = ItemManager.Instance.selectedItemSlot.itemCount;
            CountUpdate();

            ChangeConsumeSlot(index);

            if (tempData == null)
            {
                ItemManager.Instance.selectedItemSlot.SlotReset();
            }
            else
            {
                ItemManager.Instance.selectedItemSlot.SetItem(tempData);
                ItemManager.Instance.selectedItemSlot.SetItemCount(tempCount);
            }

            ItemManager.Instance.selectedItemSlot = null;
        }
    }

    void ChangeConsumeSlot(int index)
    {
        bool isContains = ItemManager.Instance.consumeItemSlot.ContainsKey(index);

        if (isContains)
        {
            ItemManager.Instance.consumeItemSlot[index] = ItemManager.Instance.selectedItemSlot.itemData;
        }
        else
        {
            ItemManager.Instance.consumeItemSlot.Add(index, ItemManager.Instance.selectedItemSlot.itemData);
        }
    }

    public void SetConsumeSlot(ItemData itemObject)
    {
        SetItemObject(itemObject);
        SpriteChange(itemObject);
    }

    public void SetItemCount(int count)
    {
        itemCount = count;
        CountUpdate();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (itemData != null)
        {
            UIController.Instance.SetActiveMoveIcon(true, itemData.icon);
            ItemManager.Instance.selectedConsumeSlot = this;
        }
            
        UIController.Instance.PopupOff();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData != null)
            UIController.Instance.ItemInfoPopupActive(itemData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIController.Instance.PopupOff();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UIController.Instance.SetActiveMoveIcon(false);
    }

    public void SlotReset()
    {
        itemData = null;
        itemSprite.sprite = baseIcon;
        itemCount = 0;

        CountUpdate();
    }

    public void UseItem()
    {
        if (ItemManager.Instance.UseConsumeItem(itemData))
        {
            itemCount--;
            ItemManager.Instance.ReduceItem(itemData);
        }

        CountUpdate();

        if (itemCount <= 0)
            SlotReset();
    }

    void SetItemObject(ItemData itemObject)
    {
        this.itemData = itemObject;
    }

    void SpriteChange(ItemData itemObject)
    {
        itemSprite.sprite = itemObject.icon;
        this.itemData = itemObject;
    }

    void CountUpdate()
    {
        if(itemCount <= 0)
            countText.text = "";
        else
            countText.text = itemCount.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ItemManager.Instance.ContextSetActive(false);
            if (eventData.clickCount == 2)
            {
                UseItem();
            }
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ItemManager.Instance.ContextSetActive(true);
        }
    }
}
