using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData
{
    public string unitCode;

    public SoundGroup soundGroup;
    public int index;
    public int prefabIndex;

    public StatData stat;
    public Equipment equipment = new Equipment();
    public List<SkillObject> skills = new List<SkillObject>();

    public Dictionary<int, int> skillPP = new Dictionary<int, int>();

    public List<StatusEffect> statusEffects = new List<StatusEffect>();

    public StatusEffect sanctuarySkill;

    public float currentHP;

    public bool gambleConcept;

    // 스킬은 나중에 추가하자 오브젝트라서 찾아 써야함

    public UnitData()
    {
        equipment = new Equipment();
        stat = new StatData();
    }

    public UnitData(UnitData unitData)
    {
        unitCode = unitData.unitCode;
        index = unitData.index;
        equipment = unitData.equipment;
        skills = unitData.skills;
        prefabIndex = unitData.prefabIndex;
        soundGroup = unitData.soundGroup;

        stat = new StatData(unitData.stat);
        currentHP = stat.HP;
        gambleConcept = unitData.gambleConcept;

        for (int i = 0; i < skills.Count; i++)
        {
            skillPP.Add(i, skills[i].PP);
        }
    }

    public UnitData(MonsterData monsterData)
    {
        try
        {
            unitCode = monsterData.monsterCode;
            soundGroup = monsterData.soundGroup;
        }
        catch(System.NullReferenceException ex)
        {
            Debug.Log(ex);
            unitCode = "Ex";
        }

        stat = new StatData(monsterData);
        currentHP = stat.HP;

        skills = monsterData.Skills;
    }
    public void SetUnitCode(string code) => unitCode = code;

    public void SetSkills(List<SkillObject> skillObjects)
    {
        skills = skillObjects;
        for (int i = 0; i < skills.Count; i++)
        {
            skillPP.Add(i, skills[i].PP);
        }
    }

    public void SetStageStat()
    {
        stat.SetHP(IncreaseStat(stat.HP));
        stat.SetAD(IncreaseStat(stat.AD));
        stat.SetAP(IncreaseStat(stat.AP));
        stat.SetDEF(IncreaseStat(stat.DEF));
        stat.SetMR(IncreaseStat(stat.MR));
        //stat.SetSpeed(IncreaseStat(stat.SPD));

        currentHP = stat.HP;
    }

    public bool EquipCheck(ItemType itemType)
    {
        return equipment.EquipCheck(itemType);
    }

    public void EquipChange(ItemData itemData)
    {
        float hpRatio = currentHP / stat.HP;

        equipment.TakeOffEquip(itemData);

        equipment.PutOnEquip(itemData);
        stat.SetEquip(equipment);

        currentHP = hpRatio * stat.HP;
    }

    public void EquipTakeOff(ItemData itemData)
    {
        float hpRatio = currentHP / stat.HP;

        equipment.TakeOffEquip(itemData);
        currentHP = hpRatio * stat.HP;
    }

    public void LearnSkill(SkillObject skillObject, int index)
    {
        skills[index] = skillObject;
        skillPP[index] = skillObject.PP;
    }

    int IncreaseStat(float stat)
    {
        float increseStat = stat;
        for (int i = 0; i < GameManager.Instance.stageData.Stage; i++)
        {
            increseStat += increseStat * GameManager.Instance.StatIncreaseRate;
        }

        return (int)((increseStat * 0.1f) * 10);
    }

    public string UnitName
    {
        get { return LocalizationManager.Instance.GetUnitLocalizingName(unitCode); }
    }

    public string MonsterName
    {
        get { return LocalizationManager.Instance.GetMonsterLocalizingName(unitCode); }
    }

    public void EquipChange(ItemSlot itemSlot)
    {
        equipment.PutOnEquip(itemSlot.itemData);

        stat.SetEquip(equipment);
    }

    public void AddEffect(StatusEffect effect)
    {
        StatusEffect exsitEffect = statusEffects.Find(x => x.Code == effect.Code);

        if (exsitEffect != null)
        {
            statusEffects.Remove(exsitEffect);
        }

        statusEffects.Add(effect);

        if (effect.isRevival)
        {
            if (sanctuarySkill != null)
            {
                RemoveEffect(sanctuarySkill);
            }
            sanctuarySkill = effect;
        }

        if (effect.ContainStatus())
        {
            stat.AddEffect(effect);
        }
    }

    void RemoveEffect(StatusEffect effect)
    {
        statusEffects.Remove(effect);
        stat.RemoveEffect(effect);
    }

    public void TurnEnd()
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            statusEffects[i].TurnEnd();
            if (statusEffects[i].IsEnd)
            {
                RemoveEffect(statusEffects[i]);
            }
        }
    }

    public void StatusEffectClear()
    {
        for (int i = 0; i < statusEffects.Count; i++)
        {
            statusEffects[i].turnCount += statusEffects[i].Duration;
        }
        TurnEnd();
    }

    public void ChangeCurrentHP(float changeValue)
    {
        currentHP += changeValue;
        if(currentHP > stat.HP)
            currentHP = stat.HP;
    }

    public List<StatusEffect> DotDamageEfffects()
    {
        List<StatusEffect> debuffEffects = new List<StatusEffect>();
        for (int i = 0; i < statusEffects.Count; i++)
        {
            if (statusEffects[i].statValues[Stat.HP] > 0)
                debuffEffects.Add(statusEffects[i]);
        }

        return debuffEffects;
    }

    public void ResetPP()
    {
        for (int i = 0; i < skills.Count; i++)
        {
            skillPP[i] = skills[i].PP;
        }
    }

    public void ReduceSkillPP(int index)
    {
        if (skillPP[index] > 0)
            skillPP[index]--;
        else
            skillPP[index] = 0;
    }

    public bool SkillCanUse(SkillObject skillObject)
    {
        int index = skills.IndexOf(skillObject);
        return skillPP[index] > 0;
    }

    int SkillIndex(SkillObject skillObject) => skills.IndexOf(skillObject);
}
