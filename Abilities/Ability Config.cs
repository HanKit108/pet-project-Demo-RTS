using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[Serializable]
public class BaseAbilityConfig : IValidate
{

    [SerializeReference]
    List<BaseAbilityComponent> _abilityComponents = new();
    [SerializeField]
    protected PassiveAbilityType _components;


    public void OnValidate()
    {
        Set<AttributeAbilityComponent>(() => _components.HasFlag(PassiveAbilityType.Attributes));
        Set<AuraAbilityComponent>(() => _components.HasFlag(PassiveAbilityType.Aura));
    }

    private void Remove<T>()
    {
        for (int i = 0; i < _abilityComponents.Count; i++)
        {
            if (_abilityComponents.ElementAt(i) is T)
            {
                _abilityComponents.Remove(_abilityComponents.ElementAt(i));
            }
        }
    }

    private void Set<T>(Func<bool> condition) where T : new()
    {
        if (TryGet<T>(out var other))
        {
            if (other is IValidate component)
            {
                component.OnValidate();
            }
            if (!condition.Invoke())
            {
                Remove<T>();
            }
        }
        else if (condition.Invoke())
        {
            T component = new T();
            _abilityComponents.Add(component as BaseAbilityComponent);
        }
    }

    private bool TryGet<T>(out T other)
    {
        foreach (var component in _abilityComponents)
        {
            if (component is T tComponent)
            {
                other = tComponent;
                return true;
            }
        }
        other = default(T);
        return false;
    }

    public void Add(Entity entity)
    {
        foreach (var component in _abilityComponents)
        {
            component.Add(entity);
        }
    }

    public void Remove(Entity entity)
    {
        foreach (var component in _abilityComponents)
        {
            component.Remove(entity);
        }
    }
}

public class PassiveAbilityConfig : BaseAbilityConfig
{

}

public class PointOrientedAbilityConfig : BaseAbilityConfig
{

}