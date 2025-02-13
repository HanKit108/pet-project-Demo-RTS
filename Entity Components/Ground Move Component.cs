using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class GroundMoveComponent : BaseConditionComponent, 
    IComponent, IMovable, ICollisiable, IUpdatable, 
    IDisposable, ISwitchable, IEnable, IDisable
{
    private const float MIN_SPEED = 3f;

    [SerializeField]
    private float _moveSpeed;
    private NavMeshAgent _agent;
    private Action _enableAction, _disableAction;

    public GroundMoveComponent(Transform owner, float moveSpeed)
    {
        _name = "Ground Move Component";

        _moveSpeed = moveSpeed;
        if (!owner.gameObject.TryGetComponent(out _agent))
        {
            _agent = owner.gameObject.AddComponent<NavMeshAgent>();

        }
        _agent.avoidancePriority = 1;
        _agent.speed = moveSpeed;
        _agent.acceleration = 10000;
        _agent.angularSpeed = 10000;

        Enable();
    }

    public void SetSwitchEvents(ref Action enableAction, ref Action disableAction)
    {
        enableAction += EnableCollision;
        disableAction += DisableCollision;

        _enableAction = enableAction;
        _disableAction = disableAction;
    }

    public void Enable()
    {
        ServiceLocator.GetService<UpdateManager>().Add(this);
    }

    public void Disable()
    {
        ServiceLocator.GetService<UpdateManager>().Remove(this);
    }

    public void SetMovePosition(Vector3 targetPosition)
    {
        if (_agent != null && _agent.enabled)
        {
            _agent.destination = targetPosition;
        }
    }

    public void StopMoving()
    {
        if (_agent.enabled)
        {
            _agent.ResetPath();
        }
    }

    public bool IsMoving()
    {
        if (_agent != null)
        {
            return _agent.velocity.magnitude > MIN_SPEED;
        }
        return false;
    }

    public void OnUpdate(float deltaTime)
    {
        if (_agent != null && _agent.enabled)
        {
            if (_compositeConditions.Invoke())
            {
                _agent.isStopped = false;
            }
            else
            {
                _agent.isStopped = true;
            }
        }
    }

    public void Dispose()
    {
        EnableCollision();
        GameObject.Destroy(_agent);
        _enableAction -= EnableCollision;
        _disableAction -= DisableCollision;
        Disable();
    }

    public void EnableCollision()
    {
        _agent.enabled = true;
    }

    public void DisableCollision()
    {
        _agent.enabled = false;
    }
}

public class GroundMoveComponentCreator : BaseMoveComponentCreator, IComponentCreator
{
    public GroundMoveComponentCreator()
    {
        _name = "Ground Moving Component";
    }

    public void CreateComponent(Entity entity)
    {
        GroundMoveComponent moving = new GroundMoveComponent(entity.transform, _moveSpeed);
        TrySetLifeEvents(entity, moving);
        TryAddAttackCondition(entity, moving);
        TryAddAnimation(entity, moving.IsMoving);
        entity.Add(moving);
    }
}