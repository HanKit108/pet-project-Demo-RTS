using System;
using UnityEngine;

[Serializable]
public class BaseComponentCreator
{
    protected const string IDLE = "Idle Animation";

    [SerializeField, HideInInspector]
    protected string _name;

    protected void TryAddDeathCondition(Entity entity, BaseConditionComponent component)
    {
        if (entity.TryGetComponent<HealthComponent>(out var health))
        {
            component.AddCondition(health.IsAlive);
        }
    }

    protected IComponent AddComponent<T>(Entity entity) where T : new()
    {
        T component = new T();
        var tComponent = component as IComponent;
        entity.Add(tComponent);
        return tComponent;
    }

    protected void TrySetLifeEvents(Entity entity, ISwitchable component)
    {
        if (entity.TryGetComponent<HealthComponent>(out var health))
        {
            component.SetSwitchEvents(ref health.OnRevived, ref health.OnDied);
        }
    }

    protected void TrySetSelectionEvents(Entity entity, ISelectable component)
    {
        if (entity.TryGetComponent<SelectComponent>(out var selection))
        {
            component.SetSelectionEvents(ref selection.OnSelected, ref selection.OnDeselected);
        }
    }
}