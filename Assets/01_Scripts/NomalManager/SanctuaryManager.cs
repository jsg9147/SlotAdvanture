using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DarkTonic.MasterAudio;

public class SanctuaryManager : MonoBehaviour
{
    public List<BuffCard> buffCards;
    public Vector3 unitLocalScale;

    public List<SkillObject> sanctuaryBuffList;

    public GameObject sanctuaryControlWindow;
    public GameObject buffPopup;
    public GameObject buffSelectEffect;

    public List<UnitSelectSlot> unitSelectBtns;

    public Button marbleButton;

    public Button okButton;
    public Button rerollButton;


    public Button cancelButton;

    [SerializeField]
    private float[] chanceArray; // 노말 레어 유니크 레전드 , 세트는 나중에

    UnitData selectUnitData;

    SkillObject selectedSkill;
    private float totalChance
    {
        get
        {
            float total = 0f;
            for (int i = 0; i < chanceArray.Length; i++)
            {
                total += chanceArray[i];
            }

            return total;
        }
    }

    private void Start()
    {
        SetBuffList();
        CreateUnitInUI();

        rerollButton.onClick.AddListener(Reroll);
        marbleButton.onClick.AddListener(SetActiveBuffPopup);
        okButton.onClick.AddListener(BuffGiveToUnit);
        cancelButton.onClick.AddListener(BackBuffPopup);
        MasterAudio.ChangePlaylistByName("Sanctuary");
    }

    void SetBuffList()
    {
        for (int i = 0; i < buffCards.Count; i++)
        {
            int randomGrade = GetGrade(Random.Range(0, totalChance));
            buffCards[i].SetSanctuaryManager(this);
            buffCards[i].SetSkill(GetBuffSkillObject(randomGrade));
        }
    }

    int GetGrade(float chance)
    {
        int grade = 0;
        float addChance = 0f;
        for (int i = 0; i < this.chanceArray.Length; i++)
        {
            addChance += chanceArray[i];

            if (chance < addChance)
            {
                grade = i;
                break;
            }
        }

        return grade;
    }

    SkillObject GetBuffSkillObject(int grade)
    {
        List<SkillObject> targetList = sanctuaryBuffList.FindAll(x => x.grade == grade);
        int random = Random.Range(0, targetList.Count);

        return targetList[random];
    }



    void CreateUnitInUI()
    {
        for (int i = 0; i < unitSelectBtns.Count; i++)
        {
            Unit unit = Instantiate(PrefabManager.Instance.GetUnitPrefab(GameManager.Instance.playerUnitDatas[i]));
            unit.unitData = GameManager.Instance.playerUnitDatas[i];
            unitSelectBtns[i].SetUnit(unit);
            unit.transform.localScale = unitLocalScale;
            unit.transform.localPosition = new(0, -45, 0);
            unit.LayerChange("Foreground");
            //unit.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 25);

            if (GameManager.Instance.playerUnitDatas[i].sanctuarySkill != null)
            {
                GameObject aura = EffectManager.Instance.InstantiateAura(unit.transform, GameManager.Instance.playerUnitDatas[i].sanctuarySkill);
                aura.GetComponent<ParticleSystemRenderer>().sortingOrder = 11;
                aura.transform.localScale = new(20, 20);
                aura.transform.localPosition = new(0, -28);
            }
        }
    }

    public void SelectCharater(int index)
    {
        selectUnitData = unitSelectBtns[index].unitData;
        okButton.interactable = true;
        for (int i = 0; i < unitSelectBtns.Count; i++)
        {
            unitSelectBtns[i].SelectUnit(i == index);
        }
    }

    public void BuffGiveToUnit()
    {
        StatusEffect effect = new StatusEffect(selectedSkill);
        effect.SetStatData(selectUnitData.stat);
        selectUnitData.AddEffect(effect);
        sanctuaryControlWindow.SetActive(false);

        if (CheckBuffGrades())
            SteamAchievements.Instance.HandleDifferenceOfStarsAchievement();

        GameManager.Instance.RoomClear();

        BackMapScene();
    }

    bool CheckBuffGrades()
    {
        try
        {
            bool isGrade_One = false;
            bool isGrade_Five = false;
            for (int i = 0; i < GameManager.Instance.playerUnitDatas.Length; i++)
            {
                if (GameManager.Instance.playerUnitDatas[i].sanctuarySkill != null)
                {
                    if (GameManager.Instance.playerUnitDatas[i].sanctuarySkill.grade == 0)
                        isGrade_One = true;
                    if (GameManager.Instance.playerUnitDatas[i].sanctuarySkill.grade == 4)
                        isGrade_Five = true;
                }
            }
            return isGrade_One && isGrade_Five;

        }
        catch
        {
            return false;
        }

    }

    public void BuffEffect(Transform cardTransform)
    {
        GameObject effect = Instantiate(buffSelectEffect);
        effect.transform.position = cardTransform.position;
        StartCoroutine(PopupDelayActive());
    }

    IEnumerator PopupDelayActive()
    {
        yield return new WaitForSeconds(1f);
        BackMapScene();
    }

    public void SetActiveBuffPopup()
    {
        MasterAudio.PlaySound("Bless");
        SelectMarkReset();
        buffPopup.SetActive(!buffPopup.activeSelf);
    }

    void BackBuffPopup()
    {
        SelectMarkReset();
        buffPopup.SetActive(true);
        sanctuaryControlWindow.SetActive(false);
    }

    void SelectMarkReset()
    {
        for (int i = 0; i < unitSelectBtns.Count; i++)
        {
            unitSelectBtns[i].SelectUnit(false);
        }
    }
    public void Reroll()
    {
        rerollButton.interactable = false;
        SetBuffList();
    }

    public void BackMapScene()
    {
        GameManager.Instance.LoadScene("MAP");
    }

    public void ControlWindowSetActive(SkillObject skillObject)
    {
        SetActiveBuffPopup();
        sanctuaryControlWindow.SetActive(!sanctuaryControlWindow.activeSelf);
        SelectMarkReset();

        selectedSkill = skillObject;
    }
}