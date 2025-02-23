using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DarkTonic.MasterAudio;

public class SkillBookPopup : MonoBehaviour
{
    public GameObject clickBlockPanel;
    public GameObject tryEffect;
    public GameObject resultPopup;

    //public Transform unitHorizentalGroup;

    public Button unitSelectedBtn;
    //public List<GameObject> selectEffects;
    public TMP_Text titleText;
    public List<TMP_Text> percentTexts;
    public TMP_Text resultText;

    [Header("캐릭터 선택창")]
    public GameObject unitSelectPopup;
    [SerializeField] List<UnitSelectSlot> unitSlots;

    [Header("스킬 선택창")]
    public GameObject removeSkillPopup;
    public List<Image> iconImage;
    public List<GameObject> removeSkillSelectMark;
    [SerializeField] Button skillSelectedBtn;
    [SerializeField] Button skillCancelBtn;

    //int selectedUnitIndex;
    int selectedSkillIndex;

    float currentPercent;

    bool isSuccess;

    UnitData selectedUnitData;

    private void Start()
    {
        //selectedUnitIndex = 0;
        UnitSetup();
        AddButtonEvent();
        unitSelectPopup.SetActive(false);
        removeSkillPopup.SetActive(false);

        titleText.font = LocalizationManager.Instance.GetFont();
        titleText.text = LocalizationManager.Instance.GetUILocalizingText("skillbookTitle");

        skillSelectedBtn.onClick.AddListener(() => TryEffectOn());
        skillCancelBtn.onClick.AddListener(() => SkillSelectPopupSetActive(false));
    }

    void PopupReset()
    {
        for (int i = 0; i < unitSlots.Count; i++)
        {
            unitSlots[i].SelectUnit(false);
        }
        for (int i = 0; i < removeSkillSelectMark.Count; i++)
        {
            removeSkillSelectMark[i].SetActive(false);
        }
        selectedUnitData = null;
        unitSelectedBtn.interactable = selectedUnitData != null;

        selectedSkillIndex = 0;
        skillSelectedBtn.interactable = false;
    }

    void UnitSetup()
    {
        if (GameManager.Instance.isGameStart)
        {
            for (int i = 0; i < GameManager.Instance.playerUnitDatas.Length; i++)
            {
                Unit unit = Instantiate( PrefabManager.Instance.GetUnitPrefab(GameManager.Instance.playerUnitDatas[i]));
                unit.unitData = (GameManager.Instance.playerUnitDatas[i]);
                unit.unitData.index = i;
                if (unitSlots.Count > i)
                {
                    unitSlots[i].SetUnit(unit);
                    unit.transform.localPosition = (Vector3.down * 60f) +(Vector3.forward * -20f);
                    unit.transform.localScale = Vector3.one * 120f;
                }
                unit.LayerChange("Foreground");
            }
        }
    }

    void AddButtonEvent()
    {
        for (int i = 0; i < unitSlots.Count; i++)
        {
            UnitData unitData = unitSlots[i].unitData;
            unitSlots[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                selectedUnitData = unitData;
                unitSelectedBtn.interactable = true;
                SelectUnitMarkReset();
            });
        }
    }

    void SkillIconSetup(UnitData unitData)
    {
        for (int i = 0; i < iconImage.Count; i++)
        {
            iconImage[i].sprite = unitData.skills[i].SkillIcon;
        }
    }

    public void SelectUnitMarkReset()
    {
        for (int i = 0; i < unitSlots.Count; i++)
        {
            unitSlots[i].SelectUnit(unitSlots[i].unitData == selectedUnitData);
        }
    }
    public void SkillSelectPopupSetActive(bool isActive)
    {
        SkillIconSetup(selectedUnitData);
        unitSelectPopup.SetActive(!isActive);
        removeSkillPopup.SetActive(isActive);
    }

    public void SelectSkill(int index)
    {
        selectedSkillIndex = index;
        skillSelectedBtn.interactable = true;
        for (int i = 0; i < removeSkillSelectMark.Count; i++)
        {
            removeSkillSelectMark[i].SetActive(index == i);
        }
    }

    public void TryEffectOn()
    {
        removeSkillPopup.SetActive(false);
        tryEffect.SetActive(true);
        StartCoroutine(SetResult());
    }


    IEnumerator SetResult()
    {
        yield return new WaitForSeconds(2f);
        tryEffect.SetActive(false);
        float value = Random.Range(0, 1000);

        isSuccess = (value < currentPercent);

        GameManager.Instance.playData.TrySkillBook(isSuccess);

        if (isSuccess)
            Success();
        else
            Failed();

        resultPopup.SetActive(true);
        PopupReset();
    }

    public void Success()
    {
        resultText.text = LocalizationManager.Instance.GetUILocalizingText("success");
        GameManager.Instance.playerUnitDatas[selectedUnitData.index].LearnSkill(SkillManager.instance.bookSkill, selectedSkillIndex);
        ItemManager.Instance.EquipSlotHeroChange(0);
        MasterAudio.PlaySound("SkillbookSuccess");
    }

    public void Failed()
    {
        resultText.text = LocalizationManager.Instance.GetUILocalizingText("failed");
        MasterAudio.PlaySound("SkillbookFailed");
    }

    public void PopupOff()
    {
        resultPopup.SetActive(false);
        clickBlockPanel.SetActive(false);
    }

    public void PopupSetActive(float percent)
    {
        unitSelectPopup.SetActive(true);
        clickBlockPanel.SetActive(true);
        currentPercent = percent;

        foreach (TMP_Text text in percentTexts)
        {
            text.text = $"{(percent * 0.1f).ToString()}%";
        }
    }
}
