using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LocalizationText : MonoBehaviour
{
    [SerializeField] string textKey;
    [SerializeField] TMP_Text textTMP;

    private void Start()
    {
        if (textTMP == null)
            textTMP = GetComponent<TMP_Text>();

        if (GameManager.Instance)
        {
            GameManager.Instance.EnrollText(this);
        }
        SetLocalizationText();
    }

    public void SetLocalizationText()
    {
        textTMP.font = LocalizationManager.Instance.GetFont();
        string text = LocalizationManager.Instance.GetUILocalizingText(textKey).Replace("\\n", "\n");
        textTMP.text = text;
    }
}
