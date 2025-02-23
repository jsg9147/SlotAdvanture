using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using DarkTonic.MasterAudio;

public class GameManager : MonoBehaviour
{   
    private static GameManager instance;
    private StageData _stageData;
    public bool testMode;
    public StageConcept testConcept;
    public AudioMixer audioMixer;

    public Sprite goldSprite;
    [SerializeField]
    private float defenseDMG;
    [SerializeField]
    private float increaseRate;

    [SerializeField] GameSetting gameSettingPopupPrefab;
    [SerializeField] GameSetting gameSettingPopup;
    public string language = "en";

    public PlayData playData;

    [Header("Ending")]
    public EndingPopup endingPopupPrefab;
    public RecordTextController endingCredit;

    public float StatIncreaseRate
    {
        get 
        {
            return increaseRate; 
        }
    }
    public UnitData[] playerUnitDatas;

    public bool isGameStart;

    public List<RoomInfo> currentStageMap;
    public RoomInfo currentPlayerRoom;
    public RoomInfo beforeRoom;

    public int startMoney;

    public UnitData activeUnitData;

    [HideInInspector] public List<StageConcept> clearStage;
    [HideInInspector] public bool doubleSpeed;
    [HideInInspector] public bool infoPopupOn = true;

    [HideInInspector] public Dictionary<ItemData, int> storeData;
    [HideInInspector] public ItemData blackjackReward;
    [HideInInspector] public ItemData ghostLegReward;

    Canvas frontCanvas;

    List<LocalizationText> localizationTexts = new List<LocalizationText>();

    public Canvas FrontCanvas { get { return frontCanvas; } }
    public UnitData SelectPlayerUnit(int index)
    {
        activeUnitData = playerUnitDatas[index];
        return activeUnitData;
    }

    public float DefenseDMG
    {
        get { return defenseDMG; }
    }

    public StageData stageData
    {
        get { return _stageData; }
    }

    public ItemList UnitEquipList
    {
        get 
        {
            ItemList itemList = new ();
            for (int unitIndex = 0; unitIndex < playerUnitDatas.Length; unitIndex++)
            {
                for (int equipIndex = 0; equipIndex < playerUnitDatas[unitIndex].equipment.equipList.Count; equipIndex++)
                {
                    if (playerUnitDatas[unitIndex].equipment.equipList[equipIndex] != null)
                    {
                        itemList.SetItem(playerUnitDatas[unitIndex].equipment.equipList[equipIndex]);
                    }
                }
            }
            return itemList;
        }
    }
    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                return null;
            }
            return instance;
        }
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
        localizationTexts = new List<LocalizationText>();
        infoPopupOn = PlayerPrefs.GetInt("PopupOn", 0) == 0;
        frontCanvas = Instantiate(PrefabManager.Instance.frontCanvasPrefab);
        gameSettingPopup = Instantiate(gameSettingPopupPrefab, frontCanvas.transform);
        SceneManager.sceneLoaded += LoadSceneInit;
    }

    void LoadSceneInit(Scene scene, LoadSceneMode mode)
    {
        localizationTexts = new List<LocalizationText>();
        frontCanvas = Instantiate(PrefabManager.Instance.frontCanvasPrefab);
        ItemManagerInit();
        gameSettingPopup = Instantiate(gameSettingPopupPrefab, frontCanvas.transform);

        if (!SteamManager.Initialized)
            Application.Quit();
    }

    void ItemManagerInit()
    {
        if (SceneManager.GetActiveScene().name != "LOADING" && ItemManager.Instance != null)
        {
            ItemManager.Instance.SetCanvas(frontCanvas);
            ItemManager.Instance.Init();
        }
    }

    public void SetHeroUnitData(UnitData[] unitDatas)
    {
        playerUnitDatas = new UnitData[unitDatas.Length];

        for (int i = 0; i < unitDatas.Length; i++)
        {
            playerUnitDatas[i] = new UnitData(unitDatas[i]);
        }
    }

    public void SetStageData(StageData stageData)
    {
        if (this._stageData == null)
            stageData.SetStage(0);
        else
            stageData.SetStage(_stageData.Stage + 1);

        this._stageData = stageData;
    }

    public void ResetStageData()
    {
        try
        {
            _stageData = null;
            ItemManager.Instance.playerItems = new();
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);  
        }
    }

    public void SetMapDataList(List<RoomInfo> map)
    {
        currentStageMap = map;

        currentPlayerRoom = map.Find(x => x.distance == 0);
    }

    public void SetCurrentRoom(RoomInfo roomInfo)
    {
        if(currentPlayerRoom != null)
            beforeRoom = currentPlayerRoom;

        currentPlayerRoom = roomInfo;
    }

    public void BackRoom()
    {
        currentPlayerRoom = beforeRoom;
        LoadScene("MAP");
    }

    public void RoomClear()
    {
        currentPlayerRoom.isClear = true;
    }

    public void LoadScene(string sceneName)
    {
        LoadingManager.LoadScene(sceneName);
    }

    public void StageClear()
    {
        try
        {
            if (storeData != null)
                storeData.Clear();
            if (stageData.stageConcept == StageConcept.Final)
            {
                SteamAchievements.Instance.GameClearAchievement();
            }
            else
            {
                NextStageLoad();
            }

            GamebleRewardReset();
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    void GamebleRewardReset()
    {
        ghostLegReward = null;
        blackjackReward = null;
    }

    void NextStageLoad()
    {
        clearStage.Add(stageData.stageConcept);
        MapGenerateData.Instance.SetStageMap(_stageData.Stage + 1);
        LoadScene("MAP");
    }

    public void ResetStatusEffect()
    {
        for (int i = 0; i < playerUnitDatas.Length; i++)
        {
            playerUnitDatas[i].StatusEffectClear();
        }
    }

    public bool RandomSuccess(Unit unit)
    {
        print("이 문구를 테스트중 본적 없다면 삭제 필요");

        int slotResult = BattleManager.instance.SlotResult(unit.belong);
        int randomNumber = Random.Range(0, 1000);
        randomNumber -= (int)unit.unitData.stat.GetStatValue(Stat.ACC) * 10;

        return (randomNumber <= slotResult);
    }

    public AudioMixerGroup BGMAudioMixerGroup() => audioMixer.FindMatchingGroups("Master")[1];
    public AudioMixerGroup SFXAudioMixerGroup() => audioMixer.FindMatchingGroups("Master")[2];

    public void EndingCreditOn()
    {
        RecordTextController recordTextController = Instantiate(endingCredit, frontCanvas.transform);
        recordTextController.gameObject.SetActive(true);
        MasterAudio.ChangePlaylistByName("Ending");
    }

    public void EndingPopupOn()
    {
        EndingPopup endingPopup = Instantiate(endingPopupPrefab, frontCanvas.transform);
        endingPopup.gameObject.SetActive(true);
    }
    public void SettingPopupOn()
    {
        gameSettingPopup.gameObject.SetActive(true);
    }

    public void TextUpdate()
    {
        for (int i = 0; i < localizationTexts.Count; i++)
        {
            localizationTexts[i].SetLocalizationText();
        }
    }

    public void EnrollText(LocalizationText localizationText) => localizationTexts.Add(localizationText);
}

