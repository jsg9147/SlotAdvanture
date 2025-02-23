using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardController : MonoBehaviour
{
    public RewardPopup rewardPopupPrefab;
    public RewardPopup stageClearPopupPrefab;
    public ItemSlot itemSlotPrefab;

    public List<ItemSlot> rewardSlots;

    ItemSlot goldSlot;

    public int maxRewardCount;

    Transform rewardContent;
    List<ItemData> dropItems;

    [HideInInspector]
    public RewardPopup rewardPopup;

    private void Awake()
    {
    }

    private void Start()
    {
        Init();
        rewardPopup.WinPopupSetActive(false);
    }

    void Init()
    {
        dropItems = new List<ItemData>();

        if(GameManager.Instance.currentPlayerRoom.roomConcept == RoomConcept.BOSS)
            rewardPopup = Instantiate(stageClearPopupPrefab, GameManager.Instance.FrontCanvas.transform);
        else
            rewardPopup = Instantiate(rewardPopupPrefab, GameManager.Instance.FrontCanvas.transform);
        InitRewardSlot();
    }

    public void SetBossReward()
    {
        rewardPopup = Instantiate(stageClearPopupPrefab, GameManager.Instance.FrontCanvas.transform);
        InitRewardSlot();
    }

    public void AddBackMapSceneListener()
    {
        rewardPopup.okBtn.onClick.AddListener(() =>
        {
            BackMapScene();
        });
    }

    void InitRewardSlot()
    {
        rewardSlots.Clear();
        for (int i = 0; i < 10; i++)
        {
            ItemSlot rewardSlot = Instantiate(itemSlotPrefab, rewardPopup.GetContentTransform());
            rewardSlots.Add(rewardSlot);
            rewardSlot.gameObject.SetActive(false);
        }

        goldSlot = Instantiate(itemSlotPrefab, rewardPopup.GetContentTransform());
        goldSlot.gameObject.SetActive(false);
    }

    public void SetRewardPopupOn(int minItemCount = 0)
    {
        AddBackMapSceneListener();
        rewardPopup.WinPopupSetActive(true);
        ClearReward(minItemCount);
        SetRewardImage();
        GetGold();
    }

    void ClearReward(int minItemCount)
    {
        int rewardCount = Random.Range(minItemCount, maxRewardCount);

        if (minItemCount == 0)
        {
            bool isDrop = Random.Range(0, 10) < 8;

            if (isDrop && rewardCount > 0)
            {
                SetReward(rewardCount);
            }
        }
        else
        {
            SetReward(rewardCount);
        }
    }

    void SetReward(int rewardCount)
    {
        float randomChance = Random.Range(0, 100f);
        for (int i = 0; i < rewardCount; i++)
        {
            ItemData itemObject = ItemManager.Instance.itemPool.GetRandomItem(GameManager.Instance.currentPlayerRoom.roomConcept);
            dropItems.Add(itemObject);
            ItemManager.Instance.AddStorageItem(itemObject);
        }
        if (randomChance > 30)
        {
            ItemData itemObject = ItemManager.Instance.itemPool.GetRandomSkillBook();
            dropItems.Add(itemObject);
            ItemManager.Instance.AddStorageItem(itemObject);
        }
    }

    void GetGold()
    {
        bool isDrop = Random.Range(0, 10) < 5;

        if (dropItems.Count == 0 && isDrop)
        {
            ItemSlot rewardSlot = rewardSlots[dropItems.Count];
            int gold = Random.Range(100, 500) * Mathf.CeilToInt((GameManager.Instance.stageData.Stage + 1) * 0.5f);
            rewardSlot.SetGold(gold);
            rewardSlots.Add(rewardSlot);
            rewardSlot.gameObject.SetActive(true);
            ItemManager.Instance.MoneyChange(gold);
        }
    }

    void SetRewardImage()
    {
        for (int i = 0; i < dropItems.Count; i++)
        {
            rewardSlots[i].gameObject.SetActive(true);
            rewardSlots[i].SetRewardItem(dropItems[i]);
        }
    }

    void BackMapScene()
    {
        if (GameManager.Instance.currentPlayerRoom.roomConcept == RoomConcept.BOSS)
        {
            GameManager.Instance.StageClear();
        }
        else
        {
            GameManager.Instance.LoadScene("MAP");
        }
        UIController.Instance.PopupOff();
    }

    #region Gamble Code
    public void GambleReward(ItemData itemData)
    {
        RewardReset();
        rewardPopup.WinPopupSetActive(true);
        SetReward(itemData);
        SetRewardImage();
    }

    public void RewardReset()
    {
        for (int i = 0; i < rewardSlots.Count; i++)
        {
            rewardSlots[i].SlotReset();
            rewardSlots[i].gameObject.SetActive(false);
        }
        dropItems.Clear();
        goldSlot.gameObject.SetActive(false);
    }

    public void GoldReward(int gold)
    {
        RewardReset();
        rewardPopup.WinPopupSetActive(true);
        GambleGold(gold);
    }

    void GambleGold(int gold)
    {
        goldSlot.SetGold(gold);
        goldSlot.gameObject.SetActive(true);
        ItemManager.Instance.MoneyChange(gold);
    }

    void SetReward(ItemData itemData)
    {
        dropItems.Add(itemData);
        ItemManager.Instance.AddStorageItem(itemData);
    }

    public void Defeat()
    {
        rewardPopup.DefeatPopupSetActive(true);
    }

    public void Draw()
    {
        rewardPopup.DrawPopupSetActive(true);
    }
    #endregion
}
