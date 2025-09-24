using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class BaseInfluence
{
    [SerializeField, HideInInspector]
    protected string _name;

    [SerializeField]
    protected float _amount, increasePerLevel;
    [SerializeField]
    protected bool _isPercent, _isNegative;
}

public class DamageChangeInfluence: BaseInfluence, IRemoveableInfluence
{

    public DamageChangeInfluence()
    {
        _name = "Increase Damage";
    }

    public void Apply(Entity entity)
    {
        if (entity.TryGetComponent<BaseAttackComponent>(out var attack))
        {
            attack.AddDamage(_amount);
        }
    }

    public void Dispel(Entity entity)
    {
        if (entity.TryGetComponent<BaseAttackComponent>(out var attack))
        {
            attack.RemoveDamage(_amount);
        }
    }
}

public class DealDamageInfluence : BaseInfluence, IOnceInfluence
{
    public DealDamageInfluence()
    {
        _name = "Deal Damage";
    }

    public void Apply(Entity entity)
    {
        if (entity.TryGetComponent<HealthComponent>(out var health))
        {
            health.TakeDamage(_amount);
        }
    }
}

public class HealInfluence : BaseInfluence, IOnceInfluence
{
    public HealInfluence()
    {
        _name = "Heal";
    }

    public void Apply(Entity entity)
    {
        if (entity.TryGetComponent<HealthComponent>(out var health))
        {
            health.Heal(_amount);
        }
    }
}