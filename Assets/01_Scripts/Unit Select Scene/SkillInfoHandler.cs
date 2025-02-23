using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInfoHandler : MonoBehaviour
{
    [SerializeField] SkillSlot firstSlot;
    [SerializeField] SkillSlot secondSlot;
    

    public void SetSkill(UnitData unitData)
    {
        //firstSlot.SetSkill(unitData.skills[0]);
        //secondSlot.SetSkill(unitData.skills[1]);

        //firstSlot.SetPPCount(unitData.skills[0].PP);
        //secondSlot.SetPPCount(unitData.skills[1].PP);

        firstSlot.SetUnitData(unitData, 0);
        secondSlot.SetUnitData(unitData, 1);
    }
}
