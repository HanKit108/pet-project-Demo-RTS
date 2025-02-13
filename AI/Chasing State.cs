public class ChasingState : State
{
    private UnitControlComponent _control;
    private Entity _owner;
    private StateMachine _stateMachine;

    public ChasingState(UnitControlComponent control, Entity owner, StateMachine stateMachine)
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
        if (_control.HasTargetUnit() && _control.NeedChase())
        {
            if (_control.NeedAttack && _control.CanAttack())
            {
                _stateMachine.ChangeState(_control.AttackState);
            }
            if (_owner.TryGetComponent<IMovable>(out var movable))
            {
                movable.SetMovePosition(_control.GetTargetUnitPosition());
            }
        }
        else
        {
            _stateMachine.ChangeState(_control.MoveState);
        }
    }
    public override void Exit()
    {

    }
}