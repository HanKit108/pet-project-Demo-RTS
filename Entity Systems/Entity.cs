using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Entity : MonoBehaviour
{
    [SerializeReference]
    private List<IComponent> _components = new List<IComponent>();

    public bool TryGetComponent<T>(out T other)
    {
        foreach (var component in _components)
        {
            if(component is T tComponent)
            {
                other = tComponent;
                return true;
            }
        }
        other = default(T);
        return false;
    }

    public T GetComponent<T>()
    {
        foreach (var component in _components)
        {
            if (component is T)
            {
                return (T) component;
            }
        }
        return default(T);
    }

    public void Add<T>(T other) where T : IComponent
    {
        foreach (var component in _components)
        {
            if (component is T)
            {
                return;
            }
        }
        _components.Add(other);
    }

    public void Remove<T>() where T : IComponent
    {
        foreach (var component in _components)
        {
            if (component is T)
            {
                _components.Remove(component);
            }
        }
    }

    public List<IComponent> GetComponentList()
    {
        return _components;
    }

    public void Dispose()
    {
        foreach (var component in _components)
        {
            if (component is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        _components.Clear();
    }
}