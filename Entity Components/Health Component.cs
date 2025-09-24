using System;
using UnityEngine;

[Serializable]
public class HealthComponent: BaseComponent, IComponent, IDamagable, IDisposable
{
    [SerializeField]
    private float _maxHealthAmount;
    public float MaxHealthAmount
    {
        get { return _maxHealthAmount; }
    }

    [SerializeField]
    private float _currentHealthAmount;
    public float CurrentHealthAmount
    {
        get { return _currentHealthAmount; }
    }

    [SerializeField]
    private float _restorationAmount;
    public float RestorationAmount
    {
        get { return _restorationAmount; }
    }

    [SerializeField, HideInInspector]
    private bool _isAlive;
    public bool IsAlive()
    {
        return _isAlive;
    }

    private TickTimer _timer;
    private float _delay = 0.1f;

    public Action<float> OnHealthAmountChanged;
    public Action<float> OnMaxHealthAmountChanged;
    public Action OnDied;
    public Action OnRevived;

    public HealthComponent(float maxHealthAmount, float restoration, string name)
    {
        _name = name;
        _maxHealthAmount = maxHealthAmount;
        _currentHealthAmount = _maxHealthAmount;
        _restorationAmount = restoration;
        _isAlive = true;
        _timer = ServiceLocator.GetService<TimerSystem>().CreateTickTimer(_delay, () => Restore(_delay));
    }

    public void TakeDamage(float damageAmount)
    {
        if (damageAmount > 0 && _isAlive)
        {
            _currentHealthAmount -= damageAmount;
            _currentHealthAmount = Mathf.Clamp(_currentHealthAmount, 0, _maxHealthAmount);
            OnHealthAmountChanged?.Invoke(_currentHealthAmount);

            if (_currentHealthAmount == 0)
            {
                _isAlive = false;
                OnDied?.Invoke();
            }
        }
    }

    public void Heal(float healAmount)
    {
        if (healAmount > 0 && _isAlive && _currentHealthAmount < _maxHealthAmount)
        {
            _currentHealthAmount += healAmount;
            _currentHealthAmount = Mathf.Clamp(_currentHealthAmount, 0, _maxHealthAmount);
            OnHealthAmountChanged?.Invoke(_currentHealthAmount);
        }
    }

    public void Revive()
    {
        if (!_isAlive)
        {
            _currentHealthAmount = _maxHealthAmount;
            _isAlive = true;
            OnRevived?.Invoke();
            _timer?.Resume();
        }
    }

    public void Kill()
    {
        if (_isAlive)
        {
            _isAlive = false;
            OnDied?.Invoke();
            _timer?.Pause();
        }
    }

    public void SetMaxHealthAmount(float amount)
    {
        float currentHealthValue = _currentHealthAmount / _maxHealthAmount;
        _maxHealthAmount = amount;
        _currentHealthAmount = amount * currentHealthValue;
        OnHealthAmountChanged?.Invoke(_currentHealthAmount);
        OnMaxHealthAmountChanged?.Invoke(_maxHealthAmount);
    }

    public void Dispose()
    {
        _timer?.Abort();
    }

    private void Restore(float deltaTime)
    {
        if (_isAlive && _currentHealthAmount < _maxHealthAmount)
        {
            _currentHealthAmount += _restorationAmount * deltaTime;
            _currentHealthAmount = Mathf.Clamp(_currentHealthAmount, 0, _maxHealthAmount);
            OnHealthAmountChanged?.Invoke(_currentHealthAmount);
        }
    }
}
public class HealthComponentCreator : BaseHealthComponentCreator, IComponentCreator
{
    private const string HEALTH_COMPONENT_NAME = "Health Component";

    [SerializeField]
    private float _maxHitPoints, _restoration;
    public HealthComponentCreator()
    {
        _name = HEALTH_COMPONENT_NAME;
    }

    public void CreateComponent(Entity entity)
    {
        HealthComponent health = new HealthComponent(_maxHitPoints, _restoration, _name);
        health.OnDied += () => ServiceLocator.GetService<EntityDisposer>().DelayedDispose(entity);
        entity.Add(health);
    }
}