using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BuffCard : MonoBehaviour
{
    SanctuaryManager sanctuaryManager;

    SkillObject skillObject;

    [SerializeField] List<GameObject> starOn;
    public List<Sprite> cardFrames;

    public Image frameImage;
    public Image iconFrameImage;
    public Image iconImage;

    public TMP_Text titleText;
    public TMP_Text infoText;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(SelectBuff);
        SetFont();
    }

    void SetFont()
    {
        titleText.font = LocalizationManager.Instance.GetFont();
        infoText.font = LocalizationManager.Instance.GetFont();
    }

    public void SetSkill(SkillObject skillObject)
    {
        ResetStar();
        this.skillObject = skillObject;

        iconImage.sprite = skillObject.SkillIcon;

        titleText.text = skillObject.GetName();
        infoText.text = skillObject.GetDescription();

        SetFrameSprite(skillObject.grade);
        SetStar(skillObject.grade);
        //iconFrameImage.sprite = iconFrames[skillObject.grade];
    }

    void SetFrameSprite(int grade)
    {
        switch (grade)
        {
            case 0:
            case 1:
                frameImage.sprite = cardFrames[0];
                break;
            case 2:
            case 3:
                frameImage.sprite = cardFrames[1];
                break;
            case 4:
                frameImage.sprite = cardFrames[2];
                break;
        }
    }
    void ResetStar()
    {
        for (int i = 0; i < starOn.Count; i++)
        {
            starOn[i].SetActive(false);
        }
    }
    void SetStar(int grade)
    {
        for (int i = 0; i <= grade; i++)
        {
            if(grade < starOn.Count)
                starOn[i].SetActive(true);
        }
    }

    public void SetSanctuaryManager(SanctuaryManager sanctuaryManager)
    {
        this.sanctuaryManager = sanctuaryManager;
    }

    void SelectBuff()
    {
        sanctuaryManager.ControlWindowSetActive(skillObject);
    }
}
