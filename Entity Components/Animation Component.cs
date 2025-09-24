using System;
using System.Collections.Generic;
using UnityEngine;
public class AnimationPlayer
{
    private string _animation;
    private int _variantsCount;
    private Animator _animator;
    private Action _action;

    public AnimationPlayer(
        string animation, 
        Animator animator, 
        ref Action action,
        int variantsCount
        )
    {
        _animation = animation;
        _animator = animator;
        _variantsCount = variantsCount;

        action += Play;
        _action = action;
    }

    public void Disable()
    {
        _action -= Play;
    }

    private void Play()
    {
        int variant = UnityEngine.Random.Range(0, _variantsCount);
        string animation = variant > 0 ? _animation + " " + variant : _animation;
        _animator.CrossFade(animation, Constants.ANIMATION_TRANSITION_DELAY, 0, 0f);
    }
}

public class RepeatingAnimationPlayer
{
    private string _animation;
    private int _variantsCount, _currentVariant;
    private Animator _animator;
    private CompositeCondition _conditions;
    private Timer _timer;
    private bool _isBored = true;
    private float _animationChangeCooldown = 3f;

    public string Animation => _animation;

    public RepeatingAnimationPlayer(
        string animation,
        Animator animator,
        int variantsCount
        )
    {
        _animation = animation;
        _animator = animator;
        _variantsCount = variantsCount;
        _conditions = new();
    }

    public void AddCondition(Func<bool> condition)
    {
        _conditions.AddCondition(condition);
    }

    public void RemoveCondition(Func<bool> condition)
    {
        _conditions.RemoveCondition(condition);
    }

    public bool TryPlay(string currentAnimation, out string animation)
    {
        TryChangeVariant();
        animation = _currentVariant > 0 ? _animation + " " + _currentVariant : _animation;
        if (_conditions.Invoke() && animation != currentAnimation)
        {
            _animator.CrossFade(animation, Constants.ANIMATION_TRANSITION_DELAY, 0, 0f);
            return true;
        }
        return false;
    }

    public void Disable()
    {
        _timer?.Abort();
    }

    private void RefreshCooldown()
    {
        _animationChangeCooldown = _animator.GetCurrentAnimatorStateInfo(0).length;
    }

    private void TryChangeVariant()
    {
        if (_isBored)
        {
            _isBored = false;
            _currentVariant = UnityEngine.Random.Range(0, _variantsCount);
            if (UnityEngine.Random.Range(0, 100) < Constants.IDLE_ANIMATION_PROBABILITY)
            {
                _currentVariant = 0;
            }
            _timer = ServiceLocator.GetService<TimerSystem>().CreateTimer(_animationChangeCooldown, () => _isBored = true);
        }
    }
}

[Serializable]
public class AnimationComponent : BaseComponent, 
    IComponent, IUpdatable, IEnable, IDisable, IDisposable
{
    private List<AnimationPlayer> _animations = new();
    private List<RepeatingAnimationPlayer> _repeatingAnimations = new();
    private Animator _animator;
    private Func<bool> _moveCondition;
    private string _moveAnimation;
    private string _currentAnimation = "";
    private float currentTime = 0f;

    public Action OnBored;

    public AnimationComponent(Animator animator, string name)
    {
        _name = name;
        if (animator != null)
        {
            _animator = animator;
        }

        Enable();
    }

    public void AddAnimation(string animation, ref Action action, Func<bool> condition, int variantsCount = 0)
    {
        AnimationPlayer player = new AnimationPlayer(animation, _animator, ref action, variantsCount);
        _animations.Add(player);
        action += ResetAnimation;

        for(int i = 0; i < _repeatingAnimations.Count; i++)
        {
            _repeatingAnimations[i].AddCondition(() => !condition.Invoke());
        }
    }

    public void AddAnimation(string animation, int variantsCount = 0)
    {
        RepeatingAnimationPlayer player = new RepeatingAnimationPlayer(animation, _animator, variantsCount);
        _repeatingAnimations.Add(player);
    }

    public RepeatingAnimationPlayer GetRepeatingAnimationPlayer(string animation)
    {
        for (int i = 0; i < _repeatingAnimations.Count; i++)
        {
            if (animation == _repeatingAnimations[i].Animation)
            {
                return _repeatingAnimations[i];
            }
        }
        return null;
    }

    public void AddCondition(string animation, Func<bool> condition)
    {
        var animationPlayer = GetRepeatingAnimationPlayer(animation);
        animationPlayer?.AddCondition(condition);
        for(int i = 0; i < _repeatingAnimations.Count; i++)
        {
            if (animationPlayer != _repeatingAnimations[i])
            {
                _repeatingAnimations[i].AddCondition(() => !condition.Invoke());
            }
        }
    }

    public void RemoveCondition(string animation, Func<bool> condition)
    {
        var animationPlayer = GetRepeatingAnimationPlayer(animation);
        animationPlayer?.RemoveCondition(condition);
        for (int i = 0; i < _repeatingAnimations.Count; i++)
        {
            if (animationPlayer != _repeatingAnimations[i])
            {
                _repeatingAnimations[i].RemoveCondition(() => !condition.Invoke());
            }
        }
    }

    public void SetMultipluer(string multipluer, float value)
    {
        _animator.SetFloat(multipluer, value);
    }

    public void Clear()
    {
        for (int i = 0; i < _repeatingAnimations.Count; i++)
        {
            _repeatingAnimations[i].Disable();
        }
        for (int i = 0; i < _animations.Count; i++)
        {
            _animations[i].Disable();
        }
        _animations.Clear();
        _repeatingAnimations.Clear();
    }

    public void Enable()
    {
        ServiceLocator.GetService<UpdateManager>().Add(this);
    }

    public void Disable()
    {
        ServiceLocator.GetService<UpdateManager>().Remove(this);
    }

    public void OnUpdate(float deltaTime)
    {
        for (int i = 0; i < _repeatingAnimations.Count; i++)
        {
            if (_repeatingAnimations[i].TryPlay(_currentAnimation, out var animation))
            {
                _currentAnimation = animation;
            }
        }
                //Debug.Log("current = " + _currentAnimation + ", state = " + GetAnimationName());
    }

    private string GetAnimationName()
    {
        foreach(var anim in _repeatingAnimations)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(anim.Animation))
            {
                return anim.Animation;
            }
        }
        return null;
    }

    public void Dispose()
    {
        Disable();
        Clear();
    }

    public void SetAnimations(Entity entity)
    {
        Clear();
        int variants = _animator.GetInteger(Constants.IDLE_ANIMATION + Constants.VARIANTS);
        AddAnimation(Constants.IDLE_ANIMATION, variants);
        TryAddMoveAnimation(entity);
        TryAddDeathAnimation(entity);
        TryAddAttackAnimation(entity);
    }

    private void ResetAnimation()
    {
        _currentAnimation = "";
    }


    private bool TryAddMoveAnimation(Entity entity)
    {
        if (entity.TryGetComponent<GroundMoveComponent>(out var move))
        {
            int variants = _animator.GetInteger(Constants.MOVE_ANIMATION + Constants.VARIANTS);
            AddAnimation(Constants.MOVE_ANIMATION, variants);
            AddCondition(Constants.MOVE_ANIMATION, move.IsMoving);

            float multipluer = move.MoveSpeed / Constants.DEFAULT_MOVE_SPEED;
            SetMultipluer(Constants.MOVE_MULTIPLUER, multipluer);
            return true;
        }
        return false;
    }

    private bool TryAddAttackAnimation(Entity entity)
    {
        if (entity.TryGetComponent<BaseAttackComponent>(out var attack))
        {
            int variants = _animator.GetInteger(Constants.ATTACK_ANIMATION + Constants.VARIANTS);
            AddAnimation(Constants.ATTACK_ANIMATION, ref attack.OnAttack, attack.IsAttack, variants);
            float multipluer = 1 / (Constants.ATTACK_ANIMATION_KOEF * attack.AttackDelay);
            SetMultipluer(Constants.ATTACK_MULTIPLUER, multipluer);
            return true;
        }
        return false;
    }

    private bool TryAddDeathAnimation(Entity entity)
    {
        if (entity.TryGetComponent<HealthComponent>(out var health))
        {
            int variants = _animator.GetInteger(Constants.DEATH_ANIMATION + Constants.VARIANTS);
            Func<bool> isAlive = health.IsAlive;
            AddAnimation(Constants.DEATH_ANIMATION, ref health.OnDied, () => !isAlive.Invoke(), variants);
            return true;
        }
        return false;
    }
}
