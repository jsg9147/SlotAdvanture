using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryWindow : MonoBehaviour , IPointerClickHandler
{   
    public TMP_Text[] heroSlotText;
    public Transform itemSlotContent;

    public RectTransform heroPosition;

    public EquipmentManager equipmentSlot;

    public TMP_Text inventoryCountText;

    public InventorySlotButton[] slotButtons;
    public ItemSlot itemSlotPrefab;
    public List<SkillSlot> skillSlots;

    public List<ConsumeSlot> consumeSlots;

    public StatUI statUI;

    public TMP_Text goldText;
    public ProgressBarPro hpBar;

    List<ItemSlot> itemSlots;
    int currentItemCount
    {
        get 
        { 
            int count = 0;
            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (itemSlots[i].itemData != null)
                    count++;
            }

            return count;
        }
    }

    void Start()
    {
        for (int i = 0; i < consumeSlots.Count; i++)
        {
            consumeSlots[i].index = i;
        }
    }
    public void InitInventory()
    {
        itemSlots = new List<ItemSlot>();
        for (int i = 0; i < 20; i++)
        {
            ItemSlot itemSlot = Instantiate(itemSlotPrefab, itemSlotContent);
            itemSlot.SlotReset();
            itemSlots.Add(itemSlot);
        }

        goldText.text = ItemManager.Instance.currentMoney.ToString();
    }

    public void SetUnitInfo(UnitData unitData)
    {
        statUI.SetInventoryStat(unitData);

        skillSlots[0].SetUnitData(unitData, 0);
        skillSlots[1].SetUnitData(unitData, 1);
    }

    public void ItemSlotsInit(List<Item> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            int emptyIndex = EmptySlotIndex();
            itemSlots[emptyIndex].SetItem(items[i]);

            if (GameManager.Instance.UnitEquipList.ExsitItem(items[i].itemData))
            {
                itemSlots[emptyIndex].ReduceEquipItem(GameManager.Instance.UnitEquipList.GetItem(items[i].itemData));
            }
        }

        ItemCountUpdate();
        goldText.text = ItemManager.Instance.currentMoney.ToString();
    }

    public void SetItemSlot(ItemData itemObject)
    {
        ItemSlot itemSlot = itemSlots.Find(x => x.itemData == itemObject);
        if (itemSlot)
        {
            itemSlot.AddItemCount();
        }
        else
        {
            itemSlots[EmptySlotIndex()].SetItem(itemObject);
        }

        ItemCountUpdate();
        goldText.text = ItemManager.Instance.currentMoney.ToString();
    }

    public void SetConsumeSlot(int slotIndex, ItemData itemData)
    {
        try
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (itemSlots[i].itemData == itemData)
                {
                    if (consumeSlots[slotIndex].itemData != null)
                    {
                        itemSlots[i].SetItem(consumeSlots[slotIndex].itemData);
                        itemSlots[i].SetItemCount(consumeSlots[slotIndex].itemCount);
                    }
                    consumeSlots[slotIndex].SetConsumeSlot(itemData);
                    consumeSlots[slotIndex].SetItemCount(itemSlots[i].itemCount);
                    itemSlots[i].SlotReset();

                    slotIndex++;
                    if (slotIndex >= consumeSlots.Count)
                        break;
                }
            }
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    public void ResetConsumeSlot()
    {
        for (int i = 0; i < consumeSlots.Count; i++)
        {
            consumeSlots[i].SlotReset();
        }
    }

    public void UseConsume(ItemData itemObject)
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].itemData == itemObject)
            {
                itemSlots[i].MinusItem();
            }
        }
    }

    int EmptySlotIndex()
    {
        int currentIndex = 0;
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (itemSlots[i].itemData == null)
                return i;

            else
                currentIndex = i;
        }
        return currentIndex;
    }

    public void ItemCountUpdate()
    {
        inventoryCountText.text = currentItemCount + " / " + 20;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ItemManager.Instance.ContextSetActive(false);
    }
}
