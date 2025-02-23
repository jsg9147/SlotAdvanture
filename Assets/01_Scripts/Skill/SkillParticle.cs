using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SkillParticle : MonoBehaviour
{
    public GameObject impactParticle; // Effect spawned when projectile hits a collider
    public GameObject projectileParticle; // Effect attached to the gameobject as child
    public GameObject muzzleParticle; // Effect instantly spawned when gameobject is spawned
    public GameObject persistentParticle;

    public float hitDelay;

    GameObject projectileParticleObj;
    GameObject chargedEffect;

    Vector3 startPosition;
    private void Start()
    {

    }

    public void MeleeEffect(UnitAction unitAction)
    {
        try
        {
            GameObject impactP = Instantiate(projectileParticle);
            impactP.transform.position = unitAction.unit.transform.position + unitAction.skillObject.effectPosition;
            ParticleDoubleSpeed(impactP);
            MuzzleInstantiate(unitAction.unit.transform.position);
            Destroy(impactP, 3f);
            Destroy(gameObject, 3f);
        }
        catch (System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    public void HitImpact()
    {
        if (impactParticle != null)
        {
            GameObject impactP = Instantiate(impactParticle);
            impactP.transform.position = transform.position;
            Destroy(impactP, 3f);
        }

        Destroy(gameObject, 3f);
    }

    public GameObject MissileObj(Vector3 startPosition, SkillObject skillObject)
    {
        projectileParticleObj = Instantiate(projectileParticle, startPosition + (Vector3.up * 0.5f), transform.rotation);
        ParticleDoubleSpeed(projectileParticleObj);
        projectileParticleObj.transform.localRotation = skillObject.effectRotate;
        MuzzleInstantiate(startPosition, skillObject.disapearTime);

        if (chargedEffect != null)
            Destroy(chargedEffect.gameObject);

        return projectileParticleObj;
    }


    // Missile Explosion
    public void ExplosionEffect(Transform missleObj)
    {
        GameObject impactP = Instantiate(impactParticle);
        impactP.transform.position = missleObj.transform.position;

        ParticleDoubleSpeed(impactP);

        if (chargedEffect != null)
        {
            Destroy(chargedEffect);
        }
        Destroy(projectileParticleObj); // Removes particle effect after delay
        Destroy(impactP, 3f);
        Destroy(missleObj.gameObject);
        Destroy(gameObject); // Removes the projectile
    }

    public void DestroyChargeEffect()
    {
        if (chargedEffect != null)
        {
            Destroy(chargedEffect);
        }
        Destroy(gameObject); // Removes the projectile
    }

    public void BuffSkill(Transform unitTransform, SkillObject skillObject)
    {
        projectileParticleObj = Instantiate(projectileParticle, unitTransform.position, Quaternion.FromToRotation(Vector3.up, transform.position));
        projectileParticleObj.transform.SetParent(unitTransform);
        projectileParticleObj.transform.localPosition = skillObject.effectPosition + (Vector3.down * 0.5f);
        projectileParticleObj.transform.rotation = skillObject.effectRotate;

        MuzzleInstantiate(unitTransform.position);

        ParticleDoubleSpeed(projectileParticleObj);
        Destroy(projectileParticleObj, DoubleSpeedCal(3f));
        Destroy(gameObject, DoubleSpeedCal(3f));
    }

    public void OnHitEffect(Transform unitTransform, SkillObject skillObject)
    {
        projectileParticleObj = Instantiate(persistentParticle, unitTransform.position, Quaternion.FromToRotation(Vector3.up, transform.position));
        projectileParticleObj.transform.SetParent(unitTransform);
        projectileParticleObj.transform.localPosition = (Vector3.up * 0.5f);
        projectileParticleObj.transform.rotation = skillObject.effectRotate;

        //MuzzleInstantiate();

        ParticleDoubleSpeed(projectileParticleObj);
        Destroy(projectileParticleObj, DoubleSpeedCal(3f));
        Destroy(gameObject, DoubleSpeedCal(3f));
    }

    public void StatusEffect(Transform unitTransform)
    {
        projectileParticleObj = Instantiate(persistentParticle, unitTransform);

        ParticleDoubleSpeed(projectileParticleObj);
        Destroy(projectileParticleObj, DoubleSpeedCal(3f));
        Destroy(gameObject, DoubleSpeedCal(3f));
    }

    public void FullScaleEffect(UnitAction unitAction)
    {
        startPosition = unitAction.skillObject.effectPosition;

        projectileParticleObj = Instantiate(projectileParticle, startPosition, transform.rotation) as GameObject;
        projectileParticleObj.transform.rotation = unitAction.skillObject.effectRotate;
        MuzzleInstantiate(unitAction.unit.transform.position, unitAction.skillObject.disapearTime);

        //StartCoroutine(HitEffectDelay(unitAction));
    }

    public void ChargeEffect(UnitAction unitAction)
    {
        Transform unitTransform = unitAction.unit.transform;
        SkillObject skillObject = unitAction.skillObject;

        chargedEffect = Instantiate(persistentParticle, unitTransform.position + (Vector3.up) , Quaternion.FromToRotation(Vector3.up, transform.position));
        chargedEffect.transform.SetParent(unitTransform);
        chargedEffect.transform.rotation = skillObject.effectRotate;
    }
    public void DestroySelf()
    {
        Destroy(projectileParticleObj);
        Destroy(this);
    }

    void ParticleDoubleSpeed(GameObject effect)
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.doubleSpeed)
            {
                ParticleSystem[] particleSystems = effect.GetComponentsInChildren<ParticleSystem>();

                for (int i = 0; i < particleSystems.Length; i++)
                {
                    var main = particleSystems[i].main;
                    main.simulationSpeed = main.simulationSpeed * 2;
                }
            }
        }
    }


    float DoubleSpeedCal(float disapearTime)
    {
        float speed = disapearTime;
        if (GameManager.Instance != null)
        {
            speed = GameManager.Instance.doubleSpeed ? speed / 2 : speed;
        }

        return speed;
    }

    void MuzzleInstantiate(Vector3 startPosition, float disapearTime = 2)
    {
        if (muzzleParticle)
        {
            GameObject muzzleParticleObj = Instantiate(muzzleParticle, transform.position + (Vector3.up * 0.5f), muzzleParticle.transform.rotation);
            ParticleDoubleSpeed(muzzleParticleObj);
            Destroy(muzzleParticleObj, DoubleSpeedCal(disapearTime)); // 2nd parameter is lifetime of effect in seconds
        }
    }
}
