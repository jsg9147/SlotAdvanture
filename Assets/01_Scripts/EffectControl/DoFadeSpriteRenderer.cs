using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoFadeSpriteRenderer : MonoBehaviour
{
    Color originColor;

    private void Awake()
    {
        originColor = GetComponent<SpriteRenderer>().color;
    }

    private void OnEnable()
    {
        GetComponent<SpriteRenderer>().color = originColor;
        GetComponent<SpriteRenderer>().DOFade(0.3f, 1f).SetLoops(-1, LoopType.Restart);
    }

    private void OnDisable()
    {
        GetComponent<SpriteRenderer>().DOKill();
    }

    private void OnDestroy()
    {
        GetComponent<SpriteRenderer>().DOKill();
    }
}
