using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocalStr : SerializableDictionary<string, string> { }

[System.Serializable]
public class IntPair : SerializableDictionary<int, int> { }

[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Object/Skill Data", order = int.MaxValue)]
public class SkillObject : ScriptableObject
{
    [SerializeField] private string codeName;
    public string Code
    {
        get { return codeName; }
    }

    public List<SkillEffect> skillEffects;

    [SerializeField]
    private Sprite skillIcon;

    public Sprite SkillIcon { get { return skillIcon; } }

    [SerializeField]
    private int duration;
    public int Duration { get { return duration; } }
    [SerializeField]
    private int pp;

    public int PP { get { return pp; } }

    [SerializeField]
    private VisualEffectType effectType;

    public VisualEffectType SkillEffectType { get { return effectType; } }

    public bool startTargetPosition;

    public SkillParticle effect;

    public Vector3 effectPosition;
    public Quaternion effectRotate;
    public float effectDelay;
    public float disapearTime;

    public int grade;

    public string GetName()
    {
        return LocalizationManager.Instance.GetSkillLocalizingName(codeName);
    }
    public string GetDescription()
    {
        return LocalizationManager.Instance.GetSkillLocalizingDescription(codeName);
    }

    public string GetSkillbookDescription()
    {
        return LocalizationManager.Instance.GetSkillBookLocalizingDescription(codeName);
    }

    public string GetBuffDescription()
    {
        return LocalizationManager.Instance.GetSkillLocalizingBuffDescription(codeName);
    }


    public DamageType damageType
    {
        get 
        {
            if (skillEffects.Count > 0)
            {
                return skillEffects[0].damageType;
            }
            return DamageType.AD;
        }
    }

    public bool Contains(SkillTag skillTag)
    {
        bool isContain = false;
        for (int i = 0; i < skillEffects.Count; i++)
        {
            if (skillEffects[i].skillTag == skillTag)
            {
                isContain = true;
                break;
            }
        }

        return isContain;
    }

    public float TotalValue(StatData stat)
    {
        float total = 0;
        for (int i = 0; i < skillEffects.Count; i++)
        {
            if (skillEffects[i].ratioStat == Stat.HP)
            {
                total = total + TotalValueCal(skillEffects[i], stat.HP);
            }
            if (skillEffects[i].ratioStat == Stat.AD)
            {
                total = total + TotalValueCal(skillEffects[i], stat.AD);
            }
            if (skillEffects[i].ratioStat == Stat.AP)
            {
                total = total + TotalValueCal(skillEffects[i], stat.AP);
            }
            if (skillEffects[i].ratioStat == Stat.DEF)
            {
                total = total + TotalValueCal(skillEffects[i], stat.DEF);
            }
            if (skillEffects[i].ratioStat == Stat.MR)
            {
                total = total + TotalValueCal(skillEffects[i], stat.MR);
            }
            if (skillEffects[i].ratioStat == Stat.ACC)
            {
                total = total + TotalValueCal(skillEffects[i], stat.ACC);
            }
            if (skillEffects[i].ratioStat == Stat.SPD)
            {
                total = total + TotalValueCal(skillEffects[i], stat.SPD);
            }
        }
        return total;
    }

    public float RatioCalculate(Stat stat, StatData statData)
    {
        float total = 0f;
        for (int i = 0; i < skillEffects.Count; i++)
        {
            total = total + (skillEffects[i].ratio * statData.GetStatValue(stat));
        }
        return total;
    }

    public float GetRatio()
    {
        if (skillEffects.Count > 0)
        {
            return skillEffects[0].ratio;
        }

        return 0f;
    }

    public float GetStatRatio(Stat targetStat)
    {
        float total = 0f;
        for (int i = 0; i < skillEffects.Count; i++)
        {
            if(skillEffects[i].ratioStat == targetStat)
                total = total + (skillEffects[i].ratio);
        }
        return total;
    }

    // 함수명 같아서 손봐야함
    public float BasicPower(Stat applyStat)
    {
        float total = 0f;
        for (int i = 0; i < skillEffects.Count; i++)
        {
            if(skillEffects[i].applyStat == applyStat)
                total = total + (skillEffects[i].power);
        }
        return total;
    }
    public float BasicPower()
    {
        float total = 0f;
        for (int i = 0; i < skillEffects.Count; i++)
        {
            total = total + (skillEffects[i].power);
        }
        return total;
    }

    float TotalValueCal(SkillEffect skillEffect, float statValue)
    {
        return (skillEffect.ratio * statValue) + skillEffect.power;
    }
}

public enum VisualEffectType
{
    Melee,
    Range,
    Buff,
    MultiShot,
    FullScale,
    OnHit,
    Gamble,
    Curve,
    ChargedShot
}

public enum SkillTag
{
    Buff,
    Debuff,
    Damage,
    Revival,
    OnHit
}

public enum DamageType
{
    None,
    AD,
    AP,
    Ture, // Fixed 는 완전 고정, 버프 디버프 영향 없는거, True 는 수치는 변화 하나 방어력 영향 X
}

[System.Serializable]
public class SkillEffect
{
    public SkillTag skillTag;
    public Stat ratioStat;
    public Stat applyStat;
    public DamageType damageType;
    public float power;
    public float ratio;
}