using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerticalLine : MonoBehaviour
{
    [Header("Prefab")]
    public HorizontalLine horizontalLinePrefab;

    List<HorizontalLine> horizontalLines;
    
    VerticalLayoutGroup verticalLayoutGroup;

    bool isOdd;
    public void SetLine(int lineCount)
    {
        if (horizontalLines == null)
            horizontalLines = new List<HorizontalLine>();
        for (int i = 0; i < lineCount; i++)
        {
            HorizontalLine horizontalLine = Instantiate(horizontalLinePrefab, transform);
            horizontalLines.Add(horizontalLine);
            horizontalLine.SetActive(false);
        }
    }

    public void SetLineOdd(bool isOdd)
    {
        verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();

        SetLayoutSpace(isOdd);
        SetVerticalLines(isOdd);
        ActiveLineSetting();
    }

    void SetLayoutSpace(bool isOdd)
    {
        if (isOdd)
        {
            verticalLayoutGroup.padding.top = 60;
            verticalLayoutGroup.padding.bottom = 30;
        }
        else
        {
            verticalLayoutGroup.padding.bottom = 0;
            verticalLayoutGroup.padding.top = 30;
        }
    }

    void SetVerticalLines(bool isOdd)
    {
        for (int i = 0; i < horizontalLines.Count; i++)
        {
            horizontalLines[i].gameObject.SetActive(false);
        }
        this.isOdd = isOdd;
        int lineCount = isOdd ? 5 : 6;
        for (int i = 0; i < lineCount; i++)
        {
            horizontalLines[i].gameObject.SetActive(true);
        }
    }

    void ActiveLineSetting()
    {
        int lineCount = isOdd ? 5 : 6;
        for (int i = 0; i < horizontalLines.Count; i++)
        {
            bool isActive = Random.Range(0, 2) == 0;
            horizontalLines[i].SetActive(isActive);
        }
    }
}
