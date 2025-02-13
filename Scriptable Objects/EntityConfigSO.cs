using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum AttackType
{
    None,
    Melee,
    Range
}
enum MoveType
{
    None,
    Building,
    Ground
}

[Flags]
enum Components
{
    None = 0,
    Mortable = 1,
    Selectable = 2,
    Teamable = 4,
    Production = 8,
    Income = 16,
    Visual = 32,
    Spawner = 64
}

[CreateAssetMenu(fileName = "Entity Config", menuName = "Entity Config", order = 51)]
public class EntityConfigSO : ScriptableObject
{
    [SerializeField]
    private string _name;

    [HideInInspector]
    public string Name => _name;
    [SerializeField]
    private Sprite _icon;

    [HideInInspector]
    public Sprite Icon => _icon;

    [SerializeField, Min(0)]
    private float _productionDelay, _size = 1f;
    [HideInInspector]
    public float ProductionDelay => _productionDelay;
    [HideInInspector]
    public float Size => _size;
    [SerializeField, Min(0)]
    private int _productionCost;
    [HideInInspector]
    public int ProductionCost => _productionCost;

    [SerializeField]
    private AttackType _attackType;
    [SerializeField]
    private MoveType _moveType;
    [SerializeField]
    private Components _components;

    [SerializeField, HideInInspector]
    private AttackType _previousAttackType;
    [SerializeField, HideInInspector]
    private MoveType _previousMoveType;

    [SerializeReference]
    private List<BaseComponentCreator> _setupComponents = new List<BaseComponentCreator>();

    public List<BaseComponentCreator> GetStats()
    {
        return _setupComponents;
    }

    private void OnValidate()
    {
        Set<VisualComponentCreator>(() => _components.HasFlag(Components.Visual));
        Set<TeamComponentCreator>(() => _components.HasFlag(Components.Teamable));
        Set<HealthComponentCreator>(() => _components.HasFlag(Components.Mortable));
        Set<PointsBarComponentCreator>(() => _components.HasFlag(Components.Mortable));
        SetAttackType();
        Set<RotateComponentCreator>(NeedAddRotateComponentCreator);
        SetMoveType();
        Set<SelectComponentCreator>(() => _components.HasFlag(Components.Selectable));
        Set<ColliderComponentCreator>(() => _components.HasFlag(Components.Selectable));
        Set<ProductionComponentCreator>(() => _components.HasFlag(Components.Production));
        Set<ProductionMenuComponentCreator>(() => _components.HasFlag(Components.Production));
        Set<IncomeComponentCreator>(() => _components.HasFlag(Components.Income));
        Set<UnitControlComponentCreator>(NeedAddUnitControl);
        Set<SpawnerComponentCreator>(() => _components.HasFlag(Components.Spawner));
    }

    private void SetAttackType()
    {
        if (_previousAttackType != _attackType)
        {
            Remove<BaseAttackComponentCreator>();
            if (_attackType == AttackType.Melee)
            {
                MeleeAttackComponentCreator component = new MeleeAttackComponentCreator();
                _setupComponents.Add(component as BaseComponentCreator);
            }
            if (_attackType == AttackType.Range)
            {
                RangeAttackComponentCreator component = new RangeAttackComponentCreator();
                _setupComponents.Add(component as BaseComponentCreator);
            }
        }
        _previousAttackType = _attackType;
        TryReplaceAtLast<BaseAttackComponentCreator>();
    }

    private void SetMoveType()
    {
        if (_previousMoveType != _moveType)
        {
            Remove<BaseCollisionComponentCreator>();
            if (_moveType == MoveType.Ground)
            {
                GroundMoveComponentCreator component = new GroundMoveComponentCreator();
                _setupComponents.Add(component as BaseComponentCreator);
            }
            if (_moveType == MoveType.Building)
            {
                BuildingCollisionComponentCreator component = new BuildingCollisionComponentCreator();
                _setupComponents.Add(component as BaseComponentCreator);
            }
        }
        _previousMoveType = _moveType;
        TryReplaceAtLast<BaseCollisionComponentCreator>();
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
            _setupComponents.Add(component as BaseComponentCreator);
        }
        TryReplaceAtLast<T>();
    }

    private void Remove<T>()
    {
        for (int i = 0; i < _setupComponents.Count; i++)
        {
            if (_setupComponents.ElementAt(i) is T)
            {
                _setupComponents.Remove(_setupComponents.ElementAt(i));
            }
        }
    }

    private void TryReplaceAtLast<T>()
    {
        if (TryGet<T>(out var component))
        {
            _setupComponents.Remove(component as BaseComponentCreator);
            _setupComponents.Add(component as BaseComponentCreator);
        }
    }

    private bool TryGet<T>(out T other)
    {
        foreach (var component in _setupComponents)
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

    private bool NeedAddRotateComponentCreator()
    {
        return TryGet<BaseAttackComponentCreator>(out var attack) &&
                attack.NeedAiming;
    }

    private bool NeedAddUnitControl()
    {
        return TryGet<BaseAttackComponentCreator>(out var attack) ||
                TryGet<BaseMoveComponentCreator>(out var move);
    }
}
