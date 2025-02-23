using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoulettePieceData
{
    public Sprite icon;
    public string description;
    public float value;

    // chance / 총합
    [Range(1, 100)]
    public int chance = 100;

    [HideInInspector]
    public int index;  // 순번

    [HideInInspector]
    public int weight; // 가중치
}
