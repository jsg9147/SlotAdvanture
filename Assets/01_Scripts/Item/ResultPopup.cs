using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ResultPopup : MonoBehaviour
{
    public Button btn;
    public TMP_Text textTMP;

    private void Start()
    {
        btn.onClick.AddListener(OKBtn);
        textTMP.font = LocalizationManager.Instance.GetFont();
    }

    public void SetText(string text)
    {   
        textTMP.text = text;
    }
    
    void OKBtn()
    {
        gameObject.SetActive(false);
    }
}
