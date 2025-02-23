using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingPopup : MonoBehaviour
{
    [SerializeField] Button endingCreditBtn;

    private void Start()
    {
        endingCreditBtn.onClick.AddListener(() => GameManager.Instance.EndingCreditOn());
    }
}
