using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ItemManager : MonoBehaviour
{
    private static ItemManager instance;
    public static ItemManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    public RoomConcept testConcept;

    [SerializeField] private InventoryWindow inventoryWindowPrefab;

    private InventoryWindow inventoryWindow;

    private EquipmentManager equipmentManager;

    private List<Unit> herosPreview;

    //public Canvas frontCanvas;

    public InventoryContext inventoryContextPrefab;
    InventoryContext inventoryContext;

    private TMP_Text[] heroSlotText;

    public ItemSlot itemPrefab;

    public Vector3 heroScale;

    public UnitData activeHeroData;

    [HideInInspector]
    public ItemPool itemPool;
    [HideInInspector]
    public Canvas canvas;

    [HideInInspector]
    public int currentMoney;
    [HideInInspector]
    public ItemSlot selectedItemSlot;
    [HideInInspector]
    public EquipSlot selectedEquipSlot;
    [HideInInspector]
    public ConsumeSlot selectedConsumeSlot;

    
    [HideInInspector]
    public Dictionary<int, ItemData> consumeItemSlot; //TODO : 기다리셈

    [HideInInspector]
    public ResultPopup resultPopupPrefab;
    ResultPopup resultPopup;
    
    public ItemData selectedItemObject;

    //[HideInInspector]
    //public List<ItemData> haveItemList;

    public ItemList playerItems;

    public bool InventoryActiveSelf
    {
        get { return inventoryWindow.gameObject.activeSelf; }
    }
    public int HaveItemCount(ItemData itemData)
    {
        return playerItems.GetItemCount(itemData);
    }


    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        //haveItemList = new List<ItemData>();
        playerItems = new ItemList();
        consumeItemSlot = new Dictionary<int, ItemData>();
    }

    private void Update()
    {
        InputInventoryKey();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddRandomItem(1);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SkillBookTest();
        }
#endif
    }
    public void SetCanvas(Canvas canvas) => this.canvas = canvas;

    void InputInventoryKey()
    {
        if (GameManager.Instance.isGameStart)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                OpenInventory();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                inventoryWindow.gameObject.SetActive(false);
                ContextSetActive(false);
            }
        }
        
    }


    public void OpenInventory()
    {
        inventoryWindow.gameObject.SetActive(!inventoryWindow.gameObject.activeSelf);

        ContextSetActive(false);

        if (activeHeroData == null)
        {
            EquipSlotHeroChange(0);
        }

        SetHeroNameText(); // 이후에 변경 필요
    }


    public void Init()
    {
        //canvas = Instantiate(frontCanvas);
        canvas.worldCamera = Camera.main;
        canvas.sortingLayerName = "Foreground";
        itemPool = GetComponent<ItemPool>();
        herosPreview = new List<Unit>();
        if (GameManager.Instance.isGameStart)
        {
            resultPopup = Instantiate(resultPopupPrefab, canvas.transform);

            SetInventoryWindow();
            SetInventoryContext();
            ItemSlotInit();
            ConsumeSlotInit();

            equipmentManager.Init();

            SetInventoryUnitInstantiate();
            AddListenerSlotChange();
            EquipSlotHeroChange(0);
        }
    }
    void SetInventoryContext()
    {
        if (inventoryContext == null)
            inventoryContext = Instantiate(inventoryContextPrefab, UIController.Instance.transform);

        inventoryContext.gameObject.SetActive(false);

        inventoryWindow.gameObject.SetActive(false);
    }

    void SetInventoryUnitInstantiate()
    {
        if (GameManager.Instance.isGameStart)
        {
            for (int i = 0; i < GameManager.Instance.playerUnitDatas.Length; i++)
            {
                Unit heroUnit = Instantiate(PrefabManager.Instance.GetUnitPrefab(GameManager.Instance.playerUnitDatas[i]));
                heroUnit.transform.SetParent(inventoryWindow.heroPosition);
                heroUnit.Init(GameManager.Instance.playerUnitDatas[i]);
                heroUnit.transform.localScale = heroScale;
                heroUnit.transform.localPosition = new Vector3(0, -70f, -10f);

                if (heroUnit.unitData.sanctuarySkill != null)
                {
                    GameObject aura = EffectManager.Instance.InstantiateAura(heroUnit.transform, heroUnit.unitData.sanctuarySkill);
                    aura.GetComponent<ParticleSystemRenderer>().sortingOrder = 99;
                }
                heroUnit.LayerChange("Foreground");

                heroUnit.gameObject.SetActive(false);
                herosPreview.Add(heroUnit);
            }
        }
    }

    void AddListenerSlotChange()
    {
        for (int i = 0; i < inventoryWindow.slotButtons.Length; i++)
        {
            int index = i;
            inventoryWindow.slotButtons[i].button.onClick.AddListener(() =>
            {
                EquipSlotHeroChange(index);
                SlotActive(index);
            });
        }
    }

    void SlotActive(int index)
    {
        for (int i = 0; i < inventoryWindow.slotButtons.Length; i++)
        {
            inventoryWindow.slotButtons[i].SetActiveBtn(index == i);
        }
    }

    // save 에서 불러오는 코드 짜야함
    public void SetCurrnetMoney()
    {
        currentMoney = GameManager.Instance.startMoney;
    }

    public void MoneyChange(int changeAmount)
    {
        currentMoney += changeAmount;
    }
    
    void SetInventoryWindow()
    {
        if (inventoryWindow == null)
        {
            inventoryWindow = Instantiate(inventoryWindowPrefab, canvas.transform);
            inventoryWindow.InitInventory();
            inventoryWindow.ResetConsumeSlot();
            heroSlotText = inventoryWindow.heroSlotText;
            SetFont();

            equipmentManager = inventoryWindow.equipmentSlot;

            inventoryWindow.transform.SetAsLastSibling();
        }
    }

    void SetFont()
    {
        for (int i = 0; i < heroSlotText.Length; i++)
        {
            heroSlotText[i].font = LocalizationManager.Instance.GetFont();
        }
    }

    public void EquipItemTakeOff(ItemType itemType)
    {
        ItemData takeOffItemData = activeHeroData.equipment.GetEquipParts(itemType);
        if (takeOffItemData != null)
        {
            activeHeroData.EquipTakeOff(takeOffItemData);
            inventoryWindow.SetItemSlot(takeOffItemData);
            inventoryWindow.SetUnitInfo(activeHeroData);
        }
        SetHPBar(true);
    }


    void ItemSlotInit()
    {
        inventoryWindow.ItemSlotsInit(playerItems.items);
    }

    void ConsumeSlotInit()
    {
        foreach (int slotIndex in consumeItemSlot.Keys)
        {
            if (consumeItemSlot[slotIndex] != null)
            {
                inventoryWindow.SetConsumeSlot(slotIndex, consumeItemSlot[slotIndex]);
            }
        }
    }

    void SetHeroNameText()
    {
        for (int i = 0; i < heroSlotText.Length; i++)
        {
            heroSlotText[i].text = GameManager.Instance.playerUnitDatas[i].UnitName;
        }
    }

    public ItemData GetConsumeItem(int index)
    {
        ItemData itemObject = null;
        try
        {
            if (consumeItemSlot.ContainsKey(index))
            {
                itemObject = consumeItemSlot[index];

            }
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);
        }
        return itemObject;
    }

    public void AddRandomItem(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddStorageItem(itemPool.GetRandomItem(testConcept));
        }
    }

    void SkillBookTest()
    {
        AddStorageItem(itemPool.GetRandomSkillBook());
    }

    public void AddStorageItem(ItemData item)
    {
        playerItems.SetItem(item);
        inventoryWindow.SetItemSlot(item);
    }

    public void ReduceItem(ItemData itemData)
    {
        playerItems.ReduceItem(itemData);
        inventoryWindow.ItemCountUpdate();
    }

    public void EquipSlotHeroChange(int index)
    {
        if (GameManager.Instance.playerUnitDatas == null)
            return;
        activeHeroData = GameManager.Instance.SelectPlayerUnit(index);
        inventoryWindow.SetUnitInfo(GameManager.Instance.playerUnitDatas[index]);
        for (int i = 0; i< herosPreview.Count; i++)
        {
            herosPreview[i].gameObject.SetActive((i == index));
            if(!herosPreview[i].IsAlive)
                herosPreview[i].PlayAnimation("isDie", !herosPreview[i].IsAlive);
        }
        equipmentManager.SlotRefresh(activeHeroData.equipment.equipList);
        SetHPBar(true);
    }

    void SetHPBar(bool skipAnimation = false)
    {
        inventoryWindow.hpBar.barViewValueText.maxValue = activeHeroData.stat.HP;
        inventoryWindow.hpBar.SetValue(activeHeroData.currentHP+1, activeHeroData.stat.HP, skipAnimation);
        inventoryWindow.hpBar.SetValue(activeHeroData.currentHP, activeHeroData.stat.HP, skipAnimation);
    }

    //TODO : 코드중복, 분할 필요
    public void EquipChange(ItemSlot itemSlot)
    {
        if (activeHeroData.EquipCheck(itemSlot.itemData.type))
        {
            EquipItemTakeOff(itemSlot.itemData.type);
        }
        equipmentManager.EquipSlotChange(itemSlot); // 장비창에서 교체
        activeHeroData.EquipChange(itemSlot.itemData); // 캐릭터의 실짋적 교체
        inventoryWindow.SetUnitInfo(activeHeroData);

        itemSlot.MinusItem();
        inventoryWindow.ItemCountUpdate();

        inventoryWindow.SetUnitInfo(activeHeroData);
        SetHPBar(true);

        if (GameManager.Instance.UnitEquipList.TotalItemCount() >= 12)
            SteamAchievements.Instance.HandleAllEquippedAchievement();
    }

    public void SelectedSlotInit()
    {
        selectedItemSlot.SlotReset();

        selectedEquipSlot = null;
        selectedItemSlot = null;
    }

    public bool UseConsumeItem(ItemData itemData)
    {
        if (itemData.type != ItemType.Consumable)
            return false;

        if (itemData.GetTotalStat(Stat.HP) > 0)
            return HealEffect(itemData);

        if (itemData.Contains("PP"))
            return PPEffect();

        return false;
    }

    bool HealEffect(ItemData itemData)
    {
        bool isUsed = false;
        try
        {
            if (itemData.GetTotalStat(Stat.HP) != 0)
            {
                if (activeHeroData.currentHP < activeHeroData.stat.HP)
                {
                    int value = (int)(activeHeroData.stat.HP * 0.01f * itemData.GetTotalStat(Stat.HP));

                    activeHeroData.ChangeCurrentHP(value);
                    inventoryWindow.SetUnitInfo(activeHeroData);

                    if (inventoryWindow.gameObject.activeSelf)
                    {
                        for (int i = 0; i < herosPreview.Count; i++)
                        {
                            if (herosPreview[i].gameObject.activeSelf)
                            {
                                GameObject healEffect = EffectManager.Instance.InstantiateHealEffect(inventoryWindow.heroPosition);
                                healEffect.transform.localScale = new Vector3(100, 100, 100);
                                SetHPBar();
                            }
                        }
                    }

                    isUsed = true;
                }
            }
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);
        }

        return isUsed;
    }

    bool PPEffect()
    {
        activeHeroData.ResetPP();
        inventoryWindow.SetUnitInfo(activeHeroData);
        return true;
    }

    public void ContextSetActive(bool isActive)
    {
        try
        {
            inventoryContext.gameObject.SetActive(isActive);

            if (selectedItemSlot && selectedItemSlot.itemData != null)
            {
                if (selectedItemSlot.itemData.type == ItemType.Consumable || selectedItemSlot.itemData.type == ItemType.SkillBook || selectedItemSlot.itemData.type == ItemType.Revive)
                {
                    inventoryContext.buttonText.text = LocalizationManager.Instance.GetUILocalizingText("use");
                }
                else
                {
                    inventoryContext.buttonText.text = LocalizationManager.Instance.GetUILocalizingText("equip");
                }
            }

            else if (selectedConsumeSlot)
            {
                inventoryContext.buttonText.text = LocalizationManager.Instance.GetUILocalizingText("use");
            }

            else
            {
                inventoryContext.buttonText.text = LocalizationManager.Instance.GetUILocalizingText("equip");
            }
            inventoryContext.deleteButtonText.text = LocalizationManager.Instance.GetUILocalizingText("discard");
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    public void UseItemBtn()
    {
        if (selectedConsumeSlot)
        {
            selectedConsumeSlot.UseItem();
        }
        if (selectedEquipSlot)
        {
            EquipItemTakeOff(selectedEquipSlot.itemObject.type);
            selectedEquipSlot.SlotReset();
            selectedEquipSlot = null;
        }
        if (selectedItemSlot)
        {
            selectedItemSlot.UseItem();
        }
    }

    public void DeleteItem()
    {
        if (selectedConsumeSlot)
        {
            playerItems.DiscardItem(selectedConsumeSlot.itemData);
            selectedConsumeSlot.SlotReset();
        }
        if (selectedEquipSlot)
        {
            playerItems.DiscardItem(selectedEquipSlot.itemObject);
            selectedEquipSlot.SlotReset();
        }
        if (selectedItemSlot)
        {
            playerItems.DiscardItem(selectedItemSlot.itemData);
            selectedItemSlot.SlotReset();
        }
    }

    public void ReviveItem(ItemData itemData)
    {
        SetResult();
    }

    void SetResult()
    {
        float value = Random.Range(0, 1000);
        float currentPercent = SlotMachineManager.Instance.result;
        bool isSuccess = (value <= currentPercent);
        
        if (isSuccess)
        {
            RevieEffect();
            resultPopup.gameObject.SetActive(true);
            resultPopup.SetText(LocalizationManager.Instance.GetUILocalizingText("success"));
            
        }
        else
        {
            resultPopup.gameObject.SetActive(true);
            resultPopup.SetText(LocalizationManager.Instance.GetUILocalizingText("failed"));
            FailEffect();
        }
    }

    void RevieEffect()
    {
        try
        {
            int value = (int)(activeHeroData.stat.HP * 0.5f);
            SteamAchievements.Instance.HandleRevivalAchievement();
            activeHeroData.ChangeCurrentHP(value);
            inventoryWindow.SetUnitInfo(activeHeroData);

            if (inventoryWindow.gameObject.activeSelf)
            {
                for (int i = 0; i < herosPreview.Count; i++)
                {
                    if (herosPreview[i].gameObject.activeSelf)
                    {
                        EffectManager.Instance.ReviveEffect(herosPreview[i].transform);
                        herosPreview[i].PlayAnimation("isDie", false);
                    }
                }
            }
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    void FailEffect()
    {
        print("실패 효과 넣어야하는데 안들어가있네");
        if (inventoryWindow.gameObject.activeSelf)
        {
            for (int i = 0; i < herosPreview.Count; i++)
            {
                if (herosPreview[i].gameObject.activeSelf)
                {
                    EffectManager.Instance.ReviveEffect(herosPreview[i].transform);
                }
            }
        }
    }
}
