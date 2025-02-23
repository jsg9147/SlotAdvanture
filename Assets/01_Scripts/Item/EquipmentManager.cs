using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentManager : MonoBehaviour
{
    public EquipSlot weaponSlot;
    public EquipSlot armorSlot;
    public EquipSlot jewelrySlot;
    public EquipSlot stoneSlot;
    

    EquipSlot FindEquipSlot(ItemType itemType)
    {
        EquipSlot equipSlot = null;
        if (itemType == ItemType.Weapon)
        {
            equipSlot = weaponSlot;
        }
        if (itemType == ItemType.Armor)
        {
            equipSlot = armorSlot;
        }
        if (itemType == ItemType.Rune)
        {
            equipSlot = stoneSlot;
        }
        if (itemType == ItemType.Jewelry)
        {
            equipSlot = jewelrySlot;
        }

        return equipSlot;
    }


    public void Init()
    {
        weaponSlot.Init();
        armorSlot.Init();
        jewelrySlot.Init();
        stoneSlot.Init();
    }

    public void SlotRefresh(List<ItemData> equipList)
    {
        Init();

        for (int i = 0; i < equipList.Count; i++)
        {
            if(equipList[i] != null)
                FindEquipSlot(equipList[i].type).RefreshEquip(equipList[i]);
        }
    }

    //아이템창에서 클릭하면 동작
    public void EquipSlotChange(ItemSlot equipItemSlot)
    {
        FindEquipSlot(equipItemSlot.itemData.type).PutOnEquip(equipItemSlot.itemData);
    }
}
