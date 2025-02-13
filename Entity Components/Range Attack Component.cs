using UnityEngine;

public class RangeAttackComponent : BaseAttackComponent
{
    private const float VERTICAL_OFFSET = 1f;

    [SerializeField]
    private float _projectileSpeed;
    private Transform _projectilePrefab;

    public RangeAttackComponent(
        float damage,
        float cooldown,
        float attackRange,
        float attackDelay,
        Transform transform,
        float projectileSpeed,
        Transform projectilePrefab) 
    {
        _name = "Range Attack Component";

        _damage = damage;
        _cooldown = cooldown;
        _attackRange = attackRange;
        _attackDelay = attackDelay;
        _transform = transform;
        _projectileSpeed = projectileSpeed;
        _projectilePrefab = projectilePrefab;
    }

    protected override void CompleteAttack()
    {
        base.CompleteAttack();
        if (_target != null && _compositeConditions.Invoke())
        {
            ServiceLocator.GetService<RangeAttackController>().LaunchProjectile
                (
                _projectilePrefab,
                _transform.position + Vector3.up,
                _targetTransform,
                _target,
                _damage,
                _projectileSpeed,
                VERTICAL_OFFSET
                );
        }
    }
}

public class RangeAttackComponentCreator : BaseAttackComponentCreator,
    IComponentCreator
{
    [SerializeField]
    private float _projectileSpeed;
    [SerializeField]
    private Transform _projectilePrefab;

    public RangeAttackComponentCreator()
    {
        _name = "Range Attack Component";
    }

    public void CreateComponent(Entity entity)
    {
        RangeAttackComponent attack = new RangeAttackComponent(_damage,
            _cooldown,
            _attackRange,
            _attackDelay,
            entity.transform,
            _projectileSpeed,
            _projectilePrefab);
        if (_needAiming)
        {
            TryAddAimingCondition(entity, attack);
        }
        TryAddDeathCondition(entity, attack);
        TryAddAnimation(entity, ref attack.OnAttack);
        entity.Add(attack);
    }
}