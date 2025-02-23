using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InfoPopup : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] InfoPopupTopLayout topLayout;

    [SerializeField] GameObject statGroupLayout;
    [SerializeField] GameObject textGroupLayout;
    [SerializeField] TMP_Text infoText;
    [SerializeField] StatTextHandler statTextHandle;

    [SerializeField] RectTransform statGroupParent;
    UnitData unitDt;

    RectTransform rectTransform;
    GameObject canvas;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        infoText.font = LocalizationManager.Instance.GetFont();
        //rectTransform.pivot = Vector2.up;
    }

    private void FixedUpdate()
    {
        MouseTracker();
    }

    private void OnDisable()
    {

    }

    public void SetCanvas(GameObject canvasObj) => canvas = canvasObj;

    void MouseTracker()
    {
        Vector3 mousePosition = Input.mousePosition;

        // Canvas의 Render Mode가 Screen Space - Camera 일 때
        if (Camera.main != null)
        {
            // UI 요소를 카메라에서의 월드 좌표로 변환
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                GetComponent<RectTransform>(),
                mousePosition,
                Camera.main,
                out Vector3 worldPosition
            );

            // UI 요소를 마우스 위치로 바로 이동
            transform.position = worldPosition;
        }
        rectTransform.anchoredPosition = rectTransform.anchoredPosition + new Vector2(rectTransform.rect.width/2 , -rectTransform.rect.height/2) ;
        PopupPositionCorrection();
    }

    void PopupPositionCorrection()
    {
        float maxXpos = canvas.GetComponent<RectTransform>().rect.width / 2 - rectTransform.rect.width;
        float maxYpos = canvas.GetComponent<RectTransform>().rect.height / 2 - rectTransform.rect.height;

        if (rectTransform.anchoredPosition.x >= maxXpos)
        {
            rectTransform.localPosition = new(rectTransform.localPosition.x - rectTransform.rect.width, rectTransform.localPosition.y);
        }
        if (rectTransform.anchoredPosition.y >= maxYpos)
        {
            rectTransform.localPosition = new(rectTransform.localPosition.x, rectTransform.localPosition.y - rectTransform.rect.height);
        }
        if (rectTransform.anchoredPosition.y <= -maxYpos)
        {
            rectTransform.localPosition = new(rectTransform.localPosition.x, rectTransform.localPosition.y + rectTransform.rect.height);
        }
    }

    public void SetSkillData(SkillObject skillObject, UnitData unitData)
    {
        topLayout.SetSkillData(skillObject);
        InfoTextActive();
        infoText.text = DescripReplace.StringReplace(skillObject, unitData);
    }

    public void SetBuffText(StatusEffect statusEffect)
    {
        topLayout.SetEffectData(statusEffect);
        InfoTextActive();

        infoText.text = DescripReplace.StringReplace(statusEffect);
    }
    public void SetUnitData(UnitData unitData) => this.unitDt = unitData;

    public void SetItemData(ItemData itemData)
    {
        bool isStatItem = itemData.type != ItemType.SkillBook && itemData.type != ItemType.Consumable && itemData.type != ItemType.Revive;

        if (itemData.type == ItemType.SkillBook)
        {
            SkillBookData(itemData);
        }
        else
        {
            topLayout.SetItemData(itemData);
        }

        if (isStatItem)
        {
            StatInfoActive();
            statTextHandle.SetItemData(itemData);

        }
        else
        {
            InfoTextActive();

            if (itemData.type == ItemType.Consumable)
            {
                SetConsumeItemData(itemData);
            }
            if (itemData.type == ItemType.Revive)
            {
                SetSpacialText(itemData);
            }
        }

        PopupHeightControl();
    }

    void StatInfoActive()
    {
        textGroupLayout.SetActive(false);
        statGroupLayout.SetActive(true);
        PopupHeightControl();
    }

    void InfoTextActive()
    {
        statGroupLayout.SetActive(false);
        textGroupLayout.SetActive(true);
    }

    void SetConsumeItemData(ItemData itemData)
    {
        statGroupLayout.SetActive(false);
        textGroupLayout.SetActive(true);

        infoText.text = DescripReplace.DescriptionInterpolation(itemData);
       
    }

    void SetSpacialText(ItemData itemData)
    {
        statGroupLayout.SetActive(false);
        textGroupLayout.SetActive(true);

        infoText.text = LocalizationManager.Instance.GetItemLocalizingDescription(itemData.Code);
    }

    void SkillBookData(ItemData itemData)
    {
        SkillObject skillObject = itemData.skillObject;
        topLayout.SetSkillData(skillObject);
        infoText.text =  DescripReplace.StringReplace(skillObject);

        topLayout.SetPrice(itemData.price);
    }

    void PopupHeightControl()
    {
        Vector2 size = statGroupParent.sizeDelta;
        size.y = statTextHandle.HeightCal();
        statGroupParent.sizeDelta = size;
    }

    public void UnitInfoPopup(Unit unit)
    {
        UnitData unitData = unit.unitData;
        unitDt = unitData;

        topLayout.SetUnitData(unit);
        topLayout.SetLabel(unit.belong);

        statTextHandle.SetUnitData(unitData);
        StatInfoActive();

        PopupHeightControl();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        gameObject.SetActive(false);
    }
}

public class DescripReplace
{
    public static string StringReplace(SkillObject skillObject, UnitData unitData)
    {
        string descriptionWithVariables = skillObject.GetDescription()
                                                             .Replace("{basic}", $"{(int)skillObject.BasicPower()}")
                                                             .Replace("{total}", $"{(int)skillObject.TotalValue(unitData.stat)}")
                                                             .Replace("{ratio}", $"{skillObject.GetRatio()}")
                                                             .Replace("{duration}", $"{skillObject.Duration}")
                                                             .Replace("{ratioHP}", $"{skillObject.GetStatRatio(Stat.HP)}<sprite=0>")
                                                             .Replace("{ratioAD}", $"{skillObject.GetStatRatio(Stat.AD)}<sprite=2>")
                                                             .Replace("{ratioAP}", $"{skillObject.GetStatRatio(Stat.AP)}<sprite=3>")
                                                             .Replace("{ratioDEF}", $"{skillObject.GetStatRatio(Stat.DEF)}<sprite=4>")
                                                             .Replace("{ratioMR}", $"{skillObject.GetStatRatio(Stat.MR)}<sprite=5>")
                                                             .Replace("{ratioACC}", $"{skillObject.GetStatRatio(Stat.ACC)}<sprite=6>")
                                                             .Replace("{ratioSPD}", $"{skillObject.GetStatRatio(Stat.SPD)}<sprite=1>")
                                                             .Replace("\\n", "\n");

        return descriptionWithVariables;
    }
    public static string StringReplace(SkillObject skillObject)
    {
        string descriptionWithVariables = skillObject.GetSkillbookDescription().Replace("{basic}", $"{(int)skillObject.BasicPower()}")
                                                             .Replace("{duration}", $"{skillObject.Duration}")
                                                             .Replace("{ratioHP}", $"{skillObject.GetStatRatio(Stat.HP)}<sprite=0>")
                                                             .Replace("{ratioAD}", $"{skillObject.GetStatRatio(Stat.AD)}<sprite=2>")
                                                             .Replace("{ratioAP}", $"{skillObject.GetStatRatio(Stat.AP)}<sprite=3>")
                                                             .Replace("{ratioDEF}", $"{skillObject.GetStatRatio(Stat.DEF)}<sprite=4>")
                                                             .Replace("{ratioMR}", $"{skillObject.GetStatRatio(Stat.MR)}<sprite=5>")
                                                             .Replace("{ratioACC}", $"{skillObject.GetStatRatio(Stat.ACC)}<sprite=6>")
                                                             .Replace("{ratioSPD}", $"{skillObject.GetStatRatio(Stat.SPD)}<sprite=1>")
                                                             .Replace("{total}", $"{(int)skillObject.BasicPower()} + {skillObject.GetRatio()}")
                                                             .Replace("\\n", "\n");

        return descriptionWithVariables;
    }

    public static string DescriptionInterpolation(ItemData itemData)
    {
        string descriptionWithVariables = itemData.GetItemDescription()
                                                            .Replace("{hp}", $"{itemData.GetTotalStat(Stat.HP)}<sprite=0>")
                                                             .Replace("{ad}", $"{itemData.GetTotalStat(Stat.AD)}<sprite=2>")
                                                             .Replace("{ap}", $"{itemData.GetTotalStat(Stat.AP)}<sprite=3>")
                                                             .Replace("{def}", $"{itemData.GetTotalStat(Stat.DEF)}<sprite=4>")
                                                             .Replace("{mr}", $"{itemData.GetTotalStat(Stat.MR)}<sprite=5>")
                                                             .Replace("{spd}", $"{itemData.GetTotalStat(Stat.SPD)}<sprite=1>")
                                                             .Replace("{acc}", $"{itemData.GetTotalStat(Stat.ACC)}<sprite=6>")
                                                             .Replace("\\n", "\n");

        return descriptionWithVariables;
    }

    public static string StringReplace(StatusEffect statusEffect)
    {
        string descriptionWithVariables = statusEffect.description.Replace("{hp}", $"{statusEffect.statValues[Stat.HP]}<sprite=0>")
                                                             .Replace("{ad}", $"{statusEffect.statValues[Stat.AD]}<sprite=2>")
                                                             .Replace("{ap}", $"{statusEffect.statValues[Stat.AP]}<sprite=3>")
                                                             .Replace("{def}", $"{statusEffect.statValues[Stat.DEF]}<sprite=4>")
                                                             .Replace("{mr}", $"{statusEffect.statValues[Stat.MR]}<sprite=5>")
                                                             .Replace("{spd}", $"{statusEffect.statValues[Stat.SPD]}<sprite=1>")
                                                             .Replace("{acc}", $"{statusEffect.statValues[Stat.ACC]}<sprite=6>")
                                                             .Replace("{total}", $"{statusEffect.Total()}")
                                                             .Replace("{duration}", $"{statusEffect.Duration}")
                                                             .Replace("\\n", "\n");

        return descriptionWithVariables;
    }
}