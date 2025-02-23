using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MoveExplosion : MonoBehaviour
{
    public float dropTime;

    public Transform moveObj;
    public Transform trailEffect;
    public GameObject explotionParticle;

    public float amplitude = 1f;  // 운동의 진폭
    public float frequency = 1f;  // 운동의 주파수
    private Vector3 startPosition;
    private void Start()
    {
        startPosition = trailEffect.localPosition;
        Effect();
    }

    private void FixedUpdate()
    {
        TrailEffect();
    }

    void Effect()
    {
        float speed = GameManager.Instance.doubleSpeed ? dropTime / 2 : dropTime;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(moveObj.DOLocalMove(Vector2.zero, speed));
        sequence.OnComplete(() => Instantiate(explotionParticle, transform));

        sequence.Play();
        Destroy(gameObject, dropTime + 0.5f);
    }
    void TrailEffect()
    {
        // 시간에 따라 물체의 높이를 변경
        float yOffset = (amplitude * Mathf.Sin(frequency * Time.time) * 2);

        // 새로운 높이를 적용하여 물체의 위치를 업데이트
        trailEffect.localPosition = startPosition + new Vector3(0f, yOffset, 0f);
    }
}
