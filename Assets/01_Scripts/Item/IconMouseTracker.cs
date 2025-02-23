using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconMouseTracker : MonoBehaviour
{
    public bool alwaysTracking;

    RectTransform rectTransform;
    Canvas canvas;
    Image image;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = UIController.Instance.canvas;
        image = GetComponent<Image>();

        if(image)
            image.raycastTarget = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(alwaysTracking)
            MouseTracker();
    }

    private void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = UIController.Instance.canvas;
        MouseTracker();
    }

    void MouseTracker()
    {
        Vector3 mousePos = Input.mousePosition;

        Vector3 movePosition = Camera.main.ScreenToWorldPoint(mousePos);
        movePosition.z = 10f;

        float maxXpos = canvas.GetComponent<RectTransform>().rect.width / 2 - rectTransform.rect.width;

        rectTransform.position = movePosition;
        if (rectTransform.anchoredPosition.x >= maxXpos)
        {
            rectTransform.localPosition = new(rectTransform.localPosition.x - rectTransform.rect.width, rectTransform.localPosition.y);
        }
    }
}
