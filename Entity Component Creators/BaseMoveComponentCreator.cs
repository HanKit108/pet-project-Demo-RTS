using System;
using UnityEngine;

public class BaseMoveComponentCreator : BaseCollisionComponentCreator
{
    protected const float DEFAULT_MOVE_SPEED = 5f;
    protected const string IS_MOVING = "IsMoving";
    protected const string MOVE_MULTIPLUER = "MoveMultipluer";

    [SerializeField]
    protected float _moveSpeed;
    [SerializeField]
    protected bool _canAttackOnMove;

    protected void TryAddAttackCondition(Entity entity, BaseConditionComponent component)
    {
        if (_canAttackOnMove && entity.TryGetComponent<BaseAttackComponent>(out var attack))
        {
            component.AddCondition(attack.IsNotAttack);
        }
    }

    protected void TryAddAnimation(Entity entity, Func<bool> isMoving)
    {
        if (entity.TryGetComponent<AnimationComponent>(out var animation))
        {
            animation.Add(IS_MOVING, isMoving);
            float multipluer = _moveSpeed / DEFAULT_MOVE_SPEED;
            animation.SetMultipluer(MOVE_MULTIPLUER, multipluer);
        }
    }
}
