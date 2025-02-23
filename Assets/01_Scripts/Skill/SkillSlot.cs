using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image skillIcon;
    public TMP_Text ppCount;
    public GameObject xIcon;

    UnitData unitData;
    SkillObject skillObject;
    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIController.Instance.SetSkillInfoPopup(unitData, skillObject);
        //UIController.Instance.InfoPopupSetActive(true, skillObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIController.Instance.PopupOff();
    }

    void SetSkill(SkillObject skillObject)
    {
        this.skillObject = skillObject;
        skillIcon.sprite = skillObject.SkillIcon;
    }

    void SetCount(UnitData unitData, int index)
    {
        try
        {
            int sparePP = unitData.skillPP[index];
            ppCount.text = sparePP.ToString() + "/" + skillObject.PP;

            if (button != null)
            {
                button.interactable = sparePP > 0;
                xIcon.SetActive(!button.interactable);
            }
        }
        catch(System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    public void SetPPCount(int count)
    {
        ppCount.text = count.ToString() + "/" + skillObject.PP;
    }

    public void SetUnitData(UnitData unitData) => this.unitData = unitData;

    public void SetUnitData(UnitData unitData, int targetIndex)
    {
        this.unitData = unitData;
        SetSkill(unitData.skills[targetIndex]);
        SetCount(unitData, targetIndex);
    }
}
