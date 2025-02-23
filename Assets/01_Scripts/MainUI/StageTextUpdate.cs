using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StageTextUpdate : MonoBehaviour
{
    public TMP_Text stageText;
    void Start()
    {
        stageText.text = $"{GameManager.Instance.stageData.Stage + 1}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
