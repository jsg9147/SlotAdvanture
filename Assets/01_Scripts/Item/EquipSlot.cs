using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,IPointerUpHandler, IDropHandler, IDragHandler, IPointerClickHandler
{
    public ItemType equipType;

    public Image iconSprite;
    public Sprite equipIcon;

    [HideInInspector]
    public ItemData itemObject;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemObject != null)
            UIController.Instance.ItemInfoPopupActive(itemObject);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (itemObject != null)
        {
            UIController.Instance.SetActiveMoveIcon(true, itemObject.icon);
            ItemManager.Instance.selectedEquipSlot = this;
        }
        UIController.Instance.PopupOff();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        UIController.Instance.PopupOff();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UIController.Instance.SetActiveMoveIcon(false);
    }

    private void OnDisable()
    {

    }

    public void Init()
    {
        SlotReset();
    }

    public void PutOnEquip(ItemData itemObject)
    {
        //ItemManager.Instance.EquipItemTakeOff(itemObject);
        SlotReset();
        iconSprite.sprite = itemObject.icon;
        this.itemObject = itemObject;
    }

    public void RefreshEquip(ItemData itemObject)
    {
        iconSprite.sprite = itemObject.icon;
        this.itemObject = itemObject;
        //equipItemSlot.SetWearingState(true);
    }

    //public void TakeOffEquip()
    //{
    //    if (itemObject != null)
    //    {
    //        ItemManager.Instance.EquipItemTakeOff(itemObject);
    //    }

    //    SlotReset();
    //}

    public void SlotReset()
    {
        itemObject = null;
        iconSprite.sprite = equipIcon;
    }

    void PutOnEquipFromDrag()
    {
        if (ItemManager.Instance.selectedItemSlot)
        {
            ItemManager.Instance.EquipChange(ItemManager.Instance.selectedItemSlot);
            ItemManager.Instance.SelectedSlotInit();
        }
    }


    public void OnDrop(PointerEventData eventData)
    {
        PutOnEquipFromDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 없으면 드롭이 동작 안함
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ItemManager.Instance.ContextSetActive(false);
            if (eventData.clickCount == 2)
            {
                if(itemObject != null)
                    ItemManager.Instance.EquipItemTakeOff(itemObject.type);
                SlotReset();
            }
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ItemManager.Instance.ContextSetActive(true);
        }
    }
}
