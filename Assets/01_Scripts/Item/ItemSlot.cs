using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class ItemFrameSprites
{
    public Sprite weaponFrameSprite;
    public Sprite armorFrameSprite;
    public Sprite jewelryFrameSprite;
    public Sprite runeFrameSprite;
    public Sprite consumeFrameSprite;
    public Sprite nullFrameSprite;
}

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDropHandler, IDragHandler, IPointerClickHandler
{
    [SerializeField] ItemFrameSprites itemFrameSprites;

    public Image frame;
    public Image itemIcon;
    public GameObject itemCountGroup;
    public TMP_Text countText;

    [HideInInspector]
    public ItemData itemData;

    StoreManager storeManager;

    public int itemCount;

    bool canUse = false;

    #region Mounse Handler
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData != null)
        {
            UIController.Instance.ItemInfoPopupActive(itemData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIController.Instance.PopupOff();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        UIController.Instance.PopupOff();

        if (itemData != null)
        {
            if (storeManager)
            {
                StoreItemClickEvent(eventData);
            }
            else
            {
                if (canUse)
                {
                    UIController.Instance.SetActiveMoveIcon(true, itemData.icon);
                    ItemManager.Instance.selectedItemSlot = this;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        UIController.Instance.SetActiveMoveIcon(false);
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        MoveItemSlot();
        TakeOffEquipForDrag();
        MoveConsumeItem();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그가 있어야 드롭이 동작함
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!canUse || itemData == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ItemManager.Instance.ContextSetActive(false);
            if (eventData.clickCount == 2)
            {
                UseItem();
                eventData.clickCount = 0;
            }
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (storeManager == null)
                ItemManager.Instance.ContextSetActive(true);
        }

        UIController.Instance.PopupOff();
    }
    #endregion

    public void SetStoreManager(StoreManager storeManager)
    {
        this.storeManager = storeManager;
        canUse = false;
        //isStoreSlot = true;
    }

    public void SetItem(Item item)
    {
        canUse = true;
        this.itemData = item.itemData;
        SetSlotFrame(itemData.type);
        itemIcon.sprite = itemData.icon;
        itemIcon.gameObject.SetActive(true);

        SetItemCount(item.itemCount);
    }

    public void ReduceEquipItem(Item item)
    {
        itemCount = itemCount - item.itemCount;
        SetItemCount(itemCount);
    }

    public void SetItem(ItemData itemData)
    {
        canUse = true;
        this.itemData = itemData;
        SetSlotFrame(itemData.type);
        itemIcon.sprite = itemData.icon;
        itemIcon.gameObject.SetActive(true);

        itemCount = 1;
    }

    public void SetBattle() => canUse = false;

    void SetSlotFrame(ItemType itemType)
    {
        switch(itemType)
        {
            case ItemType.Weapon:
                frame.sprite = itemFrameSprites.weaponFrameSprite;
                break;
            case ItemType.Armor:
                frame.sprite = itemFrameSprites.armorFrameSprite;
                break;
            case ItemType.Jewelry:
                frame.sprite = itemFrameSprites.jewelryFrameSprite;
                break;
            case ItemType.Rune:
                frame.sprite = itemFrameSprites.runeFrameSprite;
                break;
            case ItemType.Consumable:
                frame.sprite = itemFrameSprites.consumeFrameSprite;
                break;
            default:
                frame.sprite = itemFrameSprites.nullFrameSprite;
                break;
        }
    }

    public void SetRewardItem(ItemData itemData)
    {
        SetItem(itemData);
        canUse = false;
    }

    public void SetGold(int gold)
    {
        itemIcon.sprite = GameManager.Instance.goldSprite;
        countText.text = gold.ToString();
        canUse = false;
    }

    public void SlotReset()
    {
        itemData = null;
        itemCount = 0;
        itemIcon.gameObject.SetActive(false);
        countText.text = "";
        canUse = false;

        if(itemCountGroup)
            itemCountGroup.SetActive(false);

        frame.sprite = itemFrameSprites.nullFrameSprite;
    }
    public void SetItemCount(int count)
    {
        itemCount = count;
        countText.text = count.ToString();

        if (itemCountGroup)
        {
            if (!itemCountGroup.activeSelf)
                itemCountGroup.SetActive(true);
            if (count <= 1)
                itemCountGroup.SetActive(false);
        }

        if (itemCount == 0)
            SlotReset();
    }

    public void AddItemCount()
    {
        itemCount++;
        countText.text = itemCount.ToString();
        itemCountGroup.SetActive(true);
    }

    public void MinusItem()
    {
        itemCount--;

        if (itemCount > 1)
            countText.text = itemCount.ToString();
        else if (itemCount == 1)
            countText.text = "";
        
        if (itemCount <= 0)
            SlotReset();
    }

    public void UseItem()
    {
        if (!canUse || itemData == null)
            return;

        switch (itemData.type)
        {
            case ItemType.Consumable:
                UsePotion();
                break;
            case ItemType.SkillBook:
                UseSkillBook();
                break;
            case ItemType.Revive:
                ReviveItem();
                break;  
            default:
                EquipChange();
                break;
        }
    }

    void UsePotion()
    {
        if (ItemManager.Instance.UseConsumeItem(itemData))
        {
            ItemManager.Instance.ReduceItem(itemData);
            MinusItem();
        }

        CountUpdate();

        UIController.Instance.PopupOff();
    }

    void UseSkillBook()
    {
        SkillManager.instance.SkillBookUsed(true);
        SkillManager.instance.bookSkill = itemData.skillObject;

        ItemManager.Instance.OpenInventory();

        ItemManager.Instance.ReduceItem(itemData);
        MinusItem();
    }

    void ReviveItem()
    {
        if (ItemManager.Instance.activeHeroData.currentHP > 0)
        {
            return;
        }

        ItemManager.Instance.ReduceItem(itemData);
        SlotMachineManager.Instance.SlotStart(Belong.Player);

        MinusItem();

        UIController.Instance.PopupOff();
        StartCoroutine(RevieEffect());
    }

    IEnumerator RevieEffect()
    {
        yield return new WaitForSeconds(3f);
        ItemManager.Instance.ReviveItem(itemData);
    }

    void EquipChange()
    {
        ItemManager.Instance.EquipChange(this);
        ItemManager.Instance.selectedItemSlot = null;
        UIController.Instance.PopupOff();
    }

    // 착용중 장비창은 경고문 띄우기
    void StoreItemClickEvent(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            storeManager.LeftClickEvent(this);
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            storeManager.RightClickEvent(this);
        }
    }

    void MoveItemSlot()
    {
        if (ItemManager.Instance.selectedItemSlot != null)
        {
            ItemData tempData = itemData;
            int tempCount = itemCount;

            SetItem(ItemManager.Instance.selectedItemSlot.itemData);
            SetItemCount(ItemManager.Instance.selectedItemSlot.itemCount);

            ItemManager.Instance.selectedItemSlot.SlotReset();

            if (tempData != null)
            {
                ItemManager.Instance.selectedItemSlot.SetItem(tempData);
                ItemManager.Instance.selectedItemSlot.SetItemCount(tempCount);
            }

            ItemManager.Instance.selectedItemSlot = null;
        }
    }

    void MoveConsumeItem()
    {
        if (itemData != null)
        {
            if (itemData.type != ItemType.Consumable)
                return;
        }

        if (ItemManager.Instance.selectedConsumeSlot)
        {
            ItemData tempData = ItemManager.Instance.selectedConsumeSlot.itemData;
            int tempCount = ItemManager.Instance.selectedConsumeSlot.itemCount;

            if (itemData == null)
            {
                ItemManager.Instance.consumeItemSlot.Remove(ItemManager.Instance.selectedConsumeSlot.index);
                ItemManager.Instance.selectedConsumeSlot.SlotReset();
                ItemManager.Instance.selectedConsumeSlot = null;
            }
            else
            {
                ItemManager.Instance.selectedConsumeSlot.SetConsumeSlot(itemData);
                ItemManager.Instance.selectedConsumeSlot.SetItemCount(itemCount);
            }
            SetItem(tempData);
            SetItemCount(tempCount);
        }
    }

    void TakeOffEquipForDrag()
    {
        if (ItemManager.Instance.selectedEquipSlot)
        {
            // TODO : 탈착시 위치가 그냥 배열순으로 갈꺼임
            //SetItem(ItemManager.Instance.selectedEquipSlot.itemObject);
            print("탈착시 위치가 그냥 배열순으로 갈꺼임");
            //ItemManager.Instance.selectedEquipSlot.TakeOffEquip();
            try
            {
                ItemManager.Instance.EquipItemTakeOff(ItemManager.Instance.selectedEquipSlot.itemObject.type);
                ItemManager.Instance.selectedEquipSlot.SlotReset();
                ItemManager.Instance.selectedEquipSlot = null;
            }
            catch (System.NullReferenceException ex)
            {
                Debug.Log($"{ItemManager.Instance.selectedEquipSlot.itemObject.type}\n{ex}");
            }
        }
    }

    void CountUpdate()
    {
        if (itemCount <= 0)
            countText.text = "";
        else
        {
            countText.text = itemCount.ToString();
            itemCountGroup.SetActive(true);
        }
    }

    public void SetGoldIcon(int gold)
    {
        itemIcon.sprite = GameManager.Instance.goldSprite;
        countText.text = gold.ToString();
    }
}