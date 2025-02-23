using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseUnit : MonoBehaviour
{
    Transform unitTrnaform;
    bool isChase = false;
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (isChase)
            Chase();
    }

    public void SetUnit(Transform unitTrnas)
    {
        unitTrnaform = unitTrnas;
        isChase = true;
    }

    void Chase()
    {
        transform.position = unitTrnaform.position;
    }
}
