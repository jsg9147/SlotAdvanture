using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class StageMonsterGroup
{
    public Unit bossPrefab;
    public Unit[] monsterPrefabs;
}

public class BattleObjPooling : MonoBehaviour
{
    public TMP_Text unitNamePrefab;
    public BuffSlotController buffSlotControllerPrefab;

    [Header("Monster Prefabs")]
    public StageMonsterGroup tutorialMonsterPrefabs;
    public StageMonsterGroup fireMonsterPrefabs;
    public StageMonsterGroup forestMonsterPrefabs;
    public StageMonsterGroup caveMonsterPrefabs;
    public StageMonsterGroup snowMonsterPrefabs;
    public StageMonsterGroup finalMonsterPrefabs;

    public Transform[] heroPositions;
    public Transform[] monsterPositions;

    public Dictionary<int, Unit[]> stageBossDictionary;

    public ProgressBarPro horizentalBar;
    public Transform hpBarParent;

    public GameObject endMarkPrefab;

    [Range(0, 1)]
    public float eliteChance;
    private void Start()
    {
        SetBattleStage();
    }

    void SetBattleStage()
    {
        HeroInstantiate();

        if (GameManager.Instance.currentPlayerRoom.monsterPrefabs.Count > 0)
        {
            if (GameManager.Instance.currentPlayerRoom.roomConcept == RoomConcept.NORMAL)
                MonsterInstantiate();
        }
        else
        {
            if (GameManager.Instance.currentPlayerRoom.roomConcept == RoomConcept.BOSS)
                BossInstantiate();
            else
            {
                SetMonsterDataToRoomInfo();
                MonsterInstantiate();
            }
        }

        Instantiate(GameManager.Instance.stageData.background);
    }

    void HeroInstantiate()
    {
        for (int i = 0; i < GameManager.Instance.playerUnitDatas.Length; i++)
        {
            GameManager.Instance.playerUnitDatas[i].index = i;
            PlayerUnitSetPlay(GameManager.Instance.playerUnitDatas[i]);
        }
    }

    void PlayerUnitSetPlay(UnitData heroUnitData)
    {
        Unit heroUnit = Instantiate(PrefabManager.Instance.GetUnitPrefab(heroUnitData));

        heroUnit.SetBuffController(Instantiate(buffSlotControllerPrefab, heroUnit.transform));
        if (heroUnit.GetComponent<SpriteRenderer>())
            heroUnit.GetComponent<SpriteRenderer>().flipX = true;

        heroUnit.transform.position = heroPositions[heroUnitData.index].transform.position;
        heroUnit.transform.localScale = Vector3.one * 1.5f;
        heroUnit.Init(heroUnitData);
        heroUnit.gameObject.SetActive(true);

        heroUnit.endMark = Instantiate(endMarkPrefab, heroUnit.transform);

        heroUnit.endMark.SetActive(false);
        if (heroUnit.unitData.sanctuarySkill != null)
        {
            GameObject aura = EffectManager.Instance.InstantiateAura(heroUnit.transform, heroUnit.unitData.sanctuarySkill);
            heroUnit.SetSanctuaryEffect(aura);
            aura.transform.localPosition = new(0, 0, 0.1f);
            heroUnit.buffSlotController.SetBuffSlot(heroUnitData.statusEffects);
        }

        NameTextInstantiate(heroUnit);

        SetHpBar(heroUnit);
        BattleManager.instance.AddUnit(heroUnit);
    }

    void MonsterInstantiate()
    {
        List<Unit> monsterPrefabs = GameManager.Instance.currentPlayerRoom.monsterPrefabs;
        List<UnitData> monsterDatas = GameManager.Instance.currentPlayerRoom.monsterDatas;
        for (int i = 0; i < monsterPrefabs.Count; i++)
        {
            Unit monster = Instantiate(monsterPrefabs[i], transform);
            monster.SetBuffController(Instantiate(buffSlotControllerPrefab, monster.transform));
            monster.belong = Belong.Monster;
            monster.transform.position = monsterPositions[i].position;
            monster.transform.GetComponent<SpriteRenderer>().flipX = true;
            monster.Init(monsterDatas[i]);

            if (monsterPrefabs.Count <= 1)
            {
                monster.transform.position = monsterPositions[i].position;
            }

            if (monsterDatas[i].stat.isElite)
            {
                monster.transform.localScale = monsterPrefabs[i].transform.localScale * 1.5f;
            }

            NameTextInstantiate(monster);

            SetHpBar(monster);
            BattleManager.instance?.AddUnit(monster);
        }
    }

    void SetMonsterDataToRoomInfo()
    {
        int randomMonsterCount = Random.Range(1, monsterPositions.Length + 1);

        bool isElite = Random.Range(0f, 1f) < eliteChance;
        List<Unit> monsterPrefabs = new ();
        List<UnitData> monsterDatas = new ();
        for (int i = 0; i < randomMonsterCount; i++)
        {
            Unit monsterPrefab = StageMonsterGenerate();
            UnitData monsterData = new UnitData(monsterPrefab.monsterData);

            if (isElite && i == 0)
            {
                monsterData.stat.Elite();
            }

            monsterData.SetStageStat();

            monsterDatas.Add(monsterData);
            monsterPrefabs.Add(monsterPrefab);
        }

        GameManager.Instance.currentPlayerRoom.SetMonsterData(monsterPrefabs, monsterDatas);
    }
    void BossInstantiate()
    {
        Unit stageBoss = Instantiate(GetStageBoss(), transform);
        stageBoss.belong = Belong.Monster;
        stageBoss.transform.position = monsterPositions[0].position;
        stageBoss.transform.localScale = new(2, 2, 2);
        stageBoss.transform.GetComponent<SpriteRenderer>().flipX = true;
        stageBoss.SetBuffController(Instantiate(buffSlotControllerPrefab, stageBoss.transform));
        UnitData unitData = new UnitData(stageBoss.monsterData);
        unitData.SetStageStat();

        stageBoss.Init(unitData);
        NameTextInstantiate(stageBoss);

        SetHpBar(stageBoss);
        BattleManager.instance?.AddUnit(stageBoss);
    }
    void NameTextInstantiate(Unit unit)
    {
        TMP_Text nameText = Instantiate(unitNamePrefab, unit.transform);
        Vector2 namePosition;
        if (unit.monsterData == null)
        {
            nameText.text = unit.unitData.UnitName;
            namePosition = Vector2.up * 1.3f;
        }
        else
        {
            nameText.text = unit.unitData.MonsterName;
            namePosition = unit.monsterData.namePos;
        }

        nameText.GetComponent<RectTransform>().anchoredPosition = namePosition; // 스케일로 나눠야 스케일 다른 유닛도 정상 적용 가능
        nameText.transform.localScale = new(1f / unit.transform.localScale.x, Mathf.Abs(1 / unit.transform.localScale.y), 1);

        if (unit.isReinforced)
        {
            nameText.text = $"<sprite=7>{unit.unitData.UnitName}";
        }
    }

    Unit GetStageBoss()
    {
        Unit selectedBoss = StageMonsterPrefabsArray().bossPrefab;
        return selectedBoss;
    }

    Unit StageMonsterGenerate()
    {
        StageMonsterGroup stageMonsterGroup = StageMonsterPrefabsArray();
        Unit[] monsterPrefabs = stageMonsterGroup.monsterPrefabs;
        int randomIndex = Random.Range(0, monsterPrefabs.Length);
        return monsterPrefabs[randomIndex];
    }

    StageMonsterGroup StageMonsterPrefabsArray()
    {
        switch (GameManager.Instance.stageData.stageConcept)
        {
            case StageConcept.Tutorial:
                return tutorialMonsterPrefabs;
            case StageConcept.Fire:
                return fireMonsterPrefabs;
            case StageConcept.Forest:
                return forestMonsterPrefabs;
            case StageConcept.Cave:
                return caveMonsterPrefabs;
            case StageConcept.Snow:
                return snowMonsterPrefabs;
            case StageConcept.Final:
                return finalMonsterPrefabs;
            default:
                return finalMonsterPrefabs;
        }
    }

    void SetHpBar(Unit unit)
    {
        ProgressBarPro hpBar = Instantiate(horizentalBar, hpBarParent);
        Vector3 hpBarPos = unit.transform.position + ((Vector3.down * 0.5f) * unit.transform.localScale.y);

        if (unit.belong == Belong.Player)
        {
            hpBarPos = unit.transform.position + ((Vector3.down * 0.35f) * unit.transform.localScale.y);
        }
        unit.SetHpBar(hpBar);
        hpBar.transform.localPosition = hpBarPos;
        hpBar.transform.SetParent(unit.transform);
    }
}
