using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class ButtonController : MonoBehaviour
{
    [SerializeField] private Sprite interactableImage;
    [SerializeField] private Sprite selectedImage;

    [SerializeField] private Image btnImage;
    [SerializeField] private TMP_Text btnText;
    public Button button;

    bool isSelected;

    private void Start()
    {
        btnText.font = LocalizationManager.Instance.GetFont();
    }

    public void SetBtnImage(bool isSelected)
    {
        this.isSelected = isSelected;

        btnImage.sprite = isSelected ? selectedImage : interactableImage;
    }

    public void SetInteractable(bool isInteractable)
    {
        button.interactable = isInteractable;

        if (selectedImage != null)
            SetBtnImage(isSelected);
    }

    public void SetText(string text)
    {
        if (btnText != null)
        {
            btnText.text = text;
        }
    }
}
