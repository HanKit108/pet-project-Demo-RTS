using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesComponent : BaseComponent, 
    IComponent, ISwitchable, IEnable, IDisable
{
    [SerializeField]
    private List<Ability> _abilities = new();
    [SerializeField]
    private Entity _owner;
    private Action _enableAction, _disableAction;

    public AbilitiesComponent(Entity owner)
    {
        _name = "Abilities Component";
        _owner = owner;
    }

    public void SetSwitchEvents(ref Action enableAction, ref Action disableAction)
    {
        enableAction += Enable;
        disableAction += RemoveAll;
        _enableAction = enableAction;
        _disableAction = disableAction;
    }

    public void Enable()
    {
        for (int i = 0; i < _abilities.Count; i++)
        {
            _abilities[i].Add(_owner);
        }
    }

    public void Disable()
    {
        for (int i = 0; i < _abilities.Count; i++)
        {
            _abilities[i].Remove(_owner);
        }
    }

    public void Add<T>(T other) where T : AbilityConfigSO
    {
        for (int i = 0; i < _abilities.Count; i++)
        {
            if (_abilities[i].Name == other.Name)
            {
                return;
            }
        }
        var ability2 = other.Get();
        ability2.Add(_owner);
        _abilities.Add(other.Get());
    }

    public void Remove<T>(T other) where T : AbilityConfigSO
    {
        for (int i = 0; i < _abilities.Count; i++)
        {
            if (_abilities[i].Name == other.Name)
            {
                _abilities[i].Remove(_owner);
                _abilities.Remove(_abilities[i]);
            }
        }
    }

    public void RemoveAll()
    {
        for (int i = 0; i < _abilities.Count; i++)
        {
            _abilities[i].Remove(_owner);
        }
        _abilities.Clear();
    }

    public bool Contain(AbilityConfigSO config)
    {
        for (int i = 0; i < _abilities.Count; i++)
        {
            if (_abilities[i].Name == config.Name)
            {
                return true;
            }
        }
        return false;
    }
}


public class AbilitiesComponentCreator : BaseComponentCreator, IComponentCreator
{
    [SerializeField]
    private List<AbilityConfigSO> _abilities = new();

    public AbilitiesComponentCreator()
    {
        _name = "Abilities Component";
    }

    public void CreateComponent(Entity entity)
    {
        AbilitiesComponent abilities = new AbilitiesComponent(entity);
        foreach (var ability in _abilities)
        {
            abilities.Add(ability);
        }
        TrySetLifeEvents(entity, abilities);
        entity.Add(abilities);
    }
}