using System;
using UnityEngine;

[Serializable]
public abstract class BaseAttackComponent : BaseConditionComponent, 
    IComponent, IAttackable, ITransform, ISwitchable, IEnable, IDisable, IDisposable
{
    private bool _enabled = true;
    private Action _enableAction, _disableAction;
    private Timer _timer;

    protected IDamagable _target;
    protected bool _canAttack = true, _isAttack = false;
    [SerializeField]
    protected float _attackRange, _attackDelay, _cooldown, _damage;
    [SerializeField, HideInInspector]
    protected Transform _transform, _targetTransform;


    public Action OnAttack;

    public void Initialize(Transform transform)
    {
        _transform = transform;
    }
    public void SetSwitchEvents(ref Action enableAction, ref Action disableAction)
    {
        enableAction += Enable;
        disableAction += Disable;

        _enableAction = enableAction;
        _disableAction = disableAction;
    }

    public void Enable()
    {
        _enabled = true;
        _timer?.Resume();
    }

    public void Disable()
    {
        _enabled = false;
        _timer?.Pause();
    }

    public bool TryAttack(IDamagable target, Transform targetTransform)
    {
        _target = target;
        _targetTransform = targetTransform;

        if (!_target.IsAlive())
        {
            AbortAttack();
            return false;
        }

        if (CanAttack())
        {
            OnAttack?.Invoke();
            _isAttack = true;
            _timer = ServiceLocator.GetService<TimerSystem>().CreateTimer(_attackDelay, CompleteAttack);
            return true;
        }
        return false;
    }
    

    public bool IsAttackRange(Vector3 targetPosition)
    {
        return Vector3.Distance(_transform.position, targetPosition) <= _attackRange;
    }

    public bool IsNotAttack()
    {
        return !_isAttack;
    }

    public IDamagable GetDamagable()
    {
        return _target;
    }
    public float GetAttackRange()
    {
        return _attackRange;
    }

    public void AbortAttack()
    {
        _target = null;
        _targetTransform = null;
        _isAttack = false;
        if (_canAttack)
        {
            _timer?.Abort();
        }
    }

    public void Dispose()
    {
        _enableAction -= Enable;
        _disableAction -= Disable;
    }

    protected virtual void CompleteAttack()
    {
        _isAttack = false;
        if (_target != null && _compositeConditions.Invoke())
        {
            _canAttack = false;
            _timer = ServiceLocator.GetService<TimerSystem>().CreateTimer(_cooldown, RefreshAttack);

        }
    }

    protected void RefreshAttack()
    {
        _canAttack = true;
    }

    private bool CanAttack()
    {
        return _compositeConditions.Invoke() &&
                _enabled &&
                !_isAttack &&
                _canAttack &&
                IsAttackRange(_targetTransform.position);
    }
}