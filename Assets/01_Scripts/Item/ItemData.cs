using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptable Object/Item Data", order = int.MaxValue)]
public class ItemData : ScriptableObject
{
    [SerializeField] private string codeName;
    public string Code
    {
        get { return codeName; }
    }

    public int itemGrade;
    public List<int> stageGrade;

    public ItemType type;
    public int price;
    
    public Sprite icon;
    public ItemEffect[] effects;

    public SkillObject skillObject;

    public string GetItemName()
    {
        return LocalizationManager.Instance.GetItemLocalizingName(codeName);
    }
    public string GetItemDescription()
    {
        return LocalizationManager.Instance.GetItemLocalizingDescription(codeName);
    }

    public int StageItemGrade(int stage)
    {
        if (stageGrade.Count > stage)
        {
            return stageGrade[stage];
        }

        return -1;
    }

    public void SetSkillBook()
    {
        try
        {
            type = ItemType.SkillBook;
        }
        catch(System.NullReferenceException ex)
        {
            if(skillObject != null)
                Debug.Log($"{skillObject.name}\n{ex}");
        }
    }

    public int GetTotalStat(Stat stat)
    {
        if (effects != null)
        {
            int total = 0;
            for (int i = 0; i < effects.Length; i++)
            {
                total = +GetStat(effects[i], stat);
            }

            return total;
        }
        return 0;   
    }

    int GetStat(ItemEffect itemEffect, Stat stat)
    {
        switch (stat)
        {
            case Stat.HP:
                return itemEffect.hp;
            case Stat.AD:
                return itemEffect.ad;
            case Stat.AP:
                return itemEffect.ap;
            case Stat.DEF:
                return itemEffect.def;
            case Stat.MR:
                return itemEffect.mr;
            case Stat.SPD:
                return itemEffect.spd;
            case Stat.ACC:
                return itemEffect.acc;

            default:
                return 0;
        }
    }

    public bool Consumable()
    {
        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i].isPotionEffect)
                return true;
        }

        return false;
    }

    public bool Contains(string effectName)
    {
        foreach (ItemEffect itemEffect in effects)
        {
            if (itemEffect.name == effectName)
                return true;
        }
        return false;
    }
}

[System.Serializable]
public class ItemEffect
{
    public string name;
    public int hp;
    public int ad;
    public int ap;
    public int def;
    public int mr;
    public int spd;
    public int acc;

    public int duration;
    public bool isPotionEffect;

    public int GetStat(Stat stat)
    {
        switch (stat)
        {
            case Stat.HP:
                return hp;
            case Stat.AD:
                return ad;
            case Stat.AP:
                return ap;
            case Stat.DEF:
                return def;
            case Stat.MR:
                return mr;
            case Stat.SPD:
                return spd;
            case Stat.ACC:
                return acc;
        }

        return 0;
    }
}

public enum ItemTag
{
    Unique, // 한개 먹으면 더이상 드롭되지 않는 아이템
    Set
}
public enum ItemType
{
    Weapon,
    Armor,
    Rune,
    Jewelry,
    Consumable,
    SkillBook,
    Revive
}

public enum Stat
{
    HP,
    AD,
    AP,
    DEF,
    MR,
    SPD,
    ACC,
    NoneType
}

public class Item
{
    public ItemData itemData;
    public int itemCount;

    public Item()
    {

    }
    public Item(ItemData itemData)
    {
        this.itemData = itemData;
        itemCount = 1;
    }
}

public class ItemList
{
    public ItemList()
    {
        items = new List<Item>();
    }

    public List<Item> items;

    public Item GetItem(ItemData itemData) => items.Find(x => x.itemData == itemData);
    public int GetItemCount(ItemData itemData)
    {
        try
        {
            if (itemData != null)
            {
                Item target = GetItem(itemData);
                if (target != null)
                    return GetItem(itemData).itemCount;
                else
                    return 0;
            }
            else
            {
                return 0;
            }
        }
        catch (System.NullReferenceException ex)
        {
            Debug.Log(ex);
            return 0;
        }
    }

    public void ReduceItem(ItemData itemData)
    {
        try
        {
            Item targetItem = GetItem(itemData);
            
            if (targetItem != null)
            {
                targetItem.itemCount--;
                if (targetItem.itemCount <= 0)
                    items.Remove(targetItem);
            }
        }
        catch (System.NullReferenceException ex)
        {
            Debug.Log(ex);
        }
    }

    public void SetItem(ItemData itemData)
    {
        try
        {
            Item targetItem = GetItem(itemData);
            if (targetItem == null)
            {
                Item newItem = new(itemData);
                items.Add(newItem);
            }
            else
            {
                targetItem.itemCount++;
            }
        }
        catch (System.NullReferenceException ex)
        {
            Debug.Log(ex);
        }
    }

    public void DiscardItem(ItemData itemData)
    {
        try
        {
            Item targetItem = GetItem(itemData);
            if (targetItem != null)
            {
                items.Remove(targetItem);
            }
        }
        catch (System.NullReferenceException ex)
        {
            Debug.Log(ex);
        }
    }

    public bool ExsitItem(ItemData itemData)
    {
        return items.Find(x => x.itemData == itemData) != null;
    }

    public int TotalItemCount()
    {
        int count = 0;
        for (int i = 0; i < items.Count; i++)
        {
            count += items[i].itemCount;
        }

        return count;
    }
}