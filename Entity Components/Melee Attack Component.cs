using System;
using UnityEngine;

public class MeleeAttackComponent : BaseAttackComponent
{
    public MeleeAttackComponent(float damage,
        float cooldown,
        float attackRange,
        float attackDelay,
        Transform transform,
        string name)
    {
        _name = name;
        _damage = damage;
        _cooldown = cooldown;
        _attackRange = attackRange;
        _attackDelay = attackDelay;
        _transform = transform;
    }

    protected override void CompleteAttack()
    {
        base.CompleteAttack();
        if (_target != null && _compositeConditions.Invoke())
        {
            ServiceLocator.GetService<DealDamageController>().DealDamage(_target, _damage);
        }
    }
}

public class MeleeAttackComponentCreator : BaseAttackComponentCreator, IComponentCreator
{
    private const string MELEE_ATTACK_COMPONENT_NAME = "Melee Attack Component";

    public MeleeAttackComponentCreator()
    {
        _name = MELEE_ATTACK_COMPONENT_NAME;
    }

    public void CreateComponent(Entity entity)
    {
        MeleeAttackComponent attack = new MeleeAttackComponent(
            _damage,
            _cooldown,
            _attackRange,
            _attackDelay,
            entity.transform,
            _name);
        TryAddAimingCondition(entity, attack);
        TryAddMoveCondition(entity, attack);
        TrySetLifeEvents(entity, attack);
        entity.Add(attack);
    }
}