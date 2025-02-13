public class AttackState : State
{
    private UnitControlComponent _control;
    private Entity _owner;
    private StateMachine _stateMachine;

    public AttackState(UnitControlComponent control, Entity owner, StateMachine stateMachine)
    {
        _control = control;
        _owner = owner;
        _stateMachine = stateMachine;
    }
    public override void Enter()
    {

    }
    public override void Update(float deltaTime)
    {
        if (_control.HasTargetUnit() && 
            _control.Target.TryGetComponent<IDamagable>(out var damagable) &&
            damagable.IsAlive())
        {
            if (!_control.NeedAttack || !_control.CanAttack())
            {
                _stateMachine.ChangeState(_control.ChaseState);
            }
            if (_owner.TryGetComponent<IMovable>(out var movable))
            {
                movable.StopMoving();
            }

            if (_owner.TryGetComponent<IRotatable>(out var rotatable))
            {
                rotatable.RotateToward(_control.GetTargetTransform(), deltaTime);
            }

            var attackable = _owner.GetComponent<IAttackable>();
            attackable.TryAttack(damagable, _control.GetTargetTransform());
        }
        else
        {
            if (_control.Priority)
            {
                _control.ResetTargetPosition();
            }
            _stateMachine.ChangeState(_control.MoveState);
        }

    }
    public override void Exit()
    {

    }
}