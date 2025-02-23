using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAction
{
    public Unit unit;
    //public Unit target;
    public List<Unit> targets;
    public BattleAction battleAction;
    public float speed;
    public ItemData itemData;
    public SkillObject skillObject;

    public int indexKey;

    public int gambleResult;

    public UnitAction(Unit unit)
    {
        targets = new List<Unit>();
        this.unit = unit;
        speed = unit.unitData.stat.SPD;
    }
    public UnitAction(UnitAction copyAction)
    {
        this.unit = copyAction.unit;
        this.targets = copyAction.targets;
        this.battleAction = copyAction.battleAction;
        this.speed = copyAction.speed;
    }
    public bool RandomSuccess(int slotResult)
    {
        int randomNumber = Random.Range(0, 1000);
        randomNumber -= (int)unit.unitData.stat.ACC * 10;

        BattleManager.instance.testText.text = $"È®·ü : {slotResult}\n" +
            $"·£´ý : (»ÌÀº ¼ýÀÚ) {randomNumber + (int)unit.unitData.stat.ACC * 10} + (¸íÁß·ü ½ºÅÝ) {(int)unit.unitData.stat.ACC * 10} \n" +
            $"0º¸´Ù Å©¸é ¸íÁß : {slotResult - randomNumber}";

        if (slotResult <= 100 && randomNumber <= slotResult)
        {
            SteamAchievements.Instance.HandleComebackAchievement();
        }

        return (randomNumber <= slotResult);
    }

    public bool CanAction()
    {
        if (battleAction == BattleAction.DEF)
            return true;
        if (targets.Count == 0)
        {
            if (unit.IsAlive)
                return true;
        }
        else
        {
            if (unit.IsAlive && targets.FindAll(x => x.IsAlive).Count > 0)
                return true;
        }

        return false;
    }

    public void TargetReact()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            float damage = GetDamageCal(targets[i]);
            bool isSuccess = RandomSuccess(BattleManager.instance.SlotResult(unit.belong));
            DamageType damageType = DamageType.AD;
            if (isSuccess)
            {
                if (battleAction == BattleAction.SKILL)
                {
                    SkillEffectHandle(targets[i]);
                    damageType = skillObject.damageType;
                }

                if (damageType != DamageType.None)
                {
                    if (skillObject != null)
                    {
                        if (!skillObject.Contains(SkillTag.Debuff))
                            targets[i].Damaged(damage);
                    }
                    else
                    {
                        targets[i].Damaged(damage);
                    }
                }
            }
            else
            {
                EffectManager.Instance.MissEffect(targets[i].transform);
            }

            GameManager.Instance.playData.SetAttackRecord(unit.belong, isSuccess);
        }
    }

    void SkillEffectHandle(Unit target)
    {
        if (skillObject.Contains(SkillTag.Damage))
        {
            EffectManager.Instance.SkillDamagedEffect(skillObject, target.transform);
        }
        if (skillObject.Contains(SkillTag.Buff) || skillObject.Contains(SkillTag.Debuff))
        {
            StatEffect(target);
        }
    }

    void StatEffect(Unit target)
    {
        StatusEffect effect = new StatusEffect(skillObject);
        effect.SetStatData(unit.unitData.stat);
        target.AddBuffData(effect);
    }

    float GetDamageCal(Unit target)
    {
        float damage;
        if (skillObject == null)
        {
            damage = unit.unitData.stat.AD;
            FinalDamage(ref damage, DamageType.AD, target);
        }
        else
        {
            damage = skillObject.TotalValue(unit.unitData.stat);
            if (skillObject.damageType != DamageType.Ture)
                FinalDamage(ref damage, skillObject.damageType, target);
        }

        if (unit.unitData.gambleConcept)
            damage = damage * gambleResult;

        return damage;
    }
    void FinalDamage(ref float baseDamage, DamageType damageType, Unit target)
    {
        if (damageType == DamageType.AD)
            baseDamage = baseDamage - target.unitData.stat.DEF;
        else if (damageType == DamageType.AP)
            baseDamage = baseDamage - target.unitData.stat.MR;

        if (target.defenceMode)
            baseDamage = baseDamage * GameManager.Instance.DefenseDMG;

        if (baseDamage < 0)
            baseDamage = 0;
    }
}

public enum BattleAction
{
    ATK,
    DEF,
    SKILL,
    ITEM
}
