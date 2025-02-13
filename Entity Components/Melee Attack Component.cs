using UnityEngine;

public class MeleeAttackComponent : BaseAttackComponent
{
    public MeleeAttackComponent(float damage,
        float cooldown,
        float attackRange,
        float attackDelay,
        Transform transform)
    {
        _name = "Melee Attack Component";
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
    public MeleeAttackComponentCreator()
    {
        _name = "Melee Attack Component";
    }

    public void CreateComponent(Entity entity)
    {
        MeleeAttackComponent attack = new MeleeAttackComponent(
            _damage,
            _cooldown,
            _attackRange,
            _attackDelay,
            entity.transform);
        if (_needAiming)
        {
            TryAddAimingCondition(entity, attack);
        }
        TrySetLifeEvents(entity, attack);
        TryAddAnimation(entity, ref attack.OnAttack);
        entity.Add(attack);
    }
}