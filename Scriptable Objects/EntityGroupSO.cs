using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entity Group", menuName = "Entity Group", order = 51)]
public class EntityGroupSO : ScriptableObject
{
    [SerializeField]
    private List<EntityUnit> _entiies;

    [HideInInspector]
    public List<EntityUnit> Entiies => _entiies;

    private void OnValidate()
    {
        foreach (var entity in _entiies)
        {
            entity.UpdateName();
        }
    }

    public List<EntityConfigSO> GetEntityConfigList()
    {
        List<EntityConfigSO> entities = new();

        foreach (var unit in _entiies)
        {
            for (int i = 0; i < unit.amount; i++)
            {
                entities.Add(unit.entityConfig);
            }
        }
        return entities;
    }
}

[Serializable]
public class EntityUnit
{
    [SerializeField, HideInInspector]
    private string _name;

    public EntityConfigSO entityConfig;
    [Min(1)]
    public int amount;

    public void UpdateName()
    {
        if (entityConfig != null)
        {
            _name = entityConfig.name;
        }
    }
}