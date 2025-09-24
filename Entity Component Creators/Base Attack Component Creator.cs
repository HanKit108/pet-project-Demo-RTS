using UnityEngine;

public class BaseAttackComponentCreator : BaseComponentCreator
{
    [SerializeField, Min(0f)]
    protected float _damage, _attackRange, _attackDelay, _cooldown;

    [SerializeField]
    protected bool _needAiming;
    [SerializeField]
    protected bool _canAttackOnMove;

    [HideInInspector]
    public bool NeedAiming => _needAiming;

    protected void TryAddAimingCondition(Entity entity, BaseConditionComponent component)
    {
        if (_needAiming && entity.TryGetComponent<RotateComponent>(out var rotation))
        {
            component.AddCondition(rotation.IsAimed);
        }
    }

    protected void TryAddMoveCondition(Entity entity, BaseAttackComponent attack)
    {
        if (_canAttackOnMove && entity.TryGetComponent<GroundMoveComponent>(out var move))
        {
            move.AddCondition(attack.IsNotPreparationAttack);
        }
    }
}