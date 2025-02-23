using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect
{
    private string code;
    public string Code
    {
        get { return code; }
    }

    private int duration;
    public int Duration
    {
        get { return duration; }
    }

    public Sprite icon;

    public int turnCount;

    public string name;
    public string description;

    public Dictionary<Stat, float> statValues;

    public SkillParticle effectParticle;

    public bool isRevival;

    public bool isDebuff = false;

    public int grade;

    SkillObject skill;

    public int RemainingTurns
    {
        get 
        {
            int value = duration - turnCount + 1;

            if (duration <= 0)
                value = 0;
            return value; 
        }
    }
    

    public StatusEffect(SkillObject skillObject)
    {
        skill = skillObject;
        name = skill.GetName();
        code = skill.Code;
        duration = skill.Duration;
        turnCount = 0;
        effectParticle = skill.effect;

        icon = skill.SkillIcon;
        isRevival = skill.Contains(SkillTag.Revival);
        description = skill.GetBuffDescription();
        isDebuff = (skill.Contains(SkillTag.Debuff));

        SetEffectData(skillObject.skillEffects);
        grade = skillObject.grade;

    }

    public StatusEffect(ItemData itemData)
    {
        name = itemData.GetItemName();
        code = itemData.Code;
        turnCount = 0;
        isRevival = false;
        duration = itemData.effects[0].duration;
        icon = itemData.icon;
        SetEffectData(itemData);
        description = itemData.GetItemDescription();
    }

    public void SetStatData(StatData statData)
    {
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            float value = 0f;
            for (int i = 0; i < skill.skillEffects.Count; i++)
            {
                if (IsBuffType(skill.skillEffects[i]))
                {
                    if (skill.skillEffects[i].applyStat == stat)
                    {
                        value += (skill.skillEffects[i].ratio * statData.GetStatValue(stat));
                    }
                }
            }
            if (isDebuff)
                value = value * -1f;

            statValues[stat] = value + skill.BasicPower(stat);
        }
    }

    void SetEffectData(List<SkillEffect> effects)
    {
        statValues = new Dictionary<Stat, float>();
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            float value = 0f;
            for (int i = 0; i < effects.Count; i++)
            {
                SkillEffect skillEffect = effects[i];
                if (IsBuffType(skillEffect) && effects[i].applyStat == stat)
                {
                    value += (effects[i].power);
                }
            }
            if (isDebuff)
            {
                value = value * -1f;
            }

            statValues.Add(stat, value);
        }
    }

    bool IsBuffType(SkillEffect skillEffect)
    {
        if (skillEffect.skillTag == SkillTag.Buff)
            return true;
        if (skillEffect.skillTag == SkillTag.Debuff)
            return true;

        return false;
    }

    void SetEffectData(ItemData itemData)
    {
        statValues = new Dictionary<Stat, float>();
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            statValues.Add(stat, itemData.GetTotalStat(stat));
        }
    }

    public float GetEffectStat(Stat targetStat)
    {
        float value = statValues[targetStat];
        return value;
    }

    public void TurnEnd()
    {
        turnCount++;
    }
    public bool IsEnd
    {
        get
        {
            return turnCount > duration && duration > 0;
        }
    }

    public bool ContainStatus()
    {
        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            if (stat != Stat.HP)
            {
                if (statValues[stat] != 0)
                {
                    return true;
                }
            }
        }

        if (skill != null)
        {
            return skill.Contains(SkillTag.Buff);
        }

        return false;
    }

    public float Total()
    {
        float value = 0;

        foreach (Stat stat in Enum.GetValues(typeof(Stat)))
        {
            value = value + statValues[stat];
        }
        return value;
    }
}