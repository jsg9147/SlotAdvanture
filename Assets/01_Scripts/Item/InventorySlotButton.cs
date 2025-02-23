using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotButton : MonoBehaviour
{
    public Button button;
    [SerializeField] GameObject activeImage;

    public void SetActiveBtn(bool isActive)
    {
        activeImage.SetActive(isActive);
    }
}
