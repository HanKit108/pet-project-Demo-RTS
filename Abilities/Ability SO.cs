using System;
using UnityEngine;

public enum AbilityType
{
    Passive,
    Point_Oriented,
}

[Flags]
public enum PassiveAbilityType
{
    None = 0,
    Attributes = 1,
    Aura = 2,
    Attack_Modifier = 4,
}

[Flags]
public enum Influences
{
    None = 0,
    Attack_Damage = 1,
    Deal_Damage = 2,
    Heal = 4,
}


[CreateAssetMenu(fileName = "Ability Config", menuName = "Ability Config", order = 52)]
public class AbilityConfigSO : ScriptableObject
{
    [SerializeField]
    private string _name;

    public string Name => _name;

    [SerializeField, HideInInspector]
    private AbilityType _previousAbilityType;

    [SerializeReference]
    private BaseAbilityConfig _abilityConfig;

    [SerializeField]
    AbilityType _abilityType;

    private void OnValidate()
    {
        SetAbillity<PassiveAbilityConfig>(() => _abilityType.HasFlag(AbilityType.Passive));
        SetAbillity<PointOrientedAbilityConfig>(() => _abilityType.HasFlag(AbilityType.Point_Oriented));
    }

    private void SetAbillity<T>(Func<bool> condition) where T : new()
    {
        if (condition.Invoke() && _previousAbilityType != _abilityType)
        {
            T component = new T();
            _abilityConfig = component as BaseAbilityConfig;
        }
        _previousAbilityType = _abilityType;
        _abilityConfig?.OnValidate();
    }

    public Ability Get()
    {
        return new Ability(_name, this);
    }

    public void Add(Entity entity)
    {
        _abilityConfig.Add(entity);
    }

    public void Remove(Entity entity)
    {
        _abilityConfig.Remove(entity);
    }
}