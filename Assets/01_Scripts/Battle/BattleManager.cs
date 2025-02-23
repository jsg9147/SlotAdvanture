using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DarkTonic.MasterAudio;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    public TMPro.TMP_Text testText;

    public Button escapeBtn;
    public Button doubleSpdBtn;

    [SerializeField] List<BattleUnitButton> playerSelectBtns;
    [SerializeField] List<BattleUnitButton> monsterSelectBtns;

    public PercentText playerPercent;
    public PercentText monsterPercent;

    public RewardController rewardController;

    public BattleUI battleUI;
    public MonsterAI monsterAI;

    public List<Unit> playerUnits = new ();
    public List<Unit> monsters = new ();

    public GameObject playerSelectMark;
    public GameObject monsterSelectMark;

    public Vector3 selectMarkPosition;

    Unit selected_Hero;
    Unit selected_Monster;

    public List<UnitAction> unitActions = new ();
    Queue<UnitAction> actionQueue = new() ;

    int playerSlotResult;
    int monsterSlotResult;

    List<Unit> defenseUnit;

    List<UnitAction> currentActions = new();

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        defenseUnit = new List<Unit>();
        doubleSpdBtn.onClick.AddListener(DoubleSpeed);

        escapeBtn.onClick.AddListener(Escape);

        playerSelectMark.SetActive(true);
        monsterSelectMark.SetActive(true);
        MasterAudio.ChangePlaylistByName("Battle");
    }

    public void SelectButtonSet(Unit unit, int index)
    {
        try
        {
            if (unit.belong == Belong.Player)
            {
                playerSelectBtns[index].SetUnit(unit);
            }
            else
            {
                monsterSelectBtns[index].SetUnit(unit);
            }
        }
        catch (System.ArgumentException ex)
        {
            print(ex);
        }
    }

    int HeroAliveCount
    {
        get
        {
            int aliveCount = 0;
            for (int i = 0; i < playerUnits.Count; i++)
            {
                if (playerUnits[i].IsAlive || playerUnits[i].IsRevive)
                {
                    aliveCount++;
                }
            }
            return aliveCount;
        }
    }

    int MonsterAliveCount
    {
        get
        {
            int aliveCount = 0;
            for (int i = 0; i < monsters.Count; i++)
            {
                if (monsters[i].IsAlive)
                {
                    aliveCount++;
                }
            }
            return aliveCount;
        }
    }

    public int SlotResult(Belong belong)
    {
        int result = 0;

        if (belong == Belong.Player)
            result = playerSlotResult;
        if (belong == Belong.Monster)
            result = monsterSlotResult;
        return result;
    }

    public void AddUnit(Unit unit)
    {
        if(unit.belong == Belong.Player)
        {
            AddHeros(unit);
        }
        else if(unit.belong == Belong.Monster)
        {
            AddMonster(unit);
        }
    }

    public void SelectUnit(Unit unit)
    {
        MasterAudio.PlaySound("UnitClick");
        if (unit.belong == Belong.Player)
        {
            SelectHero(unit);
            GameManager.Instance.activeUnitData = unit.unitData;
        }
        else if (unit.belong == Belong.Monster)
        {
            SelectMonster(unit);
        }
    }

    public void SelectMonster(Unit monster)
    {
        Vector3 markPos = monster.transform.position + (Vector3.right * selectMarkPosition.x) + (Vector3.up * selectMarkPosition.y);
        selected_Monster = monster;

        monsterSelectMark.transform.position = markPos;
        monsterSelectMark.transform.SetParent(monster.transform);
        //monsterSelectMark.transform.localScale = monster.transform.localScale;
    }
    public void SelectHero(Unit unit)
    {
        if (unit.actionEnd)
            return;

        selected_Hero = unit;

        playerSelectMark.transform.position = unit.transform.position + (Vector3.left * selectMarkPosition.x) + (Vector3.up * selectMarkPosition.y);
        battleUI.ActionButtonInteractable(!selected_Hero.actionEnd);
        battleUI.SetUnitData(selected_Hero);
        battleUI.PotionBlockWhenFullHP(selected_Hero);

        if (currentActions.Find(x => x.unit == unit) != null)
            battleUI.ConsumeInteractable(false);
    }

    void AddHeros(Unit hero)
    {
        playerUnits.Add(hero);
        playerSelectBtns[hero.unitData.index].SetUnit(hero);
        if (hero.unitData.index == 0)
        {
            SelectHero(hero);
        }
    } 
    void AddMonster(Unit monster)
    {
        monster.unitData.index = monsters.Count;
        monsters.Add(monster);
        monsterSelectBtns[monster.unitData.index].SetUnit(monster);
        if (monsters.Count <= 1)
        {
            SelectMonster(monster);
        }
    }

    public void NomalAttack()
    {
        AddAction(SelectUnitAction(BattleAction.ATK));
    }

    public void Defense()
    {
        selected_Hero.defenceMode = true;
        AddAction(SelectUnitAction(BattleAction.DEF));
    }

    public void Skill(int index)
    {
        UnitAction unitAction = SelectUnitAction(BattleAction.SKILL);
        unitAction.skillObject = selected_Hero.Skill(index);
        unitAction.indexKey = index;

        if (unitAction.skillObject.SkillEffectType == VisualEffectType.MultiShot || unitAction.skillObject.SkillEffectType == VisualEffectType.FullScale)
        {
            unitAction.targets = monsters;
        }

        AddAction(unitAction);
    }

    public void AddAction(UnitAction unitAction)
    {
        if (unitAction.unit.belong == Belong.Player)
        {
            unitAction.unit.ActionEnd();
            battleUI.ActionButtonInteractable(false);
        }

        if(unitAction.battleAction != BattleAction.DEF)
            unitActions.Add(unitAction);

        if (unitAction.unit.belong == Belong.Player && PlayerUnitsEndedAction())
        {
            if(!unitAction.unit.isCharging || IsAllCharging())
                SlotMachineManager.Instance.BattleSlotStart();
        }

        if (unitAction.battleAction == BattleAction.DEF)
            defenseUnit.Add(unitAction.unit);
    }

    bool IsAllCharging()
    {
        int count = 0;
        for (int i = 0; i < playerUnits.Count; i++)
        {
            if (playerUnits[i].isCharging)
                count++;
        }

        return count == playerUnits.Count;
    }

    UnitAction SelectUnitAction(BattleAction battleAction)
    {
        UnitAction unitAction = new (selected_Hero);

        if (currentActions.Find(x => x.unit == selected_Hero) != null)
            unitAction = currentActions.Find(x => x.unit == selected_Hero);

        unitAction.battleAction = battleAction;

        if (battleAction != BattleAction.DEF || battleAction != BattleAction.ITEM)
        {
            unitAction.targets.Add(selected_Monster);
        }

        return unitAction;
    }

    //TODO : 얘도 위에서 작동되게 변경 필요
    public void UseConsume(int index)
    {
        UnitAction unitAction = SelectUnitAction(BattleAction.ITEM);
        ItemManager.Instance.ReduceItem(battleUI.battleConsumeSlots[index].itemData);
        unitAction.itemData = ItemManager.Instance.GetConsumeItem(index);

        if(unitAction.itemData.GetTotalStat(Stat.HP) <= 0)
        {
            unitAction.unit.BuffItemEffect(ItemManager.Instance.GetConsumeItem(index));
            currentActions.Add(unitAction);
            battleUI.ConsumeInteractable(false);
            battleUI.ConsumeCountUpdate();
        }
        AddAction(unitAction);
    }

    public void SetSlotResult(Belong belong, int slotNumber)
    {
        if (belong == Belong.Player)
        {
            playerSlotResult = slotNumber;
        }
        if (belong == Belong.Monster)
        {
            monsterSlotResult = slotNumber;
        }
    }

    public void MonsterActionExe()
    {
        monsterAI.MonsterAction();
        EndAction();
    }

    bool PlayerUnitsEndedAction()
    {
        for (int i = 0; i < playerUnits.Count; i++)
        {
            if (!playerUnits[i].actionEnd && playerUnits[i].IsAlive)
            {
                return false;
            }
        }

        return true;
    }

    void EndAction()
    {
        unitActions = unitActions.OrderByDescending(x => x.unit.unitData.stat.SPD).ToList();

        for (int i = 0; i < unitActions.Count; i++) 
        {
            actionQueue.Enqueue(unitActions[i]);
        }
        unitActions.Clear();
        ActionStart();
    }

    public void ActionStart()
    {
        escapeBtn.interactable = false;
        ActionSetup();
        DotEffects();
        AllUnitsDefenseEffect();
        battleUI.ConsumeCountUpdate();
        battleUI.ActionButtonInteractable(false);

        TurnDelayStartCheck();
    }
    void ActionSetup()
    {
        playerPercent.SetPercent(playerSlotResult);
        monsterPercent.SetPercent(monsterSlotResult);

        if (HeroAliveCount <= 0 || MonsterAliveCount <= 0)
            actionQueue.Clear();

        playerSelectMark.gameObject.SetActive(false);
        monsterSelectMark.gameObject.SetActive(false);
    }

    void DotEffects()
    {
        for (int i = 0; i < playerUnits.Count; i++)
        {
            if (playerUnits[i].unitData.DotDamageEfffects().Count > 0)
            {
                playerUnits[i].DotDamage();
            }
        }
        for (int i = 0; i < monsters.Count; i++)
        {
            if (monsters[i].unitData.DotDamageEfffects().Count > 0)
            {
                monsters[i].DotDamage();
            }
        }
    }

    void AllUnitsDefenseEffect()
    {
        for (int i = 0; i < defenseUnit.Count; i++)
        {
            if (defenseUnit[i].IsAlive)
            {
                EffectManager.Instance.DefenseEffect(defenseUnit[i].transform);
            }
        }

        if (defenseUnit.Count > 0)
        {
            MasterAudio.PlaySound("Defense");
        }

        defenseUnit.Clear();
    }

    void TurnDelayStartCheck()
    {
        bool isDelay = false;
        for (int i = 0; i < playerUnits.Count; i++)
        {
            if (playerUnits[i].unitData.DotDamageEfffects().Count > 0)
            {
                isDelay = true;
            }
        }
        for (int i = 0; i < monsters.Count; i++)
        {
            if (monsters[i].unitData.DotDamageEfffects().Count > 0)
            {
                isDelay = true;
            }
        }

        if (defenseUnit.Count > 0)
            isDelay = true;
        
        if(isDelay)
            StartCoroutine(TurnStart(1f));
        else
            UnitActionExcution();
    }

    void CheckSlimeKillCount()
    {
        if (GameManager.Instance.stageData.stageConcept == StageConcept.Tutorial)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                SteamAchievements.Instance.AddSlimeKill();
            }
        }
    }
    public void UnitActionExcution()
    {
        if (actionQueue.Count > 0)
        {
            UnitAction unitAction = actionQueue.Dequeue();

            if (unitAction.unit.IsAlive)
                ActionHandle(unitAction);
            else
                UnitActionExcution();
        }
        else
        {
            TurnEnd();
        }
    }
    
    IEnumerator TurnStart(float delay)
    {
        if (delay != 0)
        {
            delay = GameManager.Instance.doubleSpeed ? delay * 0.5f : delay;
        }
        yield return new WaitForSeconds(delay);
        UnitActionExcution();
    }

    void ActionHandle(UnitAction unitAction)
    {
        if (unitAction.CanAction())
        {
            unitAction.unit.SetAction(unitAction);
        }
        else
        {
            UnitActionExcution();
        }
    }


    void TurnEnd()
    {
        playerSelectMark.SetActive(true);
        monsterSelectMark.SetActive(true);
        playerPercent.EndEffect();
        monsterPercent.EndEffect();
        SetUnitEndAction();
        
        TurnStart();
    }

    void SetUnitEndAction()
    {
        currentActions.Clear();
        for (int i = 0; i < playerUnits.Count; i++)
        {
            if (playerUnits[i].IsAlive || playerUnits[i].IsRevive)
                playerUnits[i].TurnEnd();
        }
        for (int i = 0; i < monsters.Count; i++)
        {
            if(monsters[i].IsAlive)
                monsters[i].TurnEnd();
        }
    }

    [ContextMenu("Turn Start")]
    public void TurnStart()
    {
        battleUI.ActionButtonInteractable(!selected_Hero.actionEnd);
        SelectHero(selected_Hero);
        
        SelectChange();
        AllUnitDefenseModeClear();

        BattleEnd();
    }

    void SelectChange()
    {
        HeroSelectChange();
        MonsterSelectChange();
    }
    void HeroSelectChange()
    {
        if (!selected_Hero.IsAlive)
        {
            for (int i = 0; i < playerUnits.Count; i++)
            {
                if (playerUnits[i].IsAlive)
                {
                    SelectHero(playerUnits[i]);
                    break;
                }
            }
        }
    }
    void MonsterSelectChange()
    {
        if (!selected_Monster.IsAlive)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                if (monsters[i].IsAlive)
                {
                    SelectMonster(monsters[i]);
                    break;
                }
            }
        }
    }

    void AllUnitDefenseModeClear()
    {
        for(int i = 0; i < playerUnits.Count; i++)
        {
            playerUnits[i].defenceMode = false;
        }

        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].defenceMode = false;
        }
    }

    void BattleEnd()
    {
        if (MonsterAliveCount <= 0)
        {
            CheckSlimeKillCount();
            monsterSelectMark.SetActive(false);
            playerSelectMark.SetActive(false);
            Win();
            GameManager.Instance.ResetStatusEffect();
        }
        if (HeroAliveCount <= 0)
        {
            monsterSelectMark.SetActive(false);
            playerSelectMark.SetActive(false);
            Lose();
        }

        escapeBtn.interactable = true;
    }

    void Win()
    {
        GameManager.Instance.RoomClear();

        if (GameManager.Instance.currentPlayerRoom.roomConcept == RoomConcept.BOSS)
        {
            if (GameManager.Instance.stageData.stageConcept == StageConcept.Final)
            {
                GameManager.Instance.EndingPopupOn();
                SteamAchievements.Instance.GameClearAchievement();
            }
            else
            {
                MasterAudio.PlaySound("StageClear");
                MasterAudio.StopAllPlaylists();
                rewardController.SetRewardPopupOn(1);
            }
        }
        else
        {
            MasterAudio.PlaySound("Win");
            rewardController.SetRewardPopupOn();
        }
    }


    void Lose()
    {
        battleUI.LoseUI();
    }

    public void DoubleSpeed()
    {
        GameManager.Instance.doubleSpeed = !GameManager.Instance.doubleSpeed;
        battleUI.SetDoubleSpeedBtnImage();
    }

    void Escape()
    {
        bool isSuccess = Random.Range(0, 2) == 0;
        EscapeEffect();

        if (isSuccess)
        {
            StartCoroutine(BackRoom());
        }
        else
        {
            unitActions.Clear();
            AllMissEffect();
            StartCoroutine(BattleSlotStart());
        }

        escapeBtn.interactable = false;
    }

    void EscapeEffect()
    {
        for (int i = 0; i < playerUnits.Count; i++)
        {
            EffectManager.Instance.EscapeEffect(playerUnits[i].transform);
        }
    }

    void AllMissEffect()
    {
        for (int i = 0; i < playerUnits.Count; i++)
        {
            EffectManager.Instance.MissEffect(playerUnits[i].transform);
        }
    }

    IEnumerator BackRoom()
    {
        float delay = GameManager.Instance.doubleSpeed ? 0.7f : 1.5f;
        yield return new WaitForSeconds(delay);
        GameManager.Instance.BackRoom();
    }
    IEnumerator BattleSlotStart()
    {
        float delay = GameManager.Instance.doubleSpeed ? 0.7f : 1.5f;
        yield return new WaitForSeconds(delay);
        SlotMachineManager.Instance.BattleSlotStart();
    }
}