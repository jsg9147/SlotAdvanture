using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapDrag : MonoBehaviour, IDragHandler
{
    RectTransform rectTransform;

    bool isAlt;
    Vector2 clickPoint;
    float dragSpeed = 30.0f;

    Vector3 stopPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Update()
    {
        DragEvent();
    }

    void DragEvent()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt)) isAlt = true;
        if (Input.GetKeyUp(KeyCode.LeftAlt)) isAlt = false;

        if (Input.GetMouseButtonDown(0))
        {
            clickPoint = Input.mousePosition;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (ItemManager.Instance.InventoryActiveSelf)
            return;

        if (Input.GetMouseButton(0))
        {
            if (isAlt)
            {
                Vector3 position
                    = Camera.main.ScreenToViewportPoint((Vector2)Input.mousePosition - clickPoint);

                position.z = .0f;

                Vector3 move = position * (Time.deltaTime * dragSpeed);

                if (Mathf.Abs(transform.position.x) > 600f)
                    move.x = 0f;
                if (Mathf.Abs(transform.position.y) > 300f)
                    move.y = 0f;

                rectTransform.Translate(move);
                rectTransform.anchoredPosition
                    = new Vector3(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y, 0f);

                if (Mathf.Abs(rectTransform.anchoredPosition.x) > 600f)
                {
                    rectTransform.anchoredPosition = new Vector2(Mathf.Sign(rectTransform.anchoredPosition.x) * 600f, rectTransform.anchoredPosition.y);
                }
                if (Mathf.Abs(rectTransform.anchoredPosition.y) > 300f)
                {
                    rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, Mathf.Sign(rectTransform.anchoredPosition.y) * 300f);
                }
            }
        }
    }
}
