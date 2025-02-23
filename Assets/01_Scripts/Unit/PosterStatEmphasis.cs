using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Not Use
public class PosterStatEmphasis : MonoBehaviour
{
    public Color emphasisIconColor;
    public Color emphasisTextColor;

    public Stat stat;

    public Image icon;

    public TMP_Text statNameText;
    public TMP_Text statText;

    public void SetStatData(StatData statData)
    {
        switch (stat)
        {
            case Stat.AD:
                statText.text = statData.AD.ToString();
                break;
            case Stat.AP:
                statText.text = statData.AP.ToString();
                break;
            case Stat.DEF:
                statText.text = statData.DEF.ToString();
                break;
            case Stat.MR:
                statText.text = statData.MR.ToString();
                break;
            case Stat.SPD:
                statText.text = statData.SPD.ToString();
                break;
            case Stat.HP:
                statText.text = statData.HP.ToString();
                break;
        }
    }
}
