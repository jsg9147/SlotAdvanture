using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class EmphasisUnitInfo
{
    public Color adEmphasisColor;
    public Color apEmphasisColor;
    public Color defEmphasisColor;
    public Color mrEmphasisColor;
    public Color spdEmphasisColor;
    public Color accEmphasisColor;

    public Sprite adIcon;
    public Sprite apIcon;
    public Sprite defIcon;
    public Sprite mrIcon;
    public Sprite spdIcon;
    public Sprite accIcon;
}
public class UnitSelectSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] EmphasisUnitInfo emphasisUnitInfo;
    [HideInInspector]
    public UnitData unitData;

    [SerializeField] Vector3 unitPosition;
    [SerializeField] float scaleSet;
    [SerializeField] Transform prefabParent;
    [SerializeField] TMP_Text nameText;
    [SerializeField] GameObject selectMark;
    [SerializeField] GameObject selectedFrame;
    [SerializeField] Image background;
    [SerializeField] Image emphasisIcon;

    GameObject platformUnit;

    Unit unit;
    public void SetUnitData(UnitData unitData, Transform unitTrans)
    {
        this.unitData = unitData;
        unitTrans.transform.SetParent(prefabParent);
        unitTrans.localPosition = unitPosition;
        unitTrans.localScale = Vector3.one * scaleSet;
        unitTrans.gameObject.SetActive(true);
        InfoEmphasis(unitData.stat);

        if (nameText)
        {
            nameText.font = LocalizationManager.Instance.GetFont();
            nameText.text = unitData.UnitName;
        }
    }

    public void SetUnit(Unit unit)
    {
        this.unit = unit;
        this.unitData = unit.unitData;
        unit.transform.SetParent(prefabParent);
        unit.transform.localPosition = Vector3.zero;
        unit.transform.localScale = Vector3.one * 2.5f;
        unit.gameObject.SetActive(true);

        if (nameText)
        {
            nameText.font = LocalizationManager.Instance.GetFont();
            nameText.text = unitData.UnitName;
        }

        InfoEmphasis(unitData.stat);
    }

    void InfoEmphasis(StatData statData)
    {
        float[] stats = { statData.AD, statData.AP, statData.DEF, statData.MR, statData.SPD, statData.ACC };
        float maxStat = FindMaxStat(stats, out int maxIndex);
        Stat statName = GetStatName(maxIndex);
    }

    float FindMaxStat(float [] array, out int maxIndex)
    {
        float max = array[0]; // 초기값으로 배열의 첫 번째 원소 설정
        maxIndex = 0;

        // 배열을 순회하면서 최댓값 찾기
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] > max)
            {
                max = array[i];
                maxIndex = i;
            }
        }

        return max;
    }
    Stat GetStatName(int index)
    {
        switch (index)
        {
            case 0:
                background.color = emphasisUnitInfo.adEmphasisColor;
                emphasisIcon.sprite = emphasisUnitInfo.adIcon;
                return Stat.AD;
            case 1: 
                background.color = emphasisUnitInfo.apEmphasisColor;
                emphasisIcon.sprite = emphasisUnitInfo.apIcon;
                return Stat.AP;
            case 2: 
                background.color = emphasisUnitInfo.defEmphasisColor;
                emphasisIcon.sprite = emphasisUnitInfo.defIcon;
                return Stat.DEF;
            case 3: 
                background.color = emphasisUnitInfo.mrEmphasisColor;
                emphasisIcon.sprite = emphasisUnitInfo.mrIcon;
                return Stat.MR;
            case 4: 
                background.color = emphasisUnitInfo.spdEmphasisColor;
                emphasisIcon.sprite = emphasisUnitInfo.spdIcon;
                return Stat.SPD;
            case 5: 
                background.color = emphasisUnitInfo.accEmphasisColor;
                emphasisIcon.sprite = emphasisUnitInfo.accIcon;
                return Stat.ACC;
            default: 
                return Stat.HP;
        }
    }

    public void SetPlatformOnUnit(GameObject unitObj) => this.platformUnit = unitObj;

    public void SetActiveUnit(bool isActive) => platformUnit.SetActive(isActive);

    public void SelectUnit(bool isActive) => selectMark.SetActive(isActive);

    public void SelectedFrameSetActive(bool isActive) => selectedFrame.SetActive(isActive);

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (unit != null)
            UIController.Instance.UnitDataPopupActive(unit);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        UIController.Instance.PopupOff();
    }
}
