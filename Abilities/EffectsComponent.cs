using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectsComponent : BaseComponent, IComponent
{
    [SerializeField]
    private List<Effect> _effects = new();

    public EffectsComponent()
    {
        _name = "Effects Component";
    }

    public void Add(int id, Action applyAction, Action dispelAction, float duration)
    {
        if (TryGet(id, out var other))
        {
            other.Refresh(duration);
        }
        else
        {
            applyAction();
            var effect = new Effect(id, dispelAction, duration);
            _effects.Add(effect);
            effect.OnCompleted += () => Remove(effect);
        }
    }

    public void Remove(Effect effect)
    {
        effect.Dispel();
        effect.OnCompleted -= () => Remove(effect);
        _effects.Remove(effect);
    }

    public void DispelAll()
    {
        for (int i = 0; i < _effects.Count; i++)
        {
            _effects[i].Dispel();
        }
    }

    public bool TryGet(long id, out Effect effect)
    {
        for (int i = 0; i < _effects.Count; i++)
        {
            if (_effects[i].Id == id)
            {
                effect = _effects[i];
                return true;
            }
        }
        effect = null;
        return false;
    }
}


public class EffectsComponentCreator : BaseComponentCreator, IComponentCreator
{
    public EffectsComponentCreator()
    {
        _name = "Effects Component";
    }

    public void CreateComponent(Entity entity)
    {
        EffectsComponent effects = new EffectsComponent();
        entity.Add(effects);
    }
}