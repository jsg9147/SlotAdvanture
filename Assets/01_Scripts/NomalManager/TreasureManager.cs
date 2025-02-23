using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;

public class TreasureManager : MonoBehaviour
{
    public Button chestButton;

    public RewardController rewardController;

    public Transform itemContent;

    public GameObject fireworkEffect;

    public float delay;

    public List<GameObject> itemImageObjs;
    public List<Image> itemImages;
    public int maxRewardCount;


    private void Start()
    {
        chestButton.onClick.AddListener(ChestClick);
        MasterAudio.ChangePlaylistByName("Treasure");
    }

    public void ChestClick()
    {
        OpenEffect();
        chestButton.interactable = false;
    }

    void SetActiveRewardWindow()
    {
        rewardController.SetRewardPopupOn(1);
        GameManager.Instance.currentPlayerRoom.isClear = true;
    }

    void OpenEffect()
    {
        float xPos = -3, yPos = 0; 
        for (int i = 0; i < 10; i++)
        {
            GameObject effect = Instantiate(fireworkEffect);
            effect.transform.position = new(xPos, yPos);
            Destroy(effect, delay);
            xPos += 0.5f;
            yPos += 0.5f;

            if (yPos > 2)
                yPos = 0;
            if (xPos > 3)
                xPos = -3;
        }
        MasterAudio.PlaySound("ChestOpen");
        StartCoroutine(Reward());
    }

    IEnumerator Reward()
    {
        yield return new WaitForSeconds(delay);
        SetActiveRewardWindow();
    }
}
