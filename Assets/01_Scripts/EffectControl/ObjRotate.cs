using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjRotate : MonoBehaviour
{
    public float rotationSpeed = 10f;

    void Update()
    {
        // Y축 주위로 물체를 회전시킵니다.
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
