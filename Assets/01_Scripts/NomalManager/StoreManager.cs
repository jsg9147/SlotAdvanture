using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DarkTonic.MasterAudio;

public class StoreManager : MonoBehaviour
{
    public StoreSlot storeItemPrefab;

    public Transform playerItemContent;
    public Transform storeItemContent;

    public TMP_Text storeWindowMoneyText;
    public TMP_Text playerItemCountText;
    public int minItemCount, maxItemCount;

    List<StoreSlot> storeItemList;
    List<ItemSlot> playerItemList;

    ItemSlot selectedItem;
    int playerItemCount;

    void Start()
    {
        SetPlayerItemSlot();
        SetPlayerItemList();
        SetStoreItemList();
        MoenyTextUpdate();

        MasterAudio.ChangePlaylistByName("Store");
    }

    void MoenyTextUpdate()
    {
        storeWindowMoneyText.text = ItemManager.Instance.currentMoney.ToString();
    }

    void SetPlayerItemSlot()
    {
        playerItemList = new List<ItemSlot>();
        for (int i = 0; i < 20; i++)
        {
            ItemSlot itemSlot = Instantiate(ItemManager.Instance.itemPrefab, playerItemContent);
            playerItemList.Add(itemSlot);
            itemSlot.SetStoreManager(this);
        }
    }


    void SetPlayerItemList()
    {
        List<Item> items = ItemManager.Instance.playerItems.items;
        for (int i = 0; i < items.Count; i++)
        {
            int emptyIndex = EmptySlotIndex();
            playerItemList[emptyIndex].SetItem(items[i]);
            playerItemList[emptyIndex].SetStoreManager(this);
            if (GameManager.Instance.UnitEquipList.ExsitItem(items[i].itemData))
            {
                playerItemList[emptyIndex].ReduceEquipItem(GameManager.Instance.UnitEquipList.GetItem(items[i].itemData));
            }
        }
        PlayerItemCountUpdate();
    }
    int EmptySlotIndex()
    {
        int currentIndex = 0;
        for (int i = 0; i < playerItemList.Count; i++)
        {
            if (playerItemList[i].itemData == null)
                return i;

            else
                currentIndex = i;
        }
        return currentIndex;
    }
    Dictionary<ItemData, int> SetStoreItems()
    {
        Dictionary<ItemData, int> storeItemList = new Dictionary<ItemData, int>();
        if (GameManager.Instance.storeData.Count == 0)
        {
            int randomItemCount = Random.Range(minItemCount, maxItemCount);
            int breakCount = 0;

            while (true)
            {
                ItemData randomItem = ItemManager.Instance.itemPool.GetRandomItem(RoomConcept.STORE);
                if (storeItemList.ContainsKey(randomItem))
                {
                    storeItemList[randomItem] += 1;
                }
                else
                {
                    storeItemList.Add(randomItem, 1);
                }

                if (storeItemList.Count >= randomItemCount)
                    break;

                breakCount++;

                if (breakCount > 20)
                {
                    Debug.Log("¾ß ¹º°¡ Àß¸øµÆ¾î!!");
                    break;
                }
            }

            storeItemList.Add(ItemManager.Instance.itemPool.GetHealPotion(GameManager.Instance.stageData.Stage), 10);

            GameManager.Instance.storeData = storeItemList;
        }
        else
            storeItemList = GameManager.Instance.storeData;

        return storeItemList;
    }
    void SetStoreItemList()
    {
        if (GameManager.Instance.storeData == null)
            GameManager.Instance.storeData = new Dictionary<ItemData, int>();

        storeItemList = new List<StoreSlot>();
        Dictionary<ItemData, int> storeItemObject = SetStoreItems();
        foreach(ItemData key in storeItemObject.Keys)
        {
            StoreSlot storeSlot = Instantiate(storeItemPrefab, storeItemContent);

            storeSlot.SetItem(key);

            if (key.type == ItemType.Consumable)
            {
                storeSlot.ChangeItemCount(storeItemObject[key]);
            }
            storeSlot.nameText.text = (key.GetItemName());
            storeSlot.SetStoreManager(this);

            storeItemList.Add(storeSlot);
            
            storeSlot.buyButton.onClick.AddListener(() =>
            {
                BuyItem(storeSlot);
                }
            );
        }
    }
    void BuyItem(StoreSlot storeSlot)
    {
        int storePrice = (storeSlot.itemData.price);

        if (ItemManager.Instance.currentMoney < storePrice)
        {
            print("±×Áö»õ³¢");
            return;
        }
        StoreItemCountChange(storeSlot, -1);
        ItemManager.Instance.AddStorageItem(storeSlot.itemData);
        ItemManager.Instance.MoneyChange(-storePrice);

        UIController.Instance.PopupOff();

        AddStorageItemSlot(storeSlot);

        MoenyTextUpdate();
        PlayerItemCountUpdate();

    }
    void StoreItemCountChange(StoreSlot storeSlot, int count)
    {
        if ((storeSlot.itemCount + count) <= 0)
        {
            GameManager.Instance.storeData.Remove(storeSlot.itemData);
            storeSlot.gameObject.SetActive(false);
            storeItemList.Remove(storeSlot);
            Destroy(storeSlot.gameObject);
        }
        else
        {
            GameManager.Instance.storeData[storeSlot.itemData] += count;
            storeSlot.ChangeItemCount(count);
        }
    }

    void AddStorageItemSlot(StoreSlot storeSlot)
    {
        ItemSlot playerItemSlot = playerItemList.Find(x => x.itemData == storeSlot.itemData);
        if (playerItemSlot)
        {
            playerItemSlot.SetItemCount(playerItemSlot.itemCount + 1);
        }
        else
        {
            AddPlayerInventorySlot(storeSlot.itemData);

        }
    }

    void AddPlayerInventorySlot(ItemData itemData)
    {
        ItemSlot emptySlot = playerItemList.Find(x => x.itemData == null);
        emptySlot.SetItem(itemData);
    }

    void SellItem(ItemSlot itemSlot)
    {
        try
        {
            ItemManager.Instance.MoneyChange((int)(itemSlot.itemData.price));
            ItemManager.Instance.ReduceItem(itemSlot.itemData);
            RemoveStorageItem(itemSlot);
            MoenyTextUpdate();
            PlayerItemCountUpdate();
        }
        catch (System.NullReferenceException ex)
        {
            Debug.Log(ex);
        }
    }

    void RemoveStorageItem(ItemSlot itemSlot)
    {
        itemSlot.SetItemCount(itemSlot.itemCount - 1);
        if (itemSlot.itemCount <= 0)
        {
            itemSlot.SlotReset();
        }
    }

    void PlayerItemCountUpdate()
    {
        int count = ItemManager.Instance.playerItems.items.Count;
        playerItemCountText.text = $"{count} / 20";
    }

    public void LeftClickEvent(ItemSlot itemSlot)
    {
        if(selectedItem)
            selectedItem.frame.color = Color.white;

        if (selectedItem == itemSlot)
        {
            selectedItem.frame.color = Color.white;
            selectedItem = null;
        }
        else
        {
            selectedItem = itemSlot;
            selectedItem.frame.color = new Color(1, 0.6f, 0.6f);
        }


    }
    public void RightClickEvent(ItemSlot itemSlot)
    {
        if (playerItemList.Contains(itemSlot))
        {
            SellItem(itemSlot);
        }
    }

    public void SellButton()
    {
        if (selectedItem != null)
        {
            SellItem(selectedItem);
            if (selectedItem.itemCount <= 0)
            {
                selectedItem = null;
            }
        }
        PlayerItemCountUpdate();
    }

    public void BackToMapScene()
    {
        GameManager.Instance.LoadScene("MAP");
    }
}
