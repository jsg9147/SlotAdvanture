using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HorizontalLine : MonoBehaviour
{
    public BoxCollider2D leftCollider; 
    public BoxCollider2D rightCollider; 
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetActive(bool isAcitve)
    {
        leftCollider.enabled = isAcitve;
        rightCollider.enabled = isAcitve;

        GetComponent<Image>().color = isAcitve ? new(0, 0, 0, 1) : new(0, 0, 0, 0);
    }
}
