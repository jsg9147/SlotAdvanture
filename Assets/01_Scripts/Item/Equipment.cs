using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment
{
    private ItemData _weapon;
    private ItemData _armor;
    private ItemData _jewelry;
    private ItemData _rune;

    public Equipment()
    {
        _weapon = null;
        _armor = null;
        _jewelry = null;
        _rune = null;
    }

    public List<ItemData> equipList
    {
        get { return new List<ItemData> { _weapon, _armor, _jewelry, _rune }; }
    }

    public ItemData GetEquipParts(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Weapon:
                return _weapon;
            case ItemType.Armor:
                return _armor;
            case ItemType.Jewelry:
                return _jewelry;
            case ItemType.Rune:
                return _rune;
            default:
                Debug.Log("¿À·ù");
                return null;
        }
    }

    public float TotalHP
    {
        get { return EquipStat(Stat.HP); }
    }

    public float TotalAD
    {
        get { return EquipStat(Stat.AD); }
    }

    public float TotalAP
    {
        get { return EquipStat(Stat.AP); }
    }

    public float TotalDEF
    {
        get { return EquipStat(Stat.DEF); }
    }

    public float TotalMR
    {
        get { return EquipStat(Stat.MR); }
    }

    public float TotalSpeed
    {
        get { return EquipStat(Stat.SPD); }
    }

    public float TotalAccuracy
    {
        get { return EquipStat(Stat.ACC); }
    }

    public void PutOnEquip(ItemData itemObject)
    {
        switch(itemObject.type)
        {
            case ItemType.Weapon:
                _weapon = itemObject;
                break;
            case ItemType.Armor:
                _armor = itemObject;
                break;
            case ItemType.Rune:
                _rune = itemObject;
                break;
            case ItemType.Jewelry:
                _jewelry = itemObject;
                break;
        }
    }

    public void TakeOffEquip(ItemData itemObject)
    {
        switch (itemObject.type)
        {
            case ItemType.Weapon:
                _weapon = null;
                break;
            case ItemType.Armor:
                _armor = null;
                break;
            case ItemType.Rune:
                _rune = null;
                break;
            case ItemType.Jewelry:
                _jewelry = null;
                break;
        }
    }

    public bool EquipCheck(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.Weapon:
                return _weapon != null;
            case ItemType.Armor:
                return _armor != null;
            case ItemType.Rune:
                return _rune != null;
            case ItemType.Jewelry:
                return _jewelry != null;
        }

        return false;
    }

    public int EquipStat(Stat stat)
    {
        int value = 0;

        for (int i = 0; i < equipList.Count; i++)
        {
            if (equipList[i] != null)
            {
                value += equipList[i].GetTotalStat(stat);
            }
        }

        return value;
    }
}