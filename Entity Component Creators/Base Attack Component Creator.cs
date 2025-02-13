using System;
using UnityEngine;

public class BaseAttackComponentCreator : BaseComponentCreator
{
    protected const string ATTACK = "Attack Animation";
    protected const string ATTACK_MULTIPLUER = "AttackMultipluer";
    protected const float ATTACK_ANIMATION_KOEF = 2;

    [SerializeField, Min(0f)]
    protected float _damage, _attackRange, _attackDelay, _cooldown;

    [SerializeField]
    protected bool _needAiming;

    [HideInInspector]
    public bool NeedAiming => _needAiming;

    protected void TryAddAnimation(Entity entity, ref Action action)
    {
        if (entity.TryGetComponent<AnimationComponent>(out var animation))
        {
            animation.Add(ATTACK, ref action);
            float multipluer = 1 / (ATTACK_ANIMATION_KOEF * _attackDelay);
            animation.SetMultipluer(ATTACK_MULTIPLUER, multipluer);
        }
    }

    protected void TryAddAimingCondition(Entity entity, BaseConditionComponent component)
    {
        if (entity.TryGetComponent<RotateComponent>(out var rotation))
        {
            component.AddCondition(rotation.IsAimed);
        }
    }
}

public class RotateComponentCreator : BaseComponentCreator, IComponentCreator
{
    [SerializeField, Min(0f)]
    protected float _rotationSpeed;

    public RotateComponentCreator()
    {
        _name = "Rotate component";
    }

    public void CreateComponent(Entity entity)
    {
        RotateComponent rotation = new RotateComponent();
        rotation.Initialize(entity.transform, _rotationSpeed);
        TryAddDeathCondition(entity, rotation);
        entity.Add(rotation);
    }

}