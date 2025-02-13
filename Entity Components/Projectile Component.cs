using System;
using UnityEngine;

public class ProjectileComponent : BaseComponent, 
    IComponent, IUpdatable, IEnable, IDisable, IDisposable
{
    private Transform _targetTransform, _transform;
    private float _speed, _verticalOffset;
    public Action OnCompleted;

    public ProjectileComponent
    (
        Transform targetTransform,
        Transform transform,
        float speed,
        float verticalOffset
    )
    {
        _name = "Projectile Compomnent";
        _targetTransform = targetTransform;
        _transform = transform;
        _speed = speed;
        _verticalOffset = verticalOffset;

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

    public void OnUpdate(float deltaTime)
    {   
        Vector3 targetPosition = _targetTransform.position + Vector3.up * _verticalOffset;
        Vector3 direction = (targetPosition - _transform.position).normalized;
        _transform.position += direction * _speed * Time.deltaTime;
        _transform.rotation = Quaternion.LookRotation(direction);

        if (Vector3.Distance(_transform.position, targetPosition) < 0.4f)
        {
            OnCompleted?.Invoke();
        }
    }
}
