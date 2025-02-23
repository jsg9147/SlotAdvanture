using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;

public class GambleSkillEffect : MonoBehaviour
{
    public SpriteRenderer[] cardSprites;
    public float timeBetweenChanges = 1.0f;

    private int currentSpriteIndex = 0;
    private float timeSinceLastChange = 0.0f;

    private float changeTime;
    private int resultSpriteIndex;

    bool changeStart = true;

    UnitAction actionInfo;

    private void Start()
    {
        if (GameManager.Instance.doubleSpeed)
            timeBetweenChanges = timeBetweenChanges * 0.5f;
    }

    private void FixedUpdate()
    {
        if (changeStart)
        {
            SpriteChange();
        }
    }

    public void SetEffectInfo(float time, UnitAction unitAction)
    {
        actionInfo = unitAction;
        changeTime = time + Random.Range(0, 0.5f);
        resultSpriteIndex = Random.Range(0, cardSprites.Length);
        actionInfo.gambleResult = resultSpriteIndex;
        changeStart = true;
    }

    void SpriteChange()
    {
        if (changeTime < 0 && resultSpriteIndex == currentSpriteIndex)
        {
            float time = 1f;
            changeStart = false;
            StartCoroutine(BattleAction(time));
            
            return;
        }

        timeSinceLastChange += Time.deltaTime;

        if (timeSinceLastChange >= timeBetweenChanges)
        {
            // 다음 스프라이트로 변경
            currentSpriteIndex = (currentSpriteIndex + 1) % cardSprites.Length;
            SpriteAlphaChange();
            timeSinceLastChange = 0.0f;
            MasterAudio.PlaySound("Card");
        }

        changeTime -= Time.deltaTime;
    }

    IEnumerator BattleAction(float time)
    {
        yield return new WaitForSeconds(time);
        actionInfo.unit.BateleAction();
        Destroy(gameObject);
    }

    void SpriteAlphaChange()
    {
        for (int i = 0; i < cardSprites.Length; i++)
        {
            cardSprites[i].color = (currentSpriteIndex == i) ? new(1, 1, 1, 1) : new Color(1, 1, 1, 0.5f);
            cardSprites[i].sortingOrder = (currentSpriteIndex == i) ? 3 : i;
        }
    }
}
