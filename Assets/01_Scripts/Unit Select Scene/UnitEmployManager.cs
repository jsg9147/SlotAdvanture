using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using DG.Tweening;

public class UnitEmployManager : MonoBehaviour
{
    public SoundGroup soundGroup;

    [SerializeField] private int _hpMin, _hpMax;
    [SerializeField] private int _totalStat;
    [SerializeField] private int _statMin, _statMax;
    [SerializeField] private int statCount = 5;
    [SerializeField] private float gambleGenerateRate;

    private void Start()
    {
        Initialize();
    }


    [SerializeField] private List<SkillObject> startADSkillList;
    [SerializeField] private List<SkillObject> startAPSkillList;
    [SerializeField] private List<SkillObject> startBuffSkillList;

    [SerializeField] TMP_Text nameText;
    [SerializeField] Button completeBtn;
    [SerializeField] ButtonController selectBtn;

    [SerializeField] GameObject gambleMark;
    [SerializeField] StatTextHandler statTextHandler;
    [SerializeField] SkillInfoHandler skillInfoHandler;

    [SerializeField] UnitSelectSlot unitBtnPrefab;
    [SerializeField] Transform unitSelectBtnParent;
    [SerializeField] List<GameObject> unitPrefabs;
    [SerializeField] List<string> unitName;

    [SerializeField] Transform activeUnitParent;
   
    List<UnitSelectSlot> unitSelectBtns;

    List<UnitSelectSlot> selectedUnit;

    int gamePlayUnitCount = 3;

    Dictionary<UnitData, GameObject> selectUnitDictionary;

    UnitSelectSlot activeData;
    void Initialize()
    {
        GameManager.Instance.ResetStageData();
        unitSelectBtns = new();
        selectedUnit = new();
        completeBtn.interactable = false;
        InstantiateBtn();
        SetRandomDatas();
        ShuffleSelectBtns();

        selectBtn.button.onClick.AddListener(() =>
        {
            UnitSelect();
            CheckSelectCount();
        });

        completeBtn.onClick.AddListener(() =>
        {
            Complite();
        });

        try
        {
            UnitClick(unitSelectBtns[0].unitData);
            activeData = unitSelectBtns[0];
            CheckSelectCount();
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    void InstantiateBtn()
    {
        for (int i = 0; i < unitPrefabs.Count; i++)
        {
            UnitSelectSlot unitBtn = Instantiate(unitBtnPrefab, unitSelectBtnParent);
            unitSelectBtns.Add(unitBtn);

            unitBtn.GetComponent<Button>().onClick.AddListener(() =>
            {
                activeData.SelectedFrameSetActive(false);
                unitBtn.SelectedFrameSetActive(true);
                UnitClick(unitBtn.unitData);
                activeData = unitBtn;
                CheckSelectCount();
                
            });
        }
    }

    void SetRandomDatas()
    {
        selectUnitDictionary = new();
        for (int i = 0; i < unitSelectBtns.Count; i++)
        {
            UnitData unitData = GetRandomUnitData();
            unitData.prefabIndex = i;
            unitData.SetSkills(GetStartRandomSkill(i));
            unitData.SetUnitCode(unitName[i]);
            unitData.gambleConcept = (gambleGenerateRate >= Random.Range(0f, 100f));
            SetActiveUnitObject(i, unitData);
        }
    }
    UnitData GetRandomUnitData()
    {
        UnitData randomData = new UnitData();
        randomData.stat.RandomSplitStat(_totalStat, statCount, _statMin, _statMax);
        randomData.stat.CreateRandomHP(_hpMin, _hpMax);

        if (GameManager.Instance.testMode)
        {
            randomData.stat.SetHP(10000);
            randomData.stat.SetAD(10000);
            randomData.stat.SetAP(10000);
            randomData.stat.SetDEF(10000);
            randomData.stat.SetMR(10000);
            randomData.stat.SetSpeed(10000);
            randomData.stat.SetACC(100);
        }

        return randomData;
    }
    List<SkillObject> GetStartRandomSkill(int prefabIndex)
    {
        List<SkillObject> startSkills = new();

        switch (Random.Range(0, 2))
        {
            case 0:
                startSkills.Add(startADSkillList[Random.Range(0, startADSkillList.Count)]);
                break;
            case 1:
                startSkills.Add(startAPSkillList[Random.Range(0, startAPSkillList.Count)]);
                break;
        }

        startSkills.Add(startBuffSkillList[Random.Range(0, startBuffSkillList.Count)]);
        return startSkills;
    }

    void SetActiveUnitObject(int index, UnitData unitData)
    {
        Transform unitTrans = Instantiate(unitPrefabs[index]).transform;

        Unit unitOnPlatform = Instantiate(PrefabManager.Instance.GetUnitPrefab(unitData), activeUnitParent);
        unitOnPlatform.transform.localScale = Vector3.one * 150f;
        unitOnPlatform.transform.localPosition = Vector3.forward * -100f;
        unitOnPlatform.gameObject.SetActive(false);
        unitSelectBtns[index].SetUnitData(unitData, unitTrans);

        selectUnitDictionary.Add(unitData, unitOnPlatform.gameObject);
    }

    void ShuffleSelectBtns()
    {
        System.Random rng = new System.Random();

        int n = unitSelectBtns.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            UnitSelectSlot value = unitSelectBtns[k];
            unitSelectBtns[k] = unitSelectBtns[n];
            unitSelectBtns[n] = value;
        }

        for (int i = 0; i < unitSelectBtns.Count; i++)
        {
            unitSelectBtns[i].transform.SetSiblingIndex(i);
        }
    }

    void UnitClick(UnitData unitData)
    {
        statTextHandler.SetStatData(unitData);
        skillInfoHandler.SetSkill(unitData);
        nameText.font = LocalizationManager.Instance.GetFont();
        nameText.text = unitData.UnitName;
        foreach (var temp in selectUnitDictionary.Values)
        {
            temp.SetActive(false);
        }
        gambleMark.SetActive(unitData.gambleConcept);
        selectUnitDictionary[unitData].SetActive(true);
    }

    public void UnitSelect()
    {
        bool isSelected = selectedUnit.Contains(activeData);

        if (isSelected)
        {
            selectedUnit.Remove(activeData);
        }
        else
        {
            selectedUnit.Add(activeData);
        }
        
        activeData.SelectUnit(!isSelected);
        completeBtn.interactable = (selectedUnit.Count >= gamePlayUnitCount);
    }

    void CheckSelectCount()
    {
        selectBtn.SetInteractable(selectedUnit.Count < gamePlayUnitCount);
        selectBtn.SetText($"{LocalizationManager.Instance.GetUILocalizingText("select")}\n{selectedUnit.Count}/{gamePlayUnitCount}");
        selectBtn.SetBtnImage(false);

        if (selectedUnit.Contains(activeData))
        {
            selectBtn.SetInteractable(true);
            selectBtn.SetText($"{LocalizationManager.Instance.GetUILocalizingText("cancel")}\n{selectedUnit.Count}/{gamePlayUnitCount}");
            selectBtn.SetBtnImage(true);
        }
    }

    void Complite()
    {
        GameManager.Instance.playData = new PlayData();
        UnitData[] selectDatas = new UnitData[selectedUnit.Count];
        for (int i = 0; i < selectedUnit.Count; i++)
        {
            selectDatas[i] = new UnitData(selectedUnit[i].unitData);
            selectDatas[i].soundGroup = soundGroup;
        }
        GameManager.Instance.SetHeroUnitData(selectDatas);
        GameManager.Instance.isGameStart = true;
        ItemManager.Instance.SetCurrnetMoney();
        GameManager.Instance.LoadScene("MAP");
    }
}