using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class StatTextGroup
{
    public TMP_Text hpText;
    public TMP_Text adText;
    public TMP_Text apText;
    public TMP_Text defText;
    public TMP_Text mrText;
    public TMP_Text spdText;
    public TMP_Text accText;

    public GameObject hpTextGroup;
    public GameObject adTextGroup;
    public GameObject apTextGroup;
    public GameObject defTextGroup;
    public GameObject mrTextGroup;
    public GameObject spdTextGroup;
    public GameObject accTextGroup;

    public void CheckZero(ItemData itemData)
    {
        hpTextGroup.SetActive(itemData.GetTotalStat(Stat.HP) != 0);
        adTextGroup.SetActive(itemData.GetTotalStat(Stat.AD) != 0);
        apTextGroup.SetActive(itemData.GetTotalStat(Stat.AP) != 0);
        defTextGroup.SetActive(itemData.GetTotalStat(Stat.DEF) != 0);
        mrTextGroup.SetActive(itemData.GetTotalStat(Stat.MR) != 0);
        spdTextGroup.SetActive(itemData.GetTotalStat(Stat.SPD) != 0);
        accTextGroup.SetActive(itemData.GetTotalStat(Stat.ACC) != 0);
    }

    public void AllStatOn()
    {
        hpTextGroup.SetActive(true);
        adTextGroup.SetActive(true);
        apTextGroup.SetActive(true);
        defTextGroup.SetActive(true);
        mrTextGroup.SetActive(true);
        spdTextGroup.SetActive(true);
        accTextGroup.SetActive(true);
    }

    public int ActiveStatCount()
    {
        int count = 0;
        if (hpTextGroup.activeSelf)
            count++;
        if (adTextGroup.activeSelf)
            count++;
        if (apTextGroup.activeSelf)
            count++;
        if (defTextGroup.activeSelf)
            count++;
        if (mrTextGroup.activeSelf)
            count++;
        if (spdTextGroup.activeSelf)
            count++;
        if (accTextGroup.activeSelf)
            count++;

        return count;
    }
}

public class StatTextHandler : MonoBehaviour
{
    [SerializeField] StatTextGroup textGroup;
    public void SetStatData(UnitData unitData)
    {
        try
        {
            textGroup.adText.text = $"{unitData.stat.AD}";
            textGroup.apText.text = $"{unitData.stat.AP}";
            textGroup.defText.text = $"{unitData.stat.DEF}";
            textGroup.mrText.text = $"{unitData.stat.MR}";
            textGroup.spdText.text = $"{unitData.stat.SPD}";

            if(textGroup.hpText != null)
                textGroup.hpText.text = $"{unitData.stat.HP}";

            if(textGroup.accText != null)
                textGroup.accText.text = $"{unitData.stat.ACC}";
        }
        catch(System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    public void SetItemData(ItemData itemData)
    {
        try
        {
            textGroup.adText.text = $"{itemData.GetTotalStat(Stat.AD)}";
            textGroup.apText.text = $"{itemData.GetTotalStat(Stat.AP)}";
            textGroup.defText.text = $"{itemData.GetTotalStat(Stat.DEF)}";
            textGroup.mrText.text = $"{itemData.GetTotalStat(Stat.MR)}";
            textGroup.spdText.text = $"{itemData.GetTotalStat(Stat.SPD)}";
            textGroup.hpText.text = $"{itemData.GetTotalStat(Stat.HP)}";
            textGroup.accText.text = $"{itemData.GetTotalStat(Stat.ACC)}";

            textGroup.CheckZero(itemData);
        }
        catch(System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    public void SetUnitData(UnitData unitData)
    {
        try
        {
            textGroup.hpText.text = $"{unitData.stat.HP}";
            textGroup.spdText.text = $"{unitData.stat.SPD}";
            textGroup.adText.text = $"{unitData.stat.AD}";
            textGroup.apText.text = $"{unitData.stat.AP}";
            textGroup.defText.text = $"{unitData.stat.DEF}";
            textGroup.mrText.text = $"{unitData.stat.MR}";
            textGroup.accText.text = $"{unitData.stat.ACC}";

            textGroup.AllStatOn(); ;
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    public float HeightCal()
    {
        int verticalLine = Mathf.CeilToInt(textGroup.ActiveStatCount() / 2.0f);
        return verticalLine * 70;
    }
}
