using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer
{
    private string _animation;
    private Animator _animator;
    private Action _action;

    public AnimationPlayer(
        string animation, 
        Animator animator, 
        ref Action action
        )
    {
        _animation = animation;
        _animator = animator;

        action += Play;
        _action = action;
    }

    public void Disable()
    {
        _action -= Play;
    }

    private void Play()
    {
        _animator.Play(_animation);
    }
}

[Serializable]
public class AnimationComponent : BaseComponent, 
    IComponent, IUpdatable, IEnable, IDisable, IDisposable
{
    private List<AnimationPlayer> _animPlayers = new();
    private Animator _animator;
    private Func<bool> _moveCondition;
    private string _moveAnimation;

    public AnimationComponent(Animator animator)
    {
        _name = "Animation Component";
        if (animator != null)
        {
            _animator = animator;
        }

        Enable();
    }

    public void Add(string animation, ref Action action)
    {
        AnimationPlayer player = new AnimationPlayer(animation, _animator, ref action);
        _animPlayers.Add(player);
    }

    public void Add(string animation, Func<bool> isMoving)
    {
        _moveAnimation = animation;
        _moveCondition = isMoving;
    }
    public void SetMultipluer(string multipluer, float value)
    {
        _animator.SetFloat(multipluer, value);
    }

    public void Clear()
    {
        foreach (var player in _animPlayers)
        {
            player.Disable();
        }
        _animPlayers.Clear();
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
        if(_moveAnimation != null)
        {
            _animator?.SetBool(_moveAnimation, _moveCondition.Invoke());
        }
    }

    public void Dispose()
    {
        Disable();
        Clear();
    }
}
