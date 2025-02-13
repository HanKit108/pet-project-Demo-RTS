using UnityEngine;
using System;

public interface IComponent
{
    
}

public interface ISwitchable
{
    public void SetSwitchEvents(ref Action enableAction, ref Action disableAction);
}

public interface ISelectable
{
    public void SetSelectionEvents(ref Action onSelected, ref Action onDeselected);
}

public interface IStart
{
    public void OnStart();
}

public interface IEnable
{
    public void Enable();
}

public interface IDisable
{
    public void Disable();
}


public interface IComponentCreator
{
    public void CreateComponent(Entity entity);
}

public interface ITickable
{

}

public interface IUpdatable: ITickable
{
    public void OnUpdate(float deltaTime);
}

public interface ILateUpdatable : ITickable
{
    public void OnLateUpdate(float deltaTime);
}

public interface IMovable
{
    public void SetMovePosition(Vector3 targetPosition);
    public void StopMoving();
}

public interface IUnitControl
{
    public void OrderToEntity(Entity targetEntity);
    public void OrderToAttackEntity(Entity targetEntity);
    public void OrderToMove(Vector3 targetPosition);
    public void OrderToAutoattackMove(Vector3 targetPosition);
    public void OrderToStop();

}

public interface IAttackable
{    
    public bool TryAttack(IDamagable damagable, Transform targetTransform);
    public void AbortAttack();
    public IDamagable GetDamagable();
    public bool IsAttackRange(Vector3 targetPosition);
    public float GetAttackRange();
}

public interface IDamagable
{
    public void TakeDamage(float damage);
    public void Heal(float heal);
    public bool IsAlive();
}

public interface IRotatable
{
    public void Initialize(Transform transform, float rotateSpeed);
    public void RotateToward(Transform target, float deltaTime);
}

public interface ICollisiable
{
    public void EnableCollision();
    public void DisableCollision();
}

public interface ITransform
{
    public void Initialize(Transform transform);
}

public interface IDisposable
{
    public void Dispose();
}