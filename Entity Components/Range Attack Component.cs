using UnityEngine;

public class RangeAttackComponent : BaseAttackComponent
{
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
        Transform projectilePrefab,
        string name) 
    {
        _name = name;

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
                Constants.PROJECTILE_START_VERTICAL_OFFSET
                );
        }
    }
}

public class RangeAttackComponentCreator : BaseAttackComponentCreator,
    IComponentCreator
{
    private const string RANGE_ATTACK_COMPONENT_NAME = "Range Attack Component";

    [SerializeField]
    private float _projectileSpeed;
    [SerializeField]
    private Transform _projectilePrefab;

    public RangeAttackComponentCreator()
    {
        _name = RANGE_ATTACK_COMPONENT_NAME;
    }

    public void CreateComponent(Entity entity)
    {
        RangeAttackComponent attack = new RangeAttackComponent(_damage,
            _cooldown,
            _attackRange,
            _attackDelay,
            entity.transform,
            _projectileSpeed,
            _projectilePrefab,
            _name);
        TryAddAimingCondition(entity, attack);
        TryAddMoveCondition(entity, attack);
        TryAddDeathCondition(entity, attack);
        entity.Add(attack);
    }
}