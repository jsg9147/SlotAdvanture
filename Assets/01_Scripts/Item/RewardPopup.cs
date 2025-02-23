using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardPopup : MonoBehaviour
{
    public GameObject mouseBlock;

    public GameObject rewardPopup;
    public GameObject defeatPopup;
    public GameObject drawPopup;

    public Button okBtn;

    public Button defeatOkBtn;
    public Button drawOkBtn;
    [SerializeField]
    Transform rewardContent;

    private void Start()
    {
        mouseBlock.SetActive(false);
        rewardPopup.SetActive(false);

        defeatPopup.SetActive(false);
        defeatOkBtn.onClick.AddListener(() => defeatPopup.SetActive(false));
        okBtn.onClick.AddListener(() => ClearPopup());
        if(drawOkBtn)
            drawOkBtn.onClick.AddListener(() => drawPopup.SetActive(false));
    }

    public void WinPopupSetActive(bool isActive)
    {
        mouseBlock.SetActive(isActive);
        rewardPopup.SetActive(isActive);
    }

    void ClearPopup()
    {
        rewardPopup.SetActive(false);
    }

    public void DefeatPopupSetActive(bool isActive)
    {
        mouseBlock.SetActive(isActive);
        defeatPopup.SetActive(isActive);
    }

    public void DrawPopupSetActive(bool isActive)
    {
        mouseBlock.SetActive(isActive);
        drawPopup.SetActive(isActive);
    }
    public Transform GetContentTransform() => rewardContent;
}
