using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    private static PrefabManager instance;

    public Canvas frontCanvasPrefab;

    [Header("Unit Prefabs")]
    [SerializeField] private List<Unit> unitPrefabs;
    [SerializeField] private List<Unit> uiUnitPrefabs;

    [Header("Battle UI")]
    public BuffSlotController buffSlotControllerPrefab;

    public static PrefabManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }
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

    public Unit GetUnitPrefab(UnitData unitData)
    {
        return unitPrefabs[unitData.prefabIndex];
    }
    public Unit GetUIUnitPrefab(UnitData unitData, Transform parent = null)
    {
        Unit uiUnitPrefab = Instantiate(uiUnitPrefabs[unitData.prefabIndex]);
        if (parent != null)
        {
            uiUnitPrefab.transform.SetParent(parent);
            uiUnitPrefab.transform.localPosition = Vector3.zero;
        }

        return uiUnitPrefab;
    }
}
