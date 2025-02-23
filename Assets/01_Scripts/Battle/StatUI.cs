using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatUI : MonoBehaviour
{
    public Image unitImage;

    public TMP_Text hpText;
    public TMP_Text adText;
    public TMP_Text apText;
    public TMP_Text defText;
    public TMP_Text mrText;
    public TMP_Text spdText;
    public TMP_Text accText;

    public void SetStatText(UnitData unitData)
    {
        adText.text = unitData.stat.GetStatValue(Stat.AD).ToString();
        apText.text = unitData.stat.GetStatValue(Stat.AP).ToString();
        defText.text = unitData.stat.GetStatValue(Stat.DEF).ToString();
        mrText.text = unitData.stat.GetStatValue(Stat.MR).ToString();
        spdText.text = unitData.stat.GetStatValue(Stat.SPD).ToString();
        accText.text = unitData.stat.GetStatValue(Stat.ACC).ToString();
    }

    public void SetInventoryStat(UnitData unitData)
    {
        //hpText.text = $"{(int)unitData.currentHP} / {(int)unitData.stat.HP}";
        adText.text = unitData.stat.AD.ToString();
        apText.text = unitData.stat.AP.ToString();
        defText.text = unitData.stat.DEF.ToString();
        spdText.text = unitData.stat.SPD.ToString();
        mrText.text = unitData.stat.MR.ToString();
    }

    public void SetImage(Sprite sprite) => unitImage.sprite = sprite;
}
