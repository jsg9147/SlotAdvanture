using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastSlashEffect : MonoBehaviour
{
    public float startTime;
    void Start()
    {
        StartCoroutine(MoveStart());
    }

    private void FixedUpdate()
    {
        
    }
    IEnumerator MoveStart()
    {
        yield return new WaitForSeconds(startTime);

        transform.position = transform.position + (Vector3.right * 10f) + (Vector3.down * 1f);
    }
}
