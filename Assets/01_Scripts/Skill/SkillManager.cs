using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    public List<Sprite> skillBookIcons;

    [SerializeField]
    private SkillObject[] skillObjects;

    public UnitData targetUnit;

    SkillObject removeSkill;

    public SkillBookPopup popupPrefab;
    SkillBookPopup popup;

    [HideInInspector]
    public SkillObject bookSkill;

    float percent;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        SceneManager.sceneLoaded += LoadSceneInit;
    }

    void LoadSceneInit(Scene scene, LoadSceneMode mode)
    {
        
    }

    public void TargetUnitSelect(UnitData unitData) => targetUnit = unitData;

    public void RemoveTargetSkill(SkillObject skillObject) => removeSkill = skillObject;

    public void ApplySkillBook(SkillObject skillObject)
    {
        targetUnit.skills.Remove(removeSkill);
        targetUnit.skills.Add(skillObject);
    }

    public void SkillBookUsed(bool isActive)
    {
        SlotMachineManager.Instance.SlotStart(Belong.Player);

        StartCoroutine(SkillBookPopup(isActive));
    }

    IEnumerator SkillBookPopup(bool isActive)
    {
        if(popup == null)
            popup = Instantiate(popupPrefab, ItemManager.Instance.canvas.transform);

        yield return new WaitForSeconds(SlotMachineManager.Instance.actionExcutionDelay);

        popup.gameObject.SetActive(isActive);
        popup.PopupSetActive(SlotMachineManager.Instance.result);
    }
}
