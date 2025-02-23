using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageData
{
    private int stage;
    private StageConcept _stageConcept;
    public GameObject background;

    public StageConcept stageConcept
    {
        get { return _stageConcept; }
    }
    public int Stage
    {
        get { return stage; }
    }
    public void SetStage(int stage) => this.stage = stage;

    public void SetRandomStage()
    {
        _stageConcept = GetRandomStageConcept();

        if (stage == 0)
        {
            _stageConcept = StageConcept.Tutorial;
            if (GameManager.Instance.testMode)
                _stageConcept = GameManager.Instance.testConcept;
        }

        if(stage == 3)
            _stageConcept = StageConcept.Final;
    }

    StageConcept GetRandomStageConcept()
    {
        StageConcept getValue = StageConcept.Fire;
        var stageValues = System.Enum.GetValues(enumType: typeof(StageConcept));
        int count = 0;

        while (true)
        {
            getValue = (StageConcept)stageValues.GetValue(Random.Range(0, stageValues.Length - 1));
            if (!GameManager.Instance.clearStage.Contains(getValue))
                break;

            count++;
            if (count > 20)
            {
                Debug.Log("코드 수정 필요");
                break;
            }
        }
        return getValue;
    }
}

public enum StageConcept
{ 
    Tutorial,
    Fire,
    Forest,
    Snow,
    Cave,
    Final
}

