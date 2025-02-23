using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterAI : MonoBehaviour
{
    public float lastDitch_Hp_Percent;

    List<Unit> monsters;

    public void MonsterAction()
    {
        monsters = BattleManager.instance.monsters;

        for (int i = 0; i < monsters.Count; i++)
        {
            UnitAction unitAction = new UnitAction(monsters[i]);
            switch (RandomPatternNumber(monsters[i]))
            {
                case 0:
                    Attack(unitAction);
                    break;
                case 1:
                    Defense(unitAction);
                    break;
                case 2:
                    Skill(unitAction);
                    break;
            }
            monsters[i].actionEnd = true;
        }
    }

    //TODO : 스텟에 따른 행동 확률 변경 필요 AI 상향 
    int RandomPatternNumber(Unit unit)
    {
        int actionIndex;
        float attackPercent = 25f;
        float DefencePercent = 10f;

        if (unit.unitData.skills.FindAll(x => x.Contains(SkillTag.Buff)).Count >= unit.unitData.skills.Count)
        {
            attackPercent += 30f;
        }

        float randomActionNumber = Random.Range(0, 100);

        if(randomActionNumber < attackPercent) // 공
        {
            actionIndex = 0;
        }
        else if(randomActionNumber >= attackPercent &&  randomActionNumber < attackPercent + DefencePercent) // 방
        {
            actionIndex = 1;
        }
        else // 스킬
        {
            actionIndex = 2;

            if (unit.unitData.skills.Count <= 0)
                actionIndex = 0;
        }

        return actionIndex;
    }

    void Attack(UnitAction unitAction)
    {
        unitAction.battleAction = BattleAction.ATK;

        //unitAction.target = AliveRandomTarget;
        unitAction.targets.Add(AliveRandomTarget);

        BattleManager.instance.AddAction(unitAction);
    }

    void Defense(UnitAction unitAction)
    {
        unitAction.battleAction = BattleAction.DEF;

        unitAction.unit.defenceMode = (true);
        BattleManager.instance.AddAction(unitAction);
    }

    void Skill(UnitAction unitAction)
    {
        int randomSkillIndex;
        Unit monster = unitAction.unit;

        randomSkillIndex = Random.Range(0, monster.monsterData.Skills.Count);

        unitAction.battleAction = BattleAction.SKILL;
        unitAction.skillObject = monster.Skill(randomSkillIndex);

        if (unitAction.skillObject.SkillEffectType == VisualEffectType.MultiShot || unitAction.skillObject.SkillEffectType == VisualEffectType.FullScale)
            unitAction.targets = BattleManager.instance.playerUnits;
        else
            unitAction.targets.Add(AliveRandomTarget);

        BattleManager.instance.AddAction(unitAction);
    }

    Unit AliveRandomTarget
    {
        get
        {
            int whileCount = 0;
            Unit randomAliveUnit;
            while (true)
            {
                int randomHeroIndex = Random.Range(0, BattleManager.instance.playerUnits.Count);

                randomAliveUnit = BattleManager.instance.playerUnits[randomHeroIndex];

                if (randomAliveUnit.IsAlive)
                    break;
                else
                {
                    whileCount++;

                    if (whileCount > 15)
                    {
                        Debug.Log("코드 수정이 필요함");
                        break;
                    }
                }
            }

            return randomAliveUnit;
        }
    }
}
