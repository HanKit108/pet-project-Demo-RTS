using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Flags]
public enum AppropriateTargets
{
    None = 0,
    Self = 1,
    Ally = 2,
    Enemy = 4,
    Units = 8,
    Buildings = 16,
}

[Serializable]
public class BaseAbilityComponent
{
    [SerializeField, HideInInspector]
    protected string _name;

    public virtual void Add(Entity entity)
    {

    }

    public virtual void Remove(Entity entity)
    {

    }
}

public class BasePassiveAbilityComponent : BaseAbilityComponent, IValidate
{
    [SerializeField]
    protected Influences _influences;

    [SerializeReference]
    protected List<IInfluence> _influencesList = new();

    public void OnValidate()
    {
        Set<DamageChangeInfluence>(() => _influences.HasFlag(Influences.Attack_Damage));
        Set<DealDamageInfluence>(() => _influences.HasFlag(Influences.Deal_Damage));
        Set<HealInfluence>(() => _influences.HasFlag(Influences.Heal));
    }

    private void Remove<T>()
    {
        for (int i = 0; i < _influencesList.Count; i++)
        {
            if (_influencesList.ElementAt(i) is T)
            {
                _influencesList.Remove(_influencesList.ElementAt(i));
            }
        }
    }

    private void Set<T>(Func<bool> condition) where T : new()
    {
        if (TryGet<T>(out var other))
        {
            if (!condition.Invoke())
            {
                Remove<T>();
            }
        }
        else if (condition.Invoke())
        {
            T component = new T();
            _influencesList.Add(component as IInfluence);
        }
    }

    private bool TryGet<T>(out T other)
    {
        foreach (var component in _influencesList)
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

    protected void ApplyRemoveableInfluences(Entity entity)
    {
        for (int i = 0; i < _influencesList.Count; i++)
        {
            if (_influencesList[i] is IRemoveableInfluence other)
            {
                other.Apply(entity);
            }
        }
    }

    protected void DispelRemoveableInfluences(Entity entity)
    {
        for (int i = 0; i < _influencesList.Count; i++)
        {
            if (_influencesList[i] is IRemoveableInfluence other)
            {
                other.Dispel(entity);
            }
        }
    }

    protected void ApplyOnceInfluences(Entity entity)
    {
        for (int i = 0; i < _influencesList.Count; i++)
        {
            if (_influencesList[i] is IOnceInfluence other)
            {
                other.Apply(entity);
            }
        }
    }

    protected void ApplyEffect(Entity entity, float duration)
    {
        if (entity.TryGetComponent<EffectsComponent>(out var effects))
        {
            int id = this.GetHashCode();
            effects.Add(
                id, 
                () => ApplyRemoveableInfluences(entity), 
                () => DispelRemoveableInfluences(entity), 
                duration);
        }
    }
}

public class AttributeAbilityComponent : BasePassiveAbilityComponent
{
    public AttributeAbilityComponent()
    {
        _name = "Attribute";
    }

    public override void Add(Entity entity)
    {
        base.Add(entity);
        ApplyRemoveableInfluences(entity);
    }

    public override void Remove(Entity entity)
    {
        base.Remove(entity);
        DispelRemoveableInfluences(entity);
    }
}

public class AuraAbilityComponent : BasePassiveAbilityComponent
{
    private const float AURA_EFFECT_DURATION_MULTIPLUER = 2;

    [SerializeField]
    private float _tickDelay, _radius;
    [SerializeField]
    private bool _castOnEnemy;
    private const string UNIT_LAYER = "Unit";


    public AuraAbilityComponent()
    {
        _name = "Aura";
    }

    public override void Add(Entity entity)
    {
        Action<Entity> action = entity => Cast(entity);
        int id = this.GetHashCode();
        ServiceLocator.GetService<AurasHandler>().AddEntity(entity, action, id, _tickDelay);
    }

    public override void Remove(Entity entity)
    {
        int id = this.GetHashCode();
        ServiceLocator.GetService<AurasHandler>().RemoveEntity(entity, id);
    }

    private void Cast(Entity entity)
    {
        Collider[] hitColliders = Physics.OverlapSphere(entity.transform.position, _radius, LayerMask.GetMask(UNIT_LAYER));

        foreach (var hitCollider in hitColliders)
        {
            var target = hitCollider.GetComponentInParent<Entity>();
            if (target != null)
            {
                if (IsAppropriateTarget(entity, target))
                {
                    ApplyOnceInfluences(target);
                    ApplyEffect(target, AURA_EFFECT_DURATION_MULTIPLUER * _tickDelay);
                }
            }
        }
    }

    public bool IsAllyEntity(Entity owner, Entity entity)
    {
        if (entity.TryGetComponent<TeamComponent>(out var team1) &&
            owner.TryGetComponent<TeamComponent>(out var team2))
        {
            if (team1.Team == team2.Team)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsAppropriateTarget(Entity source, Entity target)
    {
        if (_castOnEnemy && !IsAllyEntity(source, target) ||
            !_castOnEnemy && IsAllyEntity(source, target))
        {
            return true;
        }
        return false;
    }
}

public class AttackModifierAbilityComponent : BasePassiveAbilityComponent
{
    public AttackModifierAbilityComponent()
    {
        _name = "Attack modifier";
    }

    public override void Add(Entity entity)
    {
        base.Add(entity);
    }

    public override void Remove(Entity entity)
    {
        base.Remove(entity);
    }
}