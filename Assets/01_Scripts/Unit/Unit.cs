using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using DarkTonic.MasterAudio;
public enum Belong
{
    Player,
    Monster,
}
[RequireComponent(typeof(BoxCollider2D))]
public class Unit : MonoBehaviour
{
    private Vector3 originPos;
    private float currentHP;

    public Belong belong;
    public MonsterData monsterData;
    [HideInInspector]
    public BuffSlotController buffSlotController;

    //TODO : 다른식으로 hide 안하게 할만한게 있는치 확인할것
    [HideInInspector] public bool isReinforced = false;
    [HideInInspector] public bool actionEnd;
    [HideInInspector] public bool defenceMode;
    [HideInInspector] public GameObject endMark;

    GameObject sanctuaryEffect;

    public UnitData unitData = new UnitData();

    ProgressBarPro _hpBar;

    Animator animator;

    List<SkillObject> skills;

    UnitAction currentAction;

    SkillParticle currentParticle;

    [HideInInspector]
    public bool isCharging = false;

    AudioSource audioSource;
    float DoubleSpeed(float baseSpeed)
    {
        float speed = baseSpeed;
        if (GameManager.Instance != null)
        {
            speed = GameManager.Instance.doubleSpeed ? speed / 2 : speed;
        }

        return speed;
    }

    public bool IsAlive
    {
        get 
        {
            bool alive = (currentHP > 0);
            return alive; 
        }
    }
    public bool IsRevive
    {
        get
        {
            return unitData.sanctuarySkill != null;
        }
    }

    private void Start()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = GameManager.Instance.SFXAudioMixerGroup();
    }

    private void OnMouseExit()
    {
        UIController.Instance.PopupOff();
    }

    public void SetSanctuaryEffect(GameObject effect) => sanctuaryEffect = effect;

    public void SetBuffController(BuffSlotController buffSlotController)
    {
        this.buffSlotController = buffSlotController;
        buffSlotController.transform.SetParent(transform);
        buffSlotController.transform.localPosition = new Vector3(-1f, 0.6f, 0);
        if (belong == Belong.Player)
            buffSlotController.transform.localPosition = new(-0.7f,0.5f,0);
    }

    public void Init(UnitData unitData)
    {
        SetUnitData(unitData);
        animator = GetComponent<Animator>();
        originPos = transform.position;

        if (GameManager.Instance.isGameStart && belong == Belong.Player)
        {
            currentHP = unitData.currentHP;
            
            if (currentHP <= 0)
            {
                Death();
            }
        }   
        else
        {
            currentHP = unitData.stat.HP;
        }
    }

    public void SetHpBar(ProgressBarPro hpBar)
    {
        _hpBar = hpBar;
        _hpBar.barViewValueText.maxValue = unitData.stat.HP;
        _hpBar.Value = currentHP;

        if(belong == Belong.Player)
        {
            _hpBar.SetValue(currentHP / unitData.stat.HP, true);
        }
    }

    void SetUnitData(UnitData unitData)
    {
        this.unitData = unitData;
        skills = unitData.skills;
    }
    public void SetAction(UnitAction unitAction)
    {
        currentAction = unitAction;
        if(!unitData.gambleConcept)
            BateleAction();
        else
            EffectManager.Instance.GambleSkillEffect(unitAction);
    }

    public SkillObject Skill(int index) => skills[index];

    public void TurnEnd()
    {
        actionEnd = false;

        if (isCharging)
        {
            SetActionChargedShot();
        }
        else
        {
            currentAction = null;
            if (endMark)
                endMark.SetActive(false);
        }
        unitData.TurnEnd();

        if(!IsAlive)
            Revival();
        buffSlotController.SetBuffSlot(unitData.statusEffects);
    }

    public void ActionEnd()
    {
        actionEnd = true;
        endMark.SetActive(true);
    }

    public void PlayAnimation(string clipName)
    {
        if(animator == null)
            animator = GetComponent<Animator>();

        try
        {
            animator.speed = GameManager.Instance.doubleSpeed ? 2 : 1;
            animator.SetTrigger(clipName);
        }
        catch (System.NullReferenceException ex)
        {
            Debug.Log(ex);
        }
    }
    public void PlayAnimation(string clipName, bool isSetBool)
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        try
        {
            if(SceneManager.GetActiveScene().name != "MAP")
                animator.speed = GameManager.Instance.doubleSpeed ? 2 : 1;
            animator.SetBool(clipName, isSetBool);
        }
        catch (System.NullReferenceException ex)
        {
            Debug.Log(ex);
        }
    }

    public void Damaged(float damage)
    {
        float finalDamage = damage;
        PlayAnimation("Damaged");
        transform.DOShakePosition(0.3f, strength: new Vector3(0.5f, 0, 0), 10, 45f);

        EffectManager.Instance.NumberEffect((int)finalDamage, transform.position);
        PlaySound(unitData.soundGroup.damaged);

        currentHP = currentHP - finalDamage;

        _hpBar.SetValue(currentHP / unitData.stat.HP);

        if (belong == Belong.Player)
        {
            unitData.ChangeCurrentHP(-finalDamage);
        }
        if (currentHP <= 0)
        {
            Death();
        }
        else
        {
            PlayAnimation("Idle");
            //StartCoroutine(IdleAnimation());
        }
        GameObject hitEffect = EffectManager.Instance.DamagedEffect(transform);
        Vector3 effectPos = (belong == Belong.Monster) ? (Vector3.left * 1.5f) + (Vector3.up * 0.5f) : (Vector3.right * 1.5f) + (Vector3.up * 0.5f);
        hitEffect.transform.position = hitEffect.transform.position + effectPos;
    }

    public void DotDamage()
    {
        List<StatusEffect> effects = unitData.DotDamageEfffects();
        float damage = 0f;
        for (int i = 0; i < effects.Count; i++)
        {
            SkillParticle skillParticle = Instantiate(effects[i].effectParticle, transform);
            skillParticle.StatusEffect(transform);
            damage += effects[i].statValues[Stat.HP];
        }

        if (damage > 0f)
            Damaged(damage);
    }

    public bool IsDotDamage() => unitData.DotDamageEfffects().Count > 0;

    void Death()
    {
        PlayAnimation("isDie", true);
    }

    void Revival()
    {
        if (IsRevive)
        {
            StatusEffect sanctuaryEffect = unitData.sanctuarySkill;
            currentHP = unitData.stat.HP * 0.5f;
            _hpBar.SetValue(currentHP / unitData.stat.HP);
            EffectManager.Instance.ReviveEffect(transform);
            PlayAnimation("isDie", false);
            if (belong == Belong.Player)
            {
                unitData.currentHP = currentHP;
            }

            unitData.statusEffects.Remove(unitData.sanctuarySkill);
            unitData.sanctuarySkill = null;
            this.sanctuaryEffect.SetActive(false);
            MasterAudio.PlaySound("Revival");
        }
    }


    public void MoveToTarget()
    {
        for (int i = 0; i < currentAction.targets.Count; i++)
        {
            Vector3 movePosition = currentAction.targets[i].transform.position;

            movePosition = (belong == Belong.Player) ? movePosition + (Vector3.left * 2.5f) : movePosition - (Vector3.left * 2.5f);
            PlayAnimation("isWalk", true);
            PlaySound(unitData.soundGroup.move);
            TrailEffect();

            Sequence sequence = DOTween.Sequence()
                .Append(transform.DOMove(movePosition, DoubleSpeed(MoveTime(transform.position, movePosition))))
                .OnComplete(() =>
                {
                    PlayAnimation("isWalk", false);
                    PlayAnimation("Attack");
                });
        }
    }

    float MoveTime(Vector3 a, Vector3 b)
    {
        float speed = 8f;
        float distance = Vector3.Distance(a, b);
        float time = distance / speed;

        return time;
    }

    void TrailEffect()
    {
        if (currentAction.skillObject != null)
        {
            currentParticle = Instantiate(currentAction.skillObject.effect, transform);
            if (currentParticle.muzzleParticle != null && currentAction.unit.belong == Belong.Monster)
            {
                GameObject trail = Instantiate(currentParticle.muzzleParticle);
                trail.transform.position = transform.position;
                trail.GetComponent<ChaseUnit>()?.SetUnit(transform);
                Destroy(trail, DoubleSpeed(1.3f));
            }
        }
    }

    public void BateleAction()
    {
        try
        {
            switch (currentAction.battleAction)
            {
                case BattleAction.ATK:
                    MoveToTarget();
                    break;
                case BattleAction.SKILL:
                    if(currentAction.skillObject.SkillEffectType == VisualEffectType.Melee)
                        MoveToTarget();
                    else
                        PlayAnimation("Skill");
                    break;
                case BattleAction.ITEM:
                    UseItem();
                    break;
            }
        }
        catch(System.NullReferenceException ex)
        {
            print($"{currentAction.battleAction}\n{ex}");
        }
    }

    

    public void UnitAttackMotion()
    {
        if (transform.position != originPos)
        {
            MeleeEffect();
        }
        else
        {
            if (currentAction.battleAction == BattleAction.SKILL)
                SkillActive();
        }
    }

    void SkillActive()
    {
        SkillUsedPP(currentAction.indexKey);
        PlaySound(unitData.soundGroup.skill);
        EffectManager.Instance.EffectHandle(currentAction);
    }

    void MeleeEffect()
    {
        if (currentAction.battleAction != BattleAction.ATK)
        {
            currentParticle.MeleeEffect(currentAction);
            PlaySound(unitData.soundGroup.skill);
            StartCoroutine(DamageDelay(currentAction));
            SkillUsedPP(currentAction.indexKey);
        }
        else
        {
            PlaySound(unitData.soundGroup.attack);
            currentAction.TargetReact();
        }

        if(currentParticle != null)
            Destroy(currentParticle.gameObject);

        currentParticle = null;
        StartCoroutine(MoveBack(currentAction));
    }

    IEnumerator DamageDelay(UnitAction unitAction)
    {
        float delay = 0;
        if (unitAction.skillObject)
            delay = unitAction.skillObject.effectDelay;
        yield return new WaitForSeconds(delay);
        currentAction.TargetReact();
    }

    IEnumerator MoveBack(UnitAction unitAction)
    {
        float delay = 1;
        if (unitAction.skillObject)
            delay = unitAction.skillObject.effectDelay + 1f;

        yield return new WaitForSeconds(DoubleSpeed(delay));
        Sequence sequence = DOTween.Sequence().OnStart(() =>
        {
            PlayAnimation("isWalk", true);
        })
            .Append(transform.DOMove(originPos, DoubleSpeed(MoveTime(transform.position, originPos))))
            .OnComplete(() =>
            {
                PlayAnimation("isWalk", false);
                PlayAnimation("Idle");
                StartCoroutine(ActionDelay(DoubleSpeed(1.5f)));
            });
    }


    void SetActionChargedShot()
    {
        //int index = skills.FindIndex(x => x == currentAction.skillObject);
        //BattleManager.instance.SelectHero(this);
        //BattleManager.instance.Skill(index);
        BattleManager.instance.AddAction(currentAction);
        endMark.SetActive(true);
    }

    public void ChargeShotSkill()
    {
        isCharging = true;
        currentParticle = Instantiate(currentAction.skillObject.effect, transform);
        currentParticle.ChargeEffect(currentAction);
    }

    public void DestroyChargeEffect()
    {
        isCharging = false;
        currentParticle.DestroyChargeEffect();
    }

    IEnumerator ActionDelay(float delay = 1.5f)
    {
        yield return new WaitForSeconds(DoubleSpeed(delay));
        PlayAnimation("Idle");
        BattleManager.instance?.UnitActionExcution();
    }


    public void AddBuffData(StatusEffect effect)
    {
        if (effect.GetEffectStat(Stat.HP) > 0 && !effect.isDebuff)
        {
            HealEffect(effect.GetEffectStat(Stat.HP));
        }
        else
        {
            unitData.AddEffect(effect);
            buffSlotController.SetBuffSlot(unitData.statusEffects);
        }
    }

    public void UseItem()
    {
        if (currentAction.itemData.GetTotalStat(Stat.HP) != 0)
            HealItemEffect();

        StartCoroutine(ActionDelay());
    }

    void HealItemEffect()
    {
        ItemData itemData = currentAction.itemData;
        try
        {
            if (itemData.GetTotalStat(Stat.HP) != 0)
            {
                if (currentHP < unitData.stat.HP)
                {
                    int value = (int)(unitData.stat.HP * 0.01f * itemData.GetTotalStat(Stat.HP));

                    if(belong == Belong.Player)
                        unitData.ChangeCurrentHP(value);

                    currentHP += value;
                    if (currentHP > unitData.stat.HP)
                        currentHP = unitData.stat.HP;
                    _hpBar.SetValue(currentHP / unitData.stat.HP);

                    GameObject healEffect = EffectManager.Instance.InstantiateHealEffect(transform);
                    healEffect.transform.localPosition = new Vector3(0, 0.5f, 0);
                    healEffect.transform.localScale = Vector3.one * 2f;
                }
            }
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    void HealEffect(float value)
    {
        try
        {
            if (currentHP < unitData.stat.HP)
            {
                if (belong == Belong.Player)
                    unitData.ChangeCurrentHP(value);

                currentHP += value;
                _hpBar.SetValue(currentHP / unitData.stat.HP);
            }
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    public void BuffItemEffect(ItemData itemData)
    {
        StatusEffect statusEffect = new StatusEffect(itemData);

        unitData.AddEffect(statusEffect);

        GameObject effect = EffectManager.Instance.InstantiateBuffPotionEffect(transform);
        effect.transform.localScale = new(Mathf.Sign(transform.localScale.x) * 0.5f, Mathf.Sign(transform.localScale.y) * 1.5f, Mathf.Sign(transform.localScale.z) * 1.5f);
    }

    public void SkillUsedPP(int index)
    {
        if (belong == Belong.Player)
        {
            unitData.ReduceSkillPP(index);
        }
    }

    public void WalkSound()
    {
        PlaySound(unitData.soundGroup.move);
    }

    void PlaySound(AudioClip audioClip)
    {
        try
        {
            if (audioClip != null)
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            }
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    public void LayerChange(string layerName)
    {
        ChangeSortingLayerRecursively(transform, layerName);
    }

    void ChangeSortingLayerRecursively(Transform parentTransform, string sortingLayer)
    {
        // 현재 부모 객체의 모든 자식을 확인
        foreach (Transform child in parentTransform)
        {
            // 자식 객체에서 SpriteRenderer 컴포넌트 찾기
            SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();

            // SpriteRenderer가 있으면 Sorting Layer 변경
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingLayerName = sortingLayer;
            }

            // 재귀적으로 하위의 하위 객체에도 적용
            ChangeSortingLayerRecursively(child, sortingLayer);
        }
    }
}
