using static UnityEngine.GraphicsBuffer;

public class NonTargetState : State
{
    private UnitControlComponent _control;
    private Entity _owner;
    private StateMachine _stateMachine;

    public NonTargetState(UnitControlComponent control, Entity owner, StateMachine stateMachine)
    {
        _control = control;
        _owner = owner;
        _stateMachine = stateMachine;
    }

    public override void Enter()
    {
        _control.ResetTarget();
    }
    public override void Update(float deltaTime)
    {
        if (_control.HasTargetUnit())
        {
            if (_control.NeedAttack && _control.CanAttack())
            {
                _stateMachine.ChangeState(_control.AttackState);
            }
            else
            {
                _stateMachine.ChangeState(_control.ChaseState);
            }
        }

        if (_owner.TryGetComponent<IAttackable>(out var attackable) && _control.AutoattackOn)
        {
            var target = _control.GetClosestDamagableEnemyInRadius(_owner.transform.position);
            _control.SetAttackTarget(target);
        }
        if (_owner.TryGetComponent<IMovable>(out var movable) &&
            _control.GetTargetPosition(out var position))
        {
            movable.SetMovePosition(position);
        }
    }
    public override void Exit()
    {

    }
}