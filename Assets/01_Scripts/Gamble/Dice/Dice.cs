using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    private float[] results;
    [SerializeField]
    private Button button;

    public DiceRoll[] diceRolls;

    public float rollDuration;

    private void Awake()
    {
        button.onClick.AddListener(() => DiceRoll());
    }


    public void DiceRoll()
    {
        results = new float[diceRolls.Length];
        for (int i = 0; i < diceRolls.Length; i++)
        {
            diceRolls[i].StartRoll(rollDuration);
            results[i] = diceRolls[i].Result;

            print(results[i]);
        }
    }
}
