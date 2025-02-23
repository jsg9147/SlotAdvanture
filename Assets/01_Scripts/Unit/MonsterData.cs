using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;

[System.Serializable]
public class SoundGroup
{
    public AudioClip move;
    public AudioClip attack;
    public AudioClip damaged;
    public AudioClip skill;
}

[CreateAssetMenu(fileName = "Unit Data", menuName = "Scriptable Object/Unit Data", order = int.MaxValue)]
public class MonsterData : ScriptableObject
{
    public Vector3 namePos;

    public string monsterCode;

    public SoundGroup soundGroup;

    private string unitName;
    public string UnitName { get { return LocalizationManager.Instance.GetMonsterLocalizingName(monsterCode); } }

    [SerializeField] private float _hp;
    public float HP { get { return _hp; } }
    [SerializeField] private float _ad;
    public float AD { get { return _ad; } }

    [SerializeField] private float _ap;
    public float AP { get { return _ap; } }
    [SerializeField] private float _defense;
    public float DEF { get { return _defense; } }

    [SerializeField] private float _mr;
    public float MR { get { return _mr; } }

    [SerializeField] private float _speed;
    public float Speed { get { return _speed; } }

    [SerializeField] private List<SkillObject> _skills;

    public List<SkillObject> Skills {  get { return _skills; } }
}
