using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    public bool testMode;
    public List<ItemData> testItem;

    [SerializeField] private List<ItemData> _bossItemList;
    [SerializeField] private List<ItemData> _treasureItemList;
    [SerializeField] private List<ItemData> _storeItemList;
    [SerializeField] private List<ItemData> _gambleItemList;
    [SerializeField] private List<ItemData> _nomalItemList;
    [SerializeField] private List<ItemData> _consumeItemList;
    [SerializeField] private List<ItemData> _healItemList;

    [SerializeField] private ItemData _skillBook;
    [SerializeField] private List<SkillObject> _skillList;


    public List<ItemData> ConsumeList
    {
        get { return _consumeItemList; }
    }

    [SerializeField]
    private float[] dropChance; // 노말 레어 유니크 레전드 , 세트는 나중에

    private float totalDropChance
    {
        get
        {
            float total = 0f;
            for (int i = 0; i < dropChance.Length; i++)
            {
                total += dropChance[i];
            }

            return total;
        }
    }

    public ItemData GetRandomItem(RoomConcept roomConcept)
    {
        List<ItemData> itemList = GetItemPool(roomConcept);

        int grade = GetGrade(Random.Range(0, totalDropChance));
        int stage = GameManager.Instance.stageData.Stage;

        ItemData randomItem;
        if (itemList.FindAll(x => x.StageItemGrade(stage) == grade).Count <= 0)
        {
            grade = 0;
        }
        List<ItemData> gradeItem = new List<ItemData>();
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].StageItemGrade(stage) == grade)
            {
                gradeItem.Add(itemList[i]);
            }
        }

        try
        {
            int randomItemIndex = Random.Range(0, gradeItem.Count);
            randomItem = gradeItem[randomItemIndex];
            return randomItem;
        }
        catch (System.ArgumentException ex)
        {
            print($"스테이지 ({GameManager.Instance.stageData.Stage}) : 방 컨셉 ({roomConcept}) : 등급 ({grade}) : 갯수 ({gradeItem.Count})\n{ex}");

            int randomItemIndex = Random.Range(0, itemList.Count);
            randomItem = itemList[randomItemIndex];
            return randomItem;
        }
    }

    public ItemData GetRandomSkillBook()
    {
        int grade = GetGrade(Random.Range(0, totalDropChance));

        List<SkillObject> targetSkillList = new List<SkillObject>();
        for (int i = 0; i < _skillList.Count; i++)
        {
            if (_skillList[i].grade == grade)
            {
                targetSkillList.Add(_skillList[i]);
            }
        }
        try
        {
            int randomItemIndex = Random.Range(0, targetSkillList.Count);
            ItemData skillBook = Instantiate(_skillBook);
            skillBook.SetSkillBook();
            skillBook.skillObject = targetSkillList[randomItemIndex];
            return skillBook;

        }
        catch (System.ArgumentException ex)
        {
            print($"등급 ({grade}) : 스킬 개수 {targetSkillList.Count}\n{ex}");
        }

        print($"무언가 에러가 있음");
        ItemData tempBook = Instantiate(_skillBook);
        tempBook.skillObject = _skillList[0];
        return tempBook;
    }

    int GetGrade(float chance)
    {
        int grade = 0;
        float addChance = 0f;
        for (int i = 0; i < dropChance.Length; i++)
        {
            addChance += dropChance[i];

            if (chance < addChance)
            {
                grade = i;
                break;
            }
        }

        return grade;
    }

    List<ItemData> GetItemPool(RoomConcept roomConcept)
    {
        List<ItemData> targetItems = new List<ItemData>();
        switch (roomConcept)
        {
            case RoomConcept.TREASURE:
                targetItems.AddRange(_treasureItemList);
                break;
            case RoomConcept.BOSS:
                targetItems.AddRange(_bossItemList);
                break;
            case RoomConcept.STORE:
                targetItems.AddRange(_storeItemList);
                break;
            case RoomConcept.GAMBLE:
                targetItems.AddRange(_gambleItemList);
                break;
            case RoomConcept.NORMAL:
                targetItems.AddRange(_nomalItemList);
                break;
            default:
                targetItems.AddRange(_nomalItemList);
                break;
        }
        targetItems.RemoveAll(x => x == null);
        targetItems.RemoveAll(x => x.StageItemGrade(GameManager.Instance.stageData.Stage) == -1);
        return targetItems;
    }

    public ItemData GetHealPotion(int stage)
    {
        try
        {
            int potionIndex = Mathf.FloorToInt(stage * 0.5f);

            if (stage == 3)
                potionIndex = _healItemList.Count - 1;

            ItemData consumeItem = _healItemList[potionIndex];
            return consumeItem;
        }
        catch(System.IndexOutOfRangeException ex)
        {
            print(ex);
            return _healItemList[_healItemList.Count - 1];
        }
    }
}

[System.Serializable]
public class RootObject
{
    public List<ItemData> items;
}