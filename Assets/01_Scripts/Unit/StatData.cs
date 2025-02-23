using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StatData
{
    private float _hp;
    private float _ad;
    private float _ap;
    private float _def;
    private float _mr;
    private float _speed;
    private float _accuracy;

    public Equipment equipment = new Equipment();
    public bool isElite;

    List<StatusEffect> statusEffects = new List<StatusEffect>();

    public StatData OriginStats()
    {
        StatData originStat = new StatData();
        originStat.SetHP(_hp);
        originStat.SetAD(_ad);
        originStat.SetAP(_ap);
        originStat.SetDEF(_def);
        originStat.SetMR(_mr);
        originStat.SetSpeed(_speed);
        originStat.SetACC(_accuracy);

        return originStat;
    }

    public float HP
    {
        get { return _hp + changeHP + equipment.TotalHP + GetTotalValue(Stat.HP); }
    }
    public float AD
    {
        get { return _ad + changeAD + equipment.TotalAD + GetTotalValue(Stat.AD); }
    }
    public float AP
    {
        get { return _ap + changeAP + equipment.TotalAP + GetTotalValue(Stat.AP); }
    }

    public float DEF
    {
        get { return _def + changeDEF + equipment.TotalDEF + GetTotalValue(Stat.DEF); }
    }

    public float MR
    {
        get { return _mr + changeMR + equipment.TotalMR + GetTotalValue(Stat.MR); }
    }

    public float SPD
    {
        get { return _speed + changeSpeed + equipment.TotalSpeed + GetTotalValue(Stat.SPD); }
    }
    public float ACC
    {
        get { return _accuracy + changeAccuracy + equipment.TotalAccuracy + GetTotalValue(Stat.ACC); }
    }

    public float GetStatValue(Stat applyStat)
    {
        switch (applyStat)
        {
            case Stat.HP:
                return HP;
            case Stat.AD:
                return AD;
            case Stat.AP:
                return AP;
            case Stat.DEF:
                return DEF;
            case Stat.MR:
                return MR;
            case Stat.SPD:
                return SPD;
            case Stat.ACC:
                return ACC;
            case Stat.NoneType:
                return 0f;
            default:
                return 0f;
        }
    }

    public void SetHP(float hp) => this._hp = (int)hp;
    public void SetAD(float ad) => this._ad = (int)ad;
    public void SetAP(float ap) => this._ap = (int)ap;
    public void SetDEF(float def) => this._def = (int)def;
    public void SetMR(float mr) => this._mr = (int)mr;
    public void SetSpeed(float speed) => this._speed = (int)speed;
    public void SetACC(float accuracy) => this._accuracy = (int)accuracy;

    public void SetStatEffect(SkillEffect skillEffect)
    {
        int effectValue = (int)skillEffect.power;

        if (skillEffect.ratioStat == Stat.HP)
        {
            effectValue += (int)(HP * skillEffect.ratio);
        }
        if (skillEffect.ratioStat == Stat.AD)
        {
            effectValue += (int)(AD * skillEffect.ratio);
        }
        if (skillEffect.ratioStat == Stat.AP)
        {
            effectValue += (int)(AP * skillEffect.ratio);
            
        }
        if (skillEffect.ratioStat == Stat.ACC)
        {
            effectValue += (int)(ACC * skillEffect.ratio);
        }
        if (skillEffect.ratioStat == Stat.SPD)
        {
            effectValue += (int)(SPD * skillEffect.ratio);
        }
        if (skillEffect.ratioStat == Stat.DEF)
        {
            effectValue += (int)(DEF * skillEffect.ratio);
        }
        if (skillEffect.ratioStat == Stat.MR)
        {
            effectValue += (int)(MR * skillEffect.ratio);
        }

        ApplyEffectStat(effectValue, skillEffect.applyStat);
    }

    void ApplyEffectStat(int effectValue, Stat applyStat)
    {
        switch (applyStat)
        {
            case Stat.HP:
                SetHP(effectValue);
                break;
            case Stat.AD:
                SetAD(effectValue);
                break;
            case Stat.AP:
                SetAP(effectValue);
                break;
            case Stat.DEF:
                SetDEF(effectValue);
                break;
            case Stat.MR:
                SetMR(effectValue);
                break;
            case Stat.ACC:
                SetACC(effectValue);
                break;
            case Stat.SPD:
                SetSpeed(effectValue);
                break;
        }
    }

    public void SetEquip(Equipment equipment) => this.equipment = equipment;

    public float changeHP;
    public float changeAD;
    public float changeAP;
    public float changeDEF;
    public float changeMR;
    public float changeSpeed;
    public float changeAccuracy;

    public StatData()
    {
        BasicStatInitialize();
        ChangeStatInitialize();
    }

    public StatData(MonsterData monsterData)
    {
        SetHP((int)monsterData.HP);
        SetAD((int)monsterData.AD);
        SetAP((int)monsterData.AP);
        SetDEF((int)monsterData.DEF);
        SetMR((int)monsterData.MR);
        SetSpeed((int)monsterData.Speed);
    }
    public void Elite()
    {
        _hp = _hp * 1.2f;
        _ad = _ad * 1.2f;
        _ap = _ap * 1.2f;
        _def = _def * 1.2f;
        _mr = _mr * 1.2f;
        _accuracy = _accuracy * 1.0f;
        isElite = true;
    }

    void BasicStatInitialize()
    {
        _hp = 0f;
        _ad = 0f;
        _ap = 0f;
        _def = 0f;
        _speed = 0f;
        _accuracy = 0f;
    }
    public void ChangeStatInitialize()
    {
        changeHP = 0f;
        changeAD = 0f;
        changeAP = 0f;
        changeDEF = 0f;
        changeMR = 0f;
        changeSpeed = 0f;
        changeAccuracy = 0f;
    }

    public StatData(StatData stat)
    {
        _hp = stat.HP;
        _ad = stat.AD;
        _ap = stat.AP;
        _def = stat.DEF;
        _mr = stat.MR;
        _speed = stat.SPD;
        _accuracy = stat.ACC;
    }

    public void CreateRandomHP(int min, int max)
    {
        _hp = Mathf.Round(Random.Range(min, max) * 0.1f) * 10f;
    }

    public void RandomSplitStat(int total, int numberOfSplits, int minQuotient, int maxQuotient)
    {
        int[] splitValues = SplitTotal(total, numberOfSplits, minQuotient, maxQuotient);
        int sum = 0;
        foreach (int element in splitValues)
        {
            sum += element;
        }

        ShuffleArray(splitValues);

        _ad = splitValues[0] * 10;
        _ap = splitValues[1] * 10;
        _def = splitValues[2] * 10;
        _mr = splitValues[3] * 10;
        _speed = splitValues[4] * 10;

        //int temp = 0;
        //for (int i = 0; i < splitValues.Length; i++)
        //{
        //    temp += splitValues[i];
        //}
        //Debug.Log(temp);
    }

    int[] SplitTotal(int total, int numberOfSplits, int minQuotient, int maxQuotient)
    {
        int[] splits = new int[numberOfSplits];
        System.Random random = new System.Random();

        for (int i = 0; i < numberOfSplits - 1; i++)
        {
            int maxSplit = total - (numberOfSplits - 1 - i) * minQuotient;
            splits[i] = random.Next(minQuotient, Mathf.Min(maxQuotient, maxSplit) + 1);
            total -= splits[i];
        }

        int escCount = 0;
        if (total > maxQuotient)
        {
            splits[numberOfSplits - 1] = maxQuotient;
            total -= maxQuotient;
            while (total > 0)
            {
                int minIndex = FindMinValueIndex(splits);
                
                splits[minIndex] = splits[minIndex] + total;

                if (splits[minIndex] > maxQuotient)
                {
                    total = splits[minIndex] - maxQuotient;
                    splits[minIndex] = maxQuotient;
                }
                else
                    break;

                escCount++;
                if (escCount >= numberOfSplits)
                    break;
            }
        }
        else
        {
            splits[numberOfSplits - 1] = total;
        }

        return splits;
    }
    void ShuffleArray(int[] array)
    {
        int arrayLength = array.Length;
        System.Random random = new System.Random();

        for (int i = arrayLength - 1; i > 0; i--)
        {
            // 배열 내에서 무작위 인덱스 선택
            int randomIndex = random.Next(i + 1);

            // 선택한 인덱스의 요소와 현재 인덱스의 요소 교환
            int temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    int FindMinValueIndex(int[] array)
    {
        int minValue = int.MaxValue;
        int minValueIndex = -1;

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] < minValue)
            {
                minValue = array[i];
                minValueIndex = i;
            }
        }

        return minValueIndex;
    }

    public void ChangeStatEffect(StatData changeStat)
    {
        changeHP += changeStat.HP;
        changeAD += changeStat.AD;
        changeAP += changeStat.AP;
        changeDEF += changeStat.DEF;
        changeMR += changeStat.MR;
        changeSpeed += changeStat.SPD;
        changeAccuracy += changeStat.ACC;
    }

    public void AddEffect(StatusEffect effect)
    {
        if (statusEffects.Find(x => x.Code == effect.Code) == null)
        {
            statusEffects.Add(effect);
        }
    }

    public void RemoveEffect(StatusEffect effect)
    {
        statusEffects.Remove(effect);
    }

    public int GetEffectStat(SkillTag skillTag)
    {
        return 0;
    }

    float GetTotalValue(Stat stat)
    {
        float value = 0f;
        foreach (var effect in statusEffects)
        {
            value += effect.GetEffectStat(stat);
        }

        return value;
    }
}
