using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class RecordTextController : MonoBehaviour, IPointerClickHandler
{
    public List<TMP_Text> textGroup;
    public Button button;

    public Image arrow;

    PlayData playData;

    float blinkInterval = 0.5f;
    private bool isBlinking = false;

    int currentIndex;
    void Start()
    {
        SetRecordText();

        if (arrow == null)
        {
            Debug.LogError("Image 컴포넌트를 찾을 수 없습니다!");
        }

        // 코루틴 시작
        StartCoroutine(Blink());

        button.onClick.AddListener(BackMain);
        button.gameObject.SetActive(false);
    }

    void SetRecordText()
    {
        playData = GameManager.Instance.playData;

        try
        {
            textGroup[0].text = $"{GetRecordText("highestPercent")} {(playData.highestPercent/10.0f).ToString("0.0")}";
            textGroup[1].text = $"{GetRecordText("lowestPercent")} {(playData.lowestPercent/10.0f).ToString("0.0")}";
            textGroup[2].text = $"{GetRecordText("hitCount")} {playData.hitCount}";
            textGroup[3].text = $"{GetRecordText("missCount")} {playData.missCount}";
            textGroup[4].text = $"{GetRecordText("monsterHitCount")} {playData.monsterHitCount}";
            textGroup[5].text = $"{GetRecordText("monsterMissCount")} {playData.monsterMissCount}";
            textGroup[6].text = $"{GetRecordText("skillBookTryCount")} {playData.skillBookTryCount}";
            textGroup[7].text = $"{GetRecordText("skillBookSuccessCount")} {playData.skillBookSuccessCount}";
            textGroup[8].text = $"{GetRecordText("blackJackCount")} {playData.blackJackCount}";
            textGroup[9].text = $"{GetRecordText("blackJackWinCount")} {playData.blackJackWinCount}";
            textGroup[10].text = $"{GetRecordText("blackJackGetItemCount")} {playData.blackJackGetItemCount}";
            textGroup[11].text = $"{GetRecordText("ghostLegCount")} {playData.ghostLegCount}";
            textGroup[12].text = $"{GetRecordText("ghostLegWinCount")} {playData.ghostLegWinCount}";
        }
        catch(System.NullReferenceException ex)
        {
            print(ex);
        }
    }

    string GetRecordText(string key)
    {
        return LocalizationManager.Instance.GetUILocalizingText(key);
    }


    void ClickEvent()
    {
        if (currentIndex < textGroup.Count)
        {
            textGroup[currentIndex].gameObject.SetActive(true);
            currentIndex++;
        }
        else
        {
            button.gameObject.SetActive(true);
        }
    }

    void BackMain() => GameManager.Instance.LoadScene("MAIN");

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickEvent();
    }

    IEnumerator Blink()
    {
        while (true)
        {
            isBlinking = !isBlinking;
            arrow.enabled = isBlinking;

            yield return new WaitForSeconds(blinkInterval);
        }
    }
}
