using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitControlComponent : BaseComponent, 
    IComponent, IUpdatable, IUnitControl, IEnable, IDisable, IDisposable
{
    private const string UNIT_LAYER = "Unit";
    [SerializeField, HideInInspector]
    private LayerMask _unitLayer = LayerMask.GetMask(UNIT_LAYER);
    [SerializeField, HideInInspector]
    private Entity _owner, _target;
    [SerializeField]
    private float _aggressionRadius = 4f;
    [SerializeField]
    private float _chasingDistance = 4f;

    [SerializeField]
    private bool _autoattackOn = true;
    private bool _needAttack = false;
    private bool _priority = false;

    private Vector3 _targetPosition;

    private StateMachine _stateMachine;
    public readonly NonTargetState MoveState;
    public readonly ChasingState ChaseState;
    public readonly AttackState AttackState;

    public Action OnStop;
    public bool AutoattackOn => _autoattackOn;
    public bool NeedAttack => _needAttack;
    public bool Priority => _priority;
    public Vector3 TargetPosition => _targetPosition;
    public Entity Target => _target;

    public UnitControlComponent(Entity owner)
    {
        _name = "Unit Control Component";
        _owner = owner;

        _stateMachine = new StateMachine();
        MoveState = new NonTargetState(this, owner, _stateMachine);
        ChaseState = new ChasingState(this, owner, _stateMachine);
        AttackState = new AttackState(this, owner, _stateMachine);

        _stateMachine.Initialize(MoveState);

        if (_owner.TryGetComponent(out IAttackable attackable))
        {
            _chasingDistance += attackable.GetAttackRange();
            _aggressionRadius += attackable.GetAttackRange();
        }

        Enable();
    }

    public void Enable()
    {
        ServiceLocator.GetService<UpdateManager>().Add(this);
    }

    public void Disable()
    {
        ServiceLocator.GetService<UpdateManager>().Remove(this);
    }

    public void Dispose()
    {
        Disable();
    }

    public void OrderToEntity(Entity entity)
    {
        if (entity == _owner)
        {
            OrderToStop();
            return;
        }
        if (IsAllyEntity(entity))
        {
            _needAttack = false;
        }
        else
        {
            _needAttack = true;
        }
        _target = entity;
        _targetPosition = _owner.transform.position;
        _priority = true;
    }

    public void OrderToAttackEntity(Entity entity)
    {
        if (entity == _owner)
        {
            OrderToStop();
            return;
        }
        _needAttack = true;
        _target = entity;
        _targetPosition = _owner.transform.position;
        _priority = true;
    }

    public void OrderToMove(Vector3 targetPosition)
    {
        ResetTarget();
        _autoattackOn = false;
        if (_owner.TryGetComponent<IAttackable>(out var attackable))
        {
            attackable.AbortAttack();
        }
        _targetPosition = targetPosition;
    }

    public void OrderToAutoattackMove(Vector3 targetPosition)
    {
        _autoattackOn = true;
        _targetPosition = targetPosition;
    }

    public void OrderToStop()
    {
        ResetTarget();
        _autoattackOn = false;
        _targetPosition = Vector3.zero;
        if (_owner.TryGetComponent<IAttackable>(out var attackable))
        {
            attackable.AbortAttack();
        }
        if (_owner.TryGetComponent<IMovable>(out var movable))
        {
            movable.StopMoving();
        }
        OnStop?.Invoke();
    }

    public void OnUpdate(float deltaTime)
    {
        _stateMachine?.CurrentState.Update(deltaTime);
    }

    public bool CanAttack()
    {
        if (_owner.TryGetComponent<IAttackable>(out var attackable))
        {
            return attackable.IsAttackRange(_target.transform.position) && _target.TryGetComponent<IDamagable>(out var damagable);
        }
        return false;
    }

    public bool NeedChase()
    {
        return Vector3.Distance(_owner.transform.position, _target.transform.position) < _chasingDistance ||
               _priority;
    }

    public bool HasTargetUnit()
    {
        return _target != null;
    }

    public bool GetTargetPosition(out Vector3 position)
    {
        position = _targetPosition;
        return _targetPosition != Vector3.zero;
    }

    public void SetAttackTarget(Entity target)
    {
        _target = target;
        _needAttack = true;
    }

    public void ResetTargetPosition()
    {
        _targetPosition = _owner.transform.position;
    }

    public void ResetTarget()
    {
        _target = null;
        _needAttack = false;
        _priority = false;
    }

    public Transform GetTargetTransform()
    {
        return _target.transform;
    }

    public bool TryGetTargetUnitPosition(out Vector3 position)
    {
        if (HasTargetUnit())
        {
            position = _target.transform.position;
            return true;
        }
        position = Vector3.zero;
        return false;
    }

    public Vector3 GetTargetUnitPosition()
    {
         return _target.transform.position;
    }

    public bool IsAllyEntity(Entity entity)
    {
        if (entity.TryGetComponent<TeamComponent>(out var team1) &&
            _owner.TryGetComponent<TeamComponent>(out var team2))
        {
            if (team1.Team == team2.Team)
            {
                return true;
            }
        }
        return false;
    }

    public Entity GetClosestDamagableEnemyInRadius(Vector3 position)
    {
        Collider[] hitColliders = Physics.OverlapSphere(position, _aggressionRadius, _unitLayer);
        List<Entity> entities = new();

        foreach (var hitCollider in hitColliders)
        {
            var entity = hitCollider.GetComponentInParent<Entity>();
            if (
                entity != null &&
                entity != _owner &&
                entity.TryGetComponent<IDamagable>(out var damagable) &&
                damagable.IsAlive() &&
                !IsAllyEntity(entity)
                )
            {
                entities.Add(entity);
            }
        }

        return GetClosestEntity(_owner.transform.position, entities);
    }

    private Entity GetClosestEntity(Vector3 position, List<Entity> entities)
    {
        var closestEntity = entities.FirstOrDefault();
        foreach (var entity in entities)
        {
            float d1 = Vector3.Distance(entity.transform.position, position);
            float d2 = Vector3.Distance(closestEntity.transform.position, position);
            if (d1 < d2)
            {
                closestEntity = entity;
            }
        }
        return closestEntity;
    }
}

public class UnitControlComponentCreator : BaseComponentCreator, IComponentCreator
{
    public UnitControlComponentCreator()
    {
        _name = "Unit Control Component";
    }

    public void CreateComponent(Entity entity)
    {
        UnitControlComponent control = new UnitControlComponent(entity);
        entity.Add(control);
        if (entity.TryGetComponent<AnimationComponent>(out var component))
        {
            component.Add(IDLE, ref control.OnStop);
        }
    }
}