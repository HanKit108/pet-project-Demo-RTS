using System;
using UnityEngine;

[Serializable]
public class Ability
{
    [SerializeField]
    private AbilityConfigSO _abilitySO;
    [SerializeField, HideInInspector]
    private string _name;
    [SerializeField]
    private bool _enabled;

    private bool _active;
    public bool Active => _active;

    public string Name => _name;

    public Ability(string name, AbilityConfigSO abilityConfigSO)
    {
        _name = name;
        _abilitySO = abilityConfigSO;
    }

    public void Add(Entity owner)
    {
        _abilitySO.Add(owner); 
    }

    public void Remove(Entity owner)
    {
        _abilitySO.Remove(owner);
    }
}
