using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using DarkTonic.MasterAudio;

public class EffectManager : MonoBehaviour
{
    private static EffectManager instance;

    public static EffectManager Instance
    {
        get { return instance; }
    }

    [Header("Text Effects")]
    [SerializeField] private GameObject missEffect;
    [SerializeField] private TMP_Text numberText;

    [Header("Skill Effects")]
    [SerializeField] private GameObject defenseEffectPrefab;
    [SerializeField] private GameObject healPotionEffectPrefab;
    [SerializeField] private GameObject buffPotionEffectPrefab;
    [SerializeField] private List<GameObject> buffAuraPrefabs;
    [SerializeField] private GameObject damagedEffect;
    [SerializeField] private GameObject reviveEffect;
    [SerializeField] private GameObject escapeEffect;
    [SerializeField] private GambleSkillEffect gambleEffect;


    UnitAction currentAction;
    float DoubleSpeed(float baseSpeed)
    {
        float speed = baseSpeed;
        if (GameManager.Instance != null)
        {
            speed = GameManager.Instance.doubleSpeed ? speed / 2 : speed;
        }

        return speed;
    }
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void MissEffect(Transform target)
    {
        GameObject effect = Instantiate(missEffect);
        effect.transform.position = target.position + (Vector3.up * 0.5f);
        MasterAudio.PlaySound("Miss");
        Destroy(effect, 4f);
    }

    public void DefenseEffect(Transform unitTransform)
    {
        GameObject effect = Instantiate(defenseEffectPrefab);
        effect.transform.position = unitTransform.position;

        Destroy(effect, 4f);
    }

    public void EscapeEffect(Transform unitTransform)
    {
        GameObject effect = Instantiate(escapeEffect);
        effect.transform.position = unitTransform.position;

        Destroy(effect, 4f);
    }

    public void NumberEffect(float number, Vector3 position)
    {
        TMP_Text textEffect = Instantiate(numberText, GameManager.Instance.FrontCanvas.transform);

        textEffect.text = number.ToString();
        textEffect.transform.position = position + Vector3.up;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(textEffect.transform.DOMoveY(textEffect.transform.position.y + 1f, 1f));
        sequence.Append(textEffect.DOFade(0, 0.5f));
        MasterAudio.PlaySound("Hit");
        Destroy(textEffect.gameObject, 2f);
    }

    public void ReviveEffect(Transform unitTransform)
    {
        GameObject effect = Instantiate(reviveEffect);
        effect.transform.position = unitTransform.position;

        Destroy(effect, 4f);
    }


    #region 스킬 이펙트 모음

    public GameObject InstantiateHealEffect(Transform unitTransform, int sortingOrder = 1000)
    {
        GameObject effect = Instantiate(healPotionEffectPrefab, unitTransform);
        effect.transform.localPosition = Vector3.zero;
        effect.GetComponent<ParticleSystemRenderer>().sortingOrder = sortingOrder;

        return effect;
    }

    public GameObject InstantiateBuffPotionEffect(Transform unitTransform, int sortingOrder = 1000)
    {
        GameObject effect = Instantiate(buffPotionEffectPrefab, unitTransform);
        effect.GetComponent<ParticleSystemRenderer>().sortingOrder = sortingOrder;

        return effect;
    }

    public GameObject InstantiateAura(Transform unitTransform, StatusEffect effect = null)
    {
        try
        {
            int effectIndex = Mathf.CeilToInt(effect.grade / 2.0f);
            GameObject aura = Instantiate(buffAuraPrefabs[effectIndex], unitTransform);
            return aura;
        }
        catch(System.NullReferenceException ex)
        {
            print(ex);
            return Instantiate(buffAuraPrefabs[0], unitTransform);
        }
    }

    public GameObject DamagedEffect(Transform unitTransform)
    {
        GameObject effect = Instantiate(damagedEffect);
        effect.transform.position = unitTransform.position;
        Destroy(effect, 4f);

        return effect;
    }

    public void GambleSkillEffect(UnitAction unitAction)
    {
        float time = GameManager.Instance.doubleSpeed ? 1f : 2f;
        GambleSkillEffect effect = Instantiate(gambleEffect, unitAction.unit.transform);
        effect.transform.localPosition = Vector3.up * 0.3f;
        effect.SetEffectInfo(time, unitAction);
    }
    #endregion

    #region 전투중 이펙트 모음

    void ActionOnComplite()
    {
        float delay = DoubleSpeed(currentAction.skillObject.disapearTime) + currentAction.skillObject.effectDelay;
        StartCoroutine(ActionDelay(delay, currentAction));
    }
    IEnumerator ActionDelay(float delay, UnitAction currentAction)
    {
        yield return new WaitForSeconds(delay);

        if(!currentAction.unit.isCharging)
            currentAction.TargetReact();

        currentAction.unit.PlayAnimation("Idle");
        yield return new WaitForSeconds(DoubleSpeed(1.5f));
        BattleManager.instance?.UnitActionExcution();
    }

    public void SkillDamagedEffect(SkillObject skillObject, Transform targetTransform)
    {
        SkillParticle skillParticle = Instantiate(skillObject.effect, targetTransform);
        skillParticle.transform.localPosition = Vector3.zero;
        skillParticle.HitImpact();

        if (skillObject.Contains(SkillTag.Debuff) || skillObject.Contains(SkillTag.Buff))
        {
            skillParticle.OnHitEffect(targetTransform, skillObject);
        }
    }

    public void EffectHandle(UnitAction unitAction)
    {
        currentAction = unitAction;
        switch (currentAction.skillObject.SkillEffectType)
        {
            case VisualEffectType.Range:
            case VisualEffectType.MultiShot:
                RangeSkillEffect();
                break;
            case VisualEffectType.FullScale:
                FullScaleEffect();
                break;
            case VisualEffectType.Curve:
                CurveShotEffect();
                break;
            case VisualEffectType.Buff:
                BuffSkillEffect();
                break;
            case VisualEffectType.ChargedShot:
                if (currentAction.unit.isCharging)
                    ChargeShot();
                else
                    ChargeEffect();
                break;
        }
    }

    void RangeSkillEffect()
    {
        SkillParticle skillParticle = Instantiate(currentAction.skillObject.effect);
        skillParticle.transform.position += currentAction.skillObject.effectPosition;

        float effectDelay = currentAction.skillObject.effectDelay;
        float disapearTime = currentAction.skillObject.disapearTime;

        for (int i = 0; i < currentAction.targets.Count; i++)
        {
            if (currentAction.targets[i].IsAlive)
            {
                GameObject missleObj = skillParticle.MissileObj(currentAction.unit.transform.position, currentAction.skillObject);
                Sequence sequence = DOTween.Sequence()
                .Append(missleObj.transform.DOMove(currentAction.targets[i].transform.position + new Vector3(0, 0.5f, 0), DoubleSpeed(disapearTime)).SetDelay(effectDelay))
                .OnComplete(() =>
                {
                    skillParticle.ExplosionEffect(missleObj.transform);
                });
            }
        }

        ActionOnComplite();
    }

    void CurveShotEffect()
    {
        SkillParticle skillParticle = Instantiate(currentAction.skillObject.effect, transform);
        GameObject missleObj = skillParticle.MissileObj(skillParticle.transform.position, currentAction.skillObject);
        missleObj.transform.position = currentAction.unit.transform.position + currentAction.skillObject.effectPosition;

        float duration = currentAction.skillObject.disapearTime;
        if (GameManager.Instance.doubleSpeed)
            duration = duration * 0.5f;

        for (int i = 0; i < currentAction.targets.Count; i++)
        {
            if (currentAction.targets[i].IsAlive)
            {
                Vector3[] path = CurvePath(currentAction.targets[i].transform);

                // DOTween의 동작
                missleObj.transform.DOPath(path, duration, PathType.CatmullRom)
                    .SetEase(Ease.InSine) // 이동에 사용할 곡선 타입 (조절 가능)
                    .OnComplete(() =>
                    {
                        skillParticle.ExplosionEffect(missleObj.transform);
                        currentAction.unit.PlayAnimation("Idle");
                        Destroy(missleObj);
                    });
            }
        }
        ActionOnComplite();
    }

    Vector3[] CurvePath(Transform destination)
    {
        float dir = Mathf.Sign(destination.position.x - transform.position.x);
        Vector3 startPos = currentAction.unit.transform.position + (dir * Vector3.right) + Vector3.up;
        Vector3 throughPoint = ((startPos + destination.position) * 0.5f) + (Vector3.up * 3f);

        // 목적지까지의 곡선 경로 설정
        Vector3[] path = new Vector3[] {
            startPos,
            throughPoint, // 중간 지점 (조절 가능)
            destination.position
        };

        return path;
    }

    void FullScaleEffect()
    {
        SkillParticle skillParticle = Instantiate(currentAction.skillObject.effect, transform);
        skillParticle.FullScaleEffect(currentAction);
        ActionOnComplite();
    }

    public void BuffSkillEffect()
    {
        if (currentAction.skillObject.Contains(SkillTag.Buff))
        {
            currentAction.targets.Clear();
            currentAction.targets.Add(currentAction.unit); 
        }
        
        for (int i = 0; i < currentAction.targets.Count; i++)
        {
            SkillParticle skillParticle = Instantiate(currentAction.skillObject.effect, currentAction.targets[i].transform);
            skillParticle.BuffSkill(currentAction.targets[i].transform, currentAction.skillObject);
        }
        MasterAudio.PlaySound("BuffSound");

        ActionOnComplite();
    }

    void ChargeEffect()
    {
        currentAction.unit.ChargeShotSkill();
        ActionOnComplite();
    }

    void ChargeShot()
    {
        RangeSkillEffect();
        currentAction.unit.DestroyChargeEffect();
    }
    #endregion
}
